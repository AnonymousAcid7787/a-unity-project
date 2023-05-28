using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct RespawnPrefabComponent : IComponentData {
    public float3 position;
    public PrefabType prefabType;
}