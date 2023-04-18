using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MovementAuthoring : MonoBehaviour
{
    public TransformUsageFlags transformUsageFlags;
}

public class MovementBaker : Baker<MovementAuthoring>
{
    public override void Bake(MovementAuthoring authoring)
    {
        AddComponent<MovementData>(GetEntity(authoring.transformUsageFlags));
    }
}
