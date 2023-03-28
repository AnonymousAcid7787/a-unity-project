using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

public struct SpriteStackComponent : IComponentData
{
    public NativeArray<DrawDataComponent> drawDatas;
}
