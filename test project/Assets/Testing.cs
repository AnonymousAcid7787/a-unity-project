using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Entities;

public class Testing : MonoBehaviour
{
    public Sprite[] sprites;
    public Material baseMaterial;

    void Start() {
        EntityManager eManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityArchetype archetype = eManager.CreateArchetype(typeof(RenderComponent));
        for(var i=0; i<sprites.Length; i++) {
            Sprite s = sprites[i];
            //Make new material for mesh
            Material spriteMaterial = new Material(baseMaterial); /*this is messing smth up*/
            spriteMaterial.mainTexture = s.texture;
            
            Vector3 pos = gameObject.transform.position+new Vector3(0, 0.1f*i, 0);
            //Cache the material
            MaterialCache.CacheRenderInfo(
                        spriteMaterial,
                        pos,
                        gameObject.transform.rotation,
                        new Vector3(1,1,1),
                        new Bounds(Vector3.zero, new Vector3(10f, 10f, 10f))
                    );
            Entity entity = eManager.CreateEntity(archetype);
            eManager.SetComponentData(entity, new RenderComponent{
                materialIndex = MaterialCache.cachedRenderInfo.Count-1
            });
        }
        // if(MaterialCache.cachedRenderInfo == null || MaterialCache.cachedRenderInfo == default) {
        //     MaterialCache.cachedRenderInfo = new List<RenderInfo>();
        // }
        // eManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        // spriteEntity = eManager.CreateEntity(typeof(RenderComponent));
        // materialIndex = MaterialCache.cachedRenderInfo.Count;
        // eManager.SetComponentData(spriteEntity, new RenderComponent{
        //     materialIndex = materialIndex
        // });
        // MaterialCache.CacheRenderInfo(
        //     spriteMaterial,
        //     transform.position,
        //     transform.rotation,
        //     transform.localScale
        // );
    }
}