using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Entities;

public class Testing : MonoBehaviour
{
    public Material spriteMaterial;

    private Entity spriteEntity;
    private int materialIndex;
    private EntityManager eManager;

    void Start() {
        if(MaterialCache.cachedRenderInfo == null || MaterialCache.cachedRenderInfo == default) {
            MaterialCache.cachedRenderInfo = new List<RenderInfo>();
        }
        eManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        spriteEntity = eManager.CreateEntity(typeof(RenderComponent));
        materialIndex = MaterialCache.cachedRenderInfo.Count;
        eManager.SetComponentData(spriteEntity, new RenderComponent{
            materialIndex = materialIndex
        });
        MaterialCache.CacheRenderInfo(
            spriteMaterial,
            transform.position,
            transform.rotation,
            transform.localScale
        );
    }

    void OnDestroy() {
        MaterialCache.RemoveCachedRenderInfo(materialIndex);
    }
}
