using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

public struct PhysicsUtils
{
    public static void RemoveEntityFromList(ref NativeList<ColliderCastHit> hits, in Entity entityToIgnore)  {
        for(var i=0; i<hits.Length; i++) {
            if(hits[i].Entity == entityToIgnore) 
                hits.RemoveAt(i);
        }
    }
}
