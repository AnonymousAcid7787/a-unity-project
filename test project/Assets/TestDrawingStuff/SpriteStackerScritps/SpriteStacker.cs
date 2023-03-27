using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;

public class SpriteStacker : MonoBehaviour
{
    public Sprite spriteSheet;
    public Material baseMaterial;
    public List<Texture2D> textures;

    public List<Entity> spriteEntities;


    void Update() {
        transform.Translate(new Vector3(0.01f, 0, 0));
    }

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

public class SpriteStackerBaker : Baker<SpriteStacker>
{
    public override void Bake(SpriteStacker authoring)
    {
        SpriteStackTag tagComponent = new SpriteStackTag();
        AddComponent(tagComponent);


        authoring.spriteEntities = new List<Entity>();

        List<Texture2D> textures = SpriteStacker.GetSlicedSpriteTextures(authoring.spriteSheet);
            textures.Reverse();

        EntityManager eManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityArchetype archetype = eManager.CreateArchetype(typeof(DrawDataComponent));
        
        Bounds bounds = new Bounds(Vector3.zero, new Vector3(10, 10, 10));
        
        for(var i=0; i<textures.Count; i++) {
            Texture2D tex = textures[i];

            Vector3 offset = new Vector3(0, 0.05f*i, 0);
            
            Material material = new Material(authoring.baseMaterial);
                material.mainTexture = tex;

            DrawDataComponent component = InstancingCache.CacheSpriteEntity(
                material,
                DrawData.NewQuadMesh(),
                _State.PrimaryEntity,
                offset,
                Vector3.one,
                bounds
            );
            
            Entity spriteEntity = eManager.CreateEntity(archetype);
            eManager.SetComponentData(spriteEntity, component);

            authoring.spriteEntities.Add(spriteEntity);
        }
    }
}