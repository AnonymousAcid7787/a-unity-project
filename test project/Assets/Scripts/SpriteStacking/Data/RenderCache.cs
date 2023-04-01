using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderCache
{
    public static List<RenderInfo> renderCache = new List<RenderInfo>();

    public static int CacheInfo(Material material, Mesh mesh) {
        for(var i=0; i<renderCache.Count; i++) {
            RenderInfo cached = renderCache[i];
            if(cached.material == null) {
                renderCache.RemoveAt(i);
                continue;
            }
            if(cached.material.mainTexture == material.mainTexture) {
                return i;
            }
        }

        RenderInfo renderInfo = new RenderInfo(material, mesh);
        renderCache.Add(renderInfo);
        return renderCache.Count-1;
    }
}
