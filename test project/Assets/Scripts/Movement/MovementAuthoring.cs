using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MovementAuthoring : MonoBehaviour
{
    public TransformUsageFlags transformUsageFlags;
    public float movementSpeed;
    public float jumpHeight;
    public float gravityFactor = 1;
}

public class MovementBaker : Baker<MovementAuthoring>
{
    public override void Bake(MovementAuthoring authoring)
    {
        AddComponent(GetEntity(authoring.transformUsageFlags), new MovementData {
            movementSpeed = authoring.movementSpeed,
            jumpHeight = authoring.jumpHeight,
            gravityFactor = authoring.gravityFactor
        });
    }
}
