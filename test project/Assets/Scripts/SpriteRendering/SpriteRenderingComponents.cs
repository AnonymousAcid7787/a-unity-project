using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct InstanceData : IComponentData
{
    public float4x4 worldMatrix;
    public float4x4 worldMatrixInverse;
}

//Will add more in the future