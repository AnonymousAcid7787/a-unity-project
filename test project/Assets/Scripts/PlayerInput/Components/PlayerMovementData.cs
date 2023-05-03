using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

//This file contains all results of key presses
public struct PlayerInputData : IComponentData
{
    public float2 movementDirection;
    public bool jump;
}