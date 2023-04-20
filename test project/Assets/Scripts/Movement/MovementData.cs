using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

//Generic movement data that stored the usual information for entities that move on their own.
public struct MovementData : IComponentData
{
    public bool isGrounded;
    public float movementSpeed;
    public float jumpHeight;
    // public float gravityModifier;
}
