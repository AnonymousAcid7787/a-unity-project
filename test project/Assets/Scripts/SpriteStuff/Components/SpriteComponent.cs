using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

public class SpriteComponent : IComponentData
{
    public InstanceDataObject instanceData;
    // public float4 color;
    public int materialCacheIndex;
}
