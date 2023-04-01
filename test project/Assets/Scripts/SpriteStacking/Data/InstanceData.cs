using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class InstanceDataObject {
    public float4x4 worldMatrix {
        get {
            return instanceDataStruct.worldMatrix;
        }
        set {
            instanceDataStruct.worldMatrix = value;
        }
    }
    public float4x4 worldMatrixInverse {
        get {
            return instanceDataStruct.worldMatrixInverse;
        }
        set {
            instanceDataStruct.worldMatrixInverse = value;
        }
    }

    public InstanceData instanceDataStruct = new InstanceData{
        worldMatrix = float4x4.zero,
        worldMatrixInverse = float4x4.zero
    };

    public InstanceDataObject(float4x4 worldMatrix, float4x4 worldMatrixInverse) {
        this.worldMatrix = worldMatrix;
        this.worldMatrixInverse = worldMatrixInverse;
    }
}

public struct InstanceData
{
    public float4x4 worldMatrix;
    public float4x4 worldMatrixInverse;
    // public float4 color;

    public static int Size() {
        return (sizeof(float) * 4*4)*2;
    }
}
