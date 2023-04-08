using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;

[BurstCompile]
public partial struct SpriteSheetAnimationSystem : ISystem
{
    public void OnCreate(ref SystemState state) {
        SpriteSheetCache.cache = new Dictionary<int, SpriteSheetDrawInfo>();
        SpriteSheetCache.cachedSpriteSheets = new Dictionary<Texture, Sprite[]>();
    }

    public void OnDestroy(ref SystemState state) {
        SpriteSheetCache.ClearCache();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        JobHandle handle = new SpriteSheetAnimator{
            deltaTime = deltaTime
        }.ScheduleParallel(state.Dependency);

        handle.Complete();
    }
}

[BurstCompile]
public partial struct SpriteSheetAnimator : IJobEntity {
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