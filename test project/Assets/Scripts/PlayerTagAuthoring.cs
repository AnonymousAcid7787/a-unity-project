using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct PlayerTag : IComponentData {}

public class PlayerTagAuthoring : MonoBehaviour {}

public class PlayerTagBaker : Baker<PlayerTagAuthoring> {
    public override void Bake(PlayerTagAuthoring authoring) {
        AddComponent<PlayerTag>(GetEntity(TransformUsageFlags.None));
    }
}