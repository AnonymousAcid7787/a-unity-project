using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public readonly partial struct SpriteStackAspect : IAspect
{
    public readonly Entity entity;

    public readonly RefRW<LocalTransform> localTransform;
    public readonly RefRW<SpriteStack> spriteStack;
}
