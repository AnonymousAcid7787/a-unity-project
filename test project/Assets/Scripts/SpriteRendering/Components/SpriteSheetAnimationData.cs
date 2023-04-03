using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct SpriteSheetAnimationData : IComponentData {
    public int currentFrame;
    public int frameCount;
    public float frameTimer;
    public float frameTimerMax;

    public int drawInfoHashCode;
    public int instanceDataHash;
}