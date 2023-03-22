using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;

public class MaterialCache
{
    public static List<RenderInfo> cachedRenderInfo;

    public static RenderInfo CacheRenderInfo(Material material, Vector3 position, Quaternion rotation, Vector3 scale) {
        if(cachedRenderInfo == null || cachedRenderInfo == default) {
            cachedRenderInfo = new List<RenderInfo>();
        }

        int index = GetMaterialIndex(material);
        if(index != -1) {
            cachedRenderInfo[index].AddInstance(position, rotation, scale);
            return cachedRenderInfo[index];
        }

        RenderInfo info = new RenderInfo(
                material,
                position, 
                rotation, 
                scale, 
                new Bounds(Vector3.zero, new Vector3(10f,10f,10f))
                );

        cachedRenderInfo.Add(
            info
        );
        return info;
    }
    
    public static void RemoveCachedRenderInfo(int index) {
        if(cachedRenderInfo == null || cachedRenderInfo == default) {
            cachedRenderInfo = new List<RenderInfo>();
            return;
        }
        cachedRenderInfo.RemoveAt(index);
        new IndexUpdaterJob{
            indexDeleted = index
        }.Run();
    }

    public static int GetMaterialIndex(Material material) {
        if(cachedRenderInfo == null || cachedRenderInfo == default) {
            cachedRenderInfo = new List<RenderInfo>();
        }

        for(var i=0; i<cachedRenderInfo.Count; i++) {
            RenderInfo info = cachedRenderInfo[i];
            if(info.material == material) {
                return i;
            }
        }
        return -1;
    } 

}

public partial struct IndexUpdaterJob : IJobEntity {
    public int indexDeleted;
    public void Execute(RenderComponent component) {
        if(component.materialIndex > indexDeleted) {
            component.materialIndex--;
        }
    }
}