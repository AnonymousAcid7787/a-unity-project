using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

public struct SpriteSheetAnimationData : IComponentData {
    public int currentFrame;
    public int frameCount;
    public float frameTimer;
    public float frameTimerMax;

    public int drawInfoHashCode;
    public int instanceKey;
    public InstanceData instanceData;
    public NativeArray<Rect> uvRects;
}