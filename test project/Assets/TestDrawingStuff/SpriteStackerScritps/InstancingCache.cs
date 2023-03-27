using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Transforms;
using Unity.Entities;

public class InstancingCache
{
    public static List<DrawData> cache = new List<DrawData>();

    public static DrawDataComponent CacheSpriteEntity(
        Material material, Mesh mesh,
        Entity baseEntity,
        Vector3 positionOffset,
        Vector3 scale,
        Bounds renderBounds
    ) {
        
        DrawDataComponent component;
        
        //if material texture is already cached, then just add an instance.
        for(var i=0; i<cache.Count; i++) {
            DrawData drawData = cache[i];

            if(drawData.material == null) {
                cache.Remove(drawData);
                continue;
            }

            if(drawData.material.mainTexture == material.mainTexture) {
                drawData.AddInstance(Vector3.zero, Quaternion.identity, scale);
                component = new DrawDataComponent {
                    cacheIndex = i,
                    positionOffset = positionOffset,
                    instanceDataIndex = drawData.instanceData.Count-1,
                    baseEntity = baseEntity,
                    scale = scale
                };
                return component;
            }
        }

        //if not cached yet, cache it.
        DrawData data = new DrawData(material, mesh, new MaterialPropertyBlock(), renderBounds);
            data.AddInstance(Vector3.zero, Quaternion.identity, scale);
        cache.Add(data);

        component = new DrawDataComponent {
            cacheIndex = cache.Count-1,
            positionOffset = positionOffset,
            instanceDataIndex = data.instanceData.Count-1,
            baseEntity = baseEntity,
            scale = scale
        };
        return component;

    }
}