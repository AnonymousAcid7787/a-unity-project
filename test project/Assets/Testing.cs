using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Entities;

public class Testing : MonoBehaviour
{
    public Sprite spriteSheet;
    public List<Texture2D> textures;
    public Material baseMaterial;

     void Start() {
        textures = GetSlicedSpriteTextures(spriteSheet);

        EntityManager eManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityArchetype archetype = eManager.CreateArchetype(typeof(RenderComponent));
        
        for(var i=0; i<textures.Count; i++) {
            Texture2D tex = textures[i];
            
            Material spriteMaterial = Instantiate(baseMaterial);
            spriteMaterial.mainTexture = tex;
            
            Vector3 pos = new Vector3(
                UnityEngine.Random.Range(-3,3),
                UnityEngine.Random.Range(-3,3),
                UnityEngine.Random.Range(-3,3)
            );

            //Cache the material
            MaterialCache.CacheRenderInfo(
                        spriteMaterial,
                        pos,
                        gameObject.transform.rotation,
                        new Vector3(1,1,1),
                        new Bounds(Vector3.zero, new Vector3(10f, 10f, 10f))
                    );
            
            //Create entity
            Entity entity = eManager.CreateEntity(archetype);
            eManager.SetComponentData(entity, new RenderComponent{
                materialIndex = MaterialCache.cachedRenderInfo.Count-1
            });
        }
    }

    //Returns array of textures in a sliced sprite.
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