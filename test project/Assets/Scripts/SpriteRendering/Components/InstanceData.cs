using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct InstanceData : IComponentData
{
    public float4x4 worldMatrix;
    public float4x4 worldMatrixInverse;
    // public float2 uvTiling;
    // public float2 uvOffset;

    public static int Size() {
        return ((sizeof(float) * 4*4) * 2) /*world matrix/inverse matrix*/
                // + (sizeof(float) * 4) /*UV tiling & offset*/
                ;
    }
}

//Will add more in the future