using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct DrawDataComponent
{
    public int materialCacheIndex;
    public MaterialInstanceData instanceData;
    public float3 positionOffset;
}
