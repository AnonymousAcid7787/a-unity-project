using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(InstanceDataUpdate3D))]
public partial class RenderSystem3D : SystemBase
{
    protected override void OnCreate()
    {
        RenderCache3D.cache = new Dictionary<int, DrawInfo3D>();
    }

    protected override void OnDestroy()
    {
        RenderCache3D.ClearCache();
    }

    protected override void OnUpdate()
    {
        foreach(RenderData3D data in SystemAPI.Query<RenderData3D>()) {
            DrawInfo3D drawInfo = RenderCache3D.cache[data.drawInfoHashCode];
            drawInfo.Draw();
        }
    }
}