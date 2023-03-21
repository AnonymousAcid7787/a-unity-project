using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct RenderComponent : IComponentData
{
    public int renderInfoIndex;
    public Entity entity;
}
