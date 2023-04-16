using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;

//Update instance data after altering it. this happens right before rendering the instances
[UpdateAfter(typeof(UpdateRenderPositions3D))]
public partial struct InstanceDataUpdate3D : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        
    }

    public void OnDestroy(ref SystemState state)
    {
        
    }

    public void OnUpdate(ref SystemState state)
    {
        JobHandle handle = new InstanceDataUpdateJob3D{}.ScheduleParallel(state.Dependency);
        handle.Complete();
    } 
}

//Update the draw information to have the right instance data.
partial struct InstanceDataUpdateJob3D: IJobEntity {
    public void Execute(in RenderData3D data) {
        DrawInfo3D drawInfo = RenderCache3D.cache[data.drawInfoHashCode];
        drawInfo.instances[data.instanceKey] = data.instanceData;
    }
}