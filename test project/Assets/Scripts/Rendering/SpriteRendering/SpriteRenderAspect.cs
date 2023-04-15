using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct SpriteRenderAspect : IAspect
{
    public readonly Entity entity;
    
    public readonly RefRW<LocalTransform> localTransform;
    public readonly RefRW<SpriteSheetAnimationData> animationData;
}
