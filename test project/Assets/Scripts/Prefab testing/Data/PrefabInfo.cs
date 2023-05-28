using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public enum PrefabType {
    Cube,
}

public struct PrefabInfoComponent : IComponentData {
    public Entity prefab;
    public PrefabType prefabType;
}

public class PrefabInfo : MonoBehaviour {
    public GameObject prefab;
    public PrefabType prefabType;
}

public class PrefabInfoBaker : Baker<PrefabInfo>
{
    public override void Bake(PrefabInfo authoring)
    {
        AddComponent(GetEntity(TransformUsageFlags.None), new PrefabInfoComponent {
            prefab = GetEntity(authoring.prefab, TransformUsageFlags.Renderable),
            prefabType = authoring.prefabType,
        });
    }
}