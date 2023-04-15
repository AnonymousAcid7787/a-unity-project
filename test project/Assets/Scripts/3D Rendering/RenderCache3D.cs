using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderCache3D
{
    public static Dictionary<int, DrawInfo3D> cache = new Dictionary<int, DrawInfo3D>();

    public static DrawInfo3D Cache3DDrawInfo(RenderArgs args) {
        foreach(KeyValuePair<int, DrawInfo3D> cachedInfo in cache) {
            if(cachedInfo.Value.material.mainTexture == args.material.mainTexture) {
                return cachedInfo.Value;
            }
        }

        DrawInfo3D drawInfo = new DrawInfo3D(args);
            cache.Add(drawInfo.GetHashCode(), drawInfo);
        return drawInfo;
    }

    public static void ClearCache() {
        foreach(KeyValuePair<int, DrawInfo3D> cachedInfo in cache) {
            cachedInfo.Value.DestroyBuffers();
        }
        cache.Clear();
    }
}
