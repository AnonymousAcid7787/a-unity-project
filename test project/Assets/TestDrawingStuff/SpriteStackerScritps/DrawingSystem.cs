using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Collections.LowLevel.Unsafe;

public partial struct DrawingSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        
    }

    public void OnDestroy(ref SystemState state)
    {
        
    }

    public void OnUpdate(ref SystemState state)
    {
        EntityManager eManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        foreach(DrawDataComponent component in SystemAPI.Query<DrawDataComponent>()) {
            DrawData data = InstancingCache.cache[component.cacheIndex];
            InstanceData instanceData = data.instanceData[component.instanceDataIndex];
            TransformAspect transformAspect = eManager.GetAspect<TransformAspect>(component.baseEntity);
            
            instanceData.worldMatrix = 
                Matrix4x4.TRS(transformAspect.LocalPosition, transformAspect.LocalRotation, component.scale);
            instanceData.worldMatrixInverse =
                Matrix4x4.Inverse(instanceData.worldMatrix);
            
            
            DrawData drawData = InstancingCache.cache[component.cacheIndex];
            drawData.Draw();
        }

    }
}