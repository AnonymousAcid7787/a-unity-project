using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialCache
{
    public static List<CachedMaterial> cachedMaterials = new List<CachedMaterial>();

    //Returns the index of the cached material
    public static int CacheMaterial(Material material, Mesh mesh) {
        for(var i=0; i<cachedMaterials.Count; i++) {
            if(cachedMaterials[i].material.mainTexture == material.mainTexture) {
                return i;
            }
        }
        cachedMaterials.Add(new CachedMaterial(material, mesh));
        return cachedMaterials.Count-1;
    }
}
