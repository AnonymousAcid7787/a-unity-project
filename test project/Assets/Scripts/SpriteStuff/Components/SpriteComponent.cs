using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

public struct SpriteComponent : IComponentData
{
    public InstanceData instanceData;
    // public float4 color;
    public int materialCacheIndex;
}
