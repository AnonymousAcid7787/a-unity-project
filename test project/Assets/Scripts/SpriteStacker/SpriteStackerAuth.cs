using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

public class SpriteStackAuth : MonoBehaviour
{
    public Material baseMaterial;
    public Mesh mesh;
    public Sprite spriteSheet;

    public static List<Texture2D> GetSlicedSpriteTextures(Sprite sprite)
    {
        List<Texture2D> textures = new List<Texture2D>();
        Sprite[] sprites = Resources.LoadAll<Sprite>(sprite.texture.name);
        
        for(var i=0; i<sprites.Length; i++) {
            Sprite s = sprites[i];
            Rect sprRect = s.rect;
            Texture2D slicedTex = new Texture2D((int)sprRect.width, (int)sprRect.height);
            slicedTex.filterMode = sprite.texture.filterMode;
            
            Color[] colors = sprite.texture.GetPixels((int)sprRect.x, (int)sprRect.y, (int)sprRect.width, (int)sprRect.height);

            slicedTex.SetPixels(
                0, 0,
                (int)sprRect.width, (int)sprRect.height,
                colors
                );
            
            slicedTex.Apply();   
            slicedTex.name = s.name;    
            textures.Add(slicedTex);
        }
 
        return textures;
    }
}

public class SpriteStackBaker : Baker<SpriteStackAuth> {
    public override void Bake(SpriteStackAuth authoring)
    {
        List<Texture2D> textures = SpriteStackAuth.GetSlicedSpriteTextures(authoring.spriteSheet);
        SpriteStackComponent spriteStackComponent = new SpriteStackComponent{
            drawDatas = new NativeArray<DrawDataComponent>(textures.Count, Allocator.Persistent)
        };

        for(var i=0; i<textures.Count; i++) {
            Texture2D tex = textures[i];
            Material material = new Material(authoring.baseMaterial);
            material.mainTexture = tex;
            int materialIndex = MaterialCache.CacheMaterial(material, CachedMaterial.NewQuadMesh());

            float3 offset = new float3(0, i*0.1f, 0);

            Matrix4x4 worldMatrix = Matrix4x4.TRS(
                authoring.transform.position,
                authoring.transform.rotation,
                Vector3.one
            );

            MaterialInstanceData instanceData = new MaterialInstanceData{
                worldMatrix = worldMatrix,
                worldMatrixInverse = Matrix4x4.Inverse(worldMatrix)
            };

            DrawDataComponent drawData = new DrawDataComponent{
                materialCacheIndex = materialIndex,
                positionOffset = offset,
                instanceData = instanceData
            };

            spriteStackComponent.drawDatas[i] = drawData;
        }

        AddComponent(spriteStackComponent);
    }
}