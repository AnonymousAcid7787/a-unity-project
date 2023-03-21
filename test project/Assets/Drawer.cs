using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;

public partial struct Drawer : ISystem
{
    

    public void OnCreate(ref SystemState state)
    {
        
    }

    public void OnDestroy(ref SystemState state)
    {
        
    }

    public void OnUpdate(ref SystemState state)
    {
        JobHandle handle = new DrawerJob {}.ScheduleParallel(state.Dependency);
        handle.Complete();
    }
}

public partial struct DrawerJob : IJobEntity {
    public void Execute(RenderComponent component) {
        RenderInfo info = MaterialCache.renderInfos[component.renderInfoIndex];
        Graphics.DrawMeshNow(info.mesh, info.position, info.rotation, 0);
    }
}