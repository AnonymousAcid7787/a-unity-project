using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct RenderData3D : IComponentData
{
    public int drawInfoHashCode;

    public InstanceData instanceData;
    public int instanceKey;
}