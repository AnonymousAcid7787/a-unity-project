using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public struct InstanceData {
    public float4x4 worldMatrix;
    public float4x4 worldMatrixInverse;

    public static int Size() {
        return sizeof(float) * 4 * 4
            + sizeof(float) * 4 * 4;
    }
}