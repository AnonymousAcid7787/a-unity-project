using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialCache : MonoBehaviour
{
    public static List<RenderInfo> cachedRenderInfo = new List<RenderInfo>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void CacheRenderInfo(Material material, Vector3 position, Quaternion rotation, Vector3 scale, Bounds bounds) {
        if(material == null || position == null || rotation == null || scale == null || bounds == null)
            return;

        //If material is cached, then add new instance to cached material and return.
        for(var i=0; i<cachedRenderInfo.Count; i++) {
            RenderInfo info = cachedRenderInfo[i];
            if(info.material.mainTexture == material.mainTexture || info.material == material) {
                info.AddInstance(position,rotation,scale,1);
                return;
            }
        }
        //If not cached, then cache the material
        cachedRenderInfo.Add(new RenderInfo(
            material,
            position,rotation,scale,
            bounds
        ));
    }
}
