using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

//This file contains all components that hold results of key presses

public struct PlayerMovementData : IComponentData
{
    public float3 movementDirection;
    public bool jump;
    public float movementSpeed;
    // public float gravityModifier;
}