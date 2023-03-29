using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct DrawDataComponent : IComponentData
{
    public int drawDataCacheIndex;
    public int instanceDataIndex;

    public float3 positionOffset;
    public float3 scale;
}
