using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

public struct SpriteStack : IComponentData
{
    public NativeList<Entity> spriteEntities;
    public bool updatedParentEntity;
}