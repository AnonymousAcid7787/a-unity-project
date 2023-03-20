using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct PlayerSpawnerComponent : IComponentData
{
    public Entity playerPrefab;

}