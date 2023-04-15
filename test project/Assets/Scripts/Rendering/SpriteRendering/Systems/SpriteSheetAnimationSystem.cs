using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;

//Instance data modifier
[UpdateAfter(typeof(SpritePositionUpdate))]
[BurstCompile]
public partial struct SpriteSheetAnimationSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {
        
    }

    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        JobHandle handle = new SpriteFrameJob{
            deltaTime = deltaTime
        }.ScheduleParallel(state.Dependency);

        handle.Complete();

        new SpriteFrameAnimator{}.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct SpriteFrameJob : IJobEntity {
    public float deltaTime;

    [BurstCompile]
    public void Execute(ref SpriteSheetAnimationData animationData) {
        animationData.frameTimer += deltaTime;
        while(animationData.frameTimer >= animationData.frameTimerMax) {
            animationData.frameTimer -= animationData.frameTimerMax;
            animationData.currentFrame = (animationData.currentFrame + 1) % animationData.frameCount;
        }
    }
}

public partial struct SpriteFrameAnimator : IJobEntity {
    public void Execute(ref SpriteSheetAnimationData animationData) {
        SpriteSheetDrawInfo drawInfo = SpriteSheetCache.cache[animationData.drawInfoHashCode];
        Rect r = drawInfo.uvRects[animationData.currentFrame];
        animationData.instanceData.uvTiling = new float2(r.width, r.height);
        animationData.instanceData.uvOffset = new float2(r.x, r.y);
    }
}