using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct PlayerCharacterTag : IComponentData {}

public class PlayerCharacterTagAuthoring : MonoBehaviour {}

public class PlayerCharacterTagBaker : Baker<PlayerCharacterTagAuthoring> {
    public override void Bake(PlayerCharacterTagAuthoring authoring) {
        AddComponent<PlayerCharacterTag>(GetEntity(TransformUsageFlags.None));
    }
}