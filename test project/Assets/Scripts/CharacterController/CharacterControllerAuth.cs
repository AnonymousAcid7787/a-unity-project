using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CharacterControllerAuth : MonoBehaviour
{
    public float3 gravity = new float3(0.0f, -9.81f, 0.0f);
    public float maxSpeed = 7.5f;
    public float speed = 5.0f;
    public float jumpStrength = 0.15f;
    public float maxStep = 0.35f;
    public float drag = 0.2f;
}

public class CharacterControllerBaker : Baker<CharacterControllerAuth>
{
    public override void Bake(CharacterControllerAuth authoring)
    {
        AddComponent(GetEntity(TransformUsageFlags.Dynamic), new CharacterControllerComponent {
            Gravity = authoring.gravity,
            MaxSpeed = authoring.maxSpeed,
            Speed = authoring.speed,
            JumpStrength = authoring.jumpStrength,
            MaxStep = authoring.maxStep,
            Drag = authoring.drag
        });
    }
}
