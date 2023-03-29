using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

//Use this class when adding instance data to a buffer
public struct InstanceDataStruct {
    public float4x4 worldMatrix;
    public float4x4 worldMatrixInverse;

    public static int Size() {
        return sizeof(float) * 4 * 4
            + sizeof(float) * 4 * 4;
    }
}

//Use this class when not adding instance data to a buffer
public class InstanceDataClass {
    public float4x4 worldMatrix {
        get {
            return bufferStruct.worldMatrix;
        }
        set {
            bufferStruct.worldMatrix = value;
        }
    }
    public float4x4 worldMatrixInverse {
        get {
            return bufferStruct.worldMatrixInverse;
        }
        set {
            bufferStruct.worldMatrixInverse = value;
        }
    }
    public InstanceDataStruct bufferStruct;

    public InstanceDataClass(float4x4 worldMatrix, float4x4 worldMatrixInverse) {
        bufferStruct = new InstanceDataStruct {
            worldMatrix = worldMatrix,
            worldMatrixInverse = worldMatrixInverse
        };

        this.worldMatrix = worldMatrix;
        this.worldMatrixInverse = worldMatrixInverse;

    }
}