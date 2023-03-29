using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

public struct SpriteStackComponent : IComponentData
{
    public int test;
    public NativeList<DrawDataComponent> spriteDrawData;
}
