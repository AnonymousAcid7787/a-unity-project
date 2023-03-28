using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
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
        //update draw positions based on offsets
        JobHandle handle = new UpdateDrawPositionsJob {}.ScheduleParallel(state.Dependency);
        handle.Complete();

        //after that, draw.
        foreach(SpriteStackComponent spriteStackComponent in SystemAPI.Query<SpriteStackComponent>()) {
            for(var i=0; i<spriteStackComponent.spriteDrawData.Length; i++) {
                DrawDataComponent drawDataComponent = spriteStackComponent.spriteDrawData[i];
                int drawDataIndex =  drawDataComponent.drawDataCacheIndex;

                DrawData drawData = InstancingCache.cache[drawDataIndex];
                drawData.UpdateBuffers();
                drawData.UpdateMaterialBuffer();
                drawData.Draw();
            }
        }
    }
}

public partial class TestSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref LocalTransform localTransform) => {
            // localTransform.Position += new Unity.Mathematics.float3(SystemAPI.Time.DeltaTime, 0, 0);
            localTransform.Rotation = 
                math.mul(localTransform.Rotation, quaternion.RotateZ(math.radians(SystemAPI.Time.DeltaTime*50)));
                // Unity.Mathematics.quaternion.EulerXYZ(new Unity.Mathematics.float3 {
                //     x = localTransform.Rotation.value.x,
                //     y = localTransform.Rotation.value.y,
                //     z = localTransform.Rotation.value.z+0.1f,
                // });
            
        }).Run();
        // foreach(LocalTransform localTransform in SystemAPI.Query<LocalTransform>()) {
        //     Unity.Mathematics.quaternion q = Unity.Mathematics.quaternion.EulerXYZ(new Unity.Mathematics.float3 {
        //             x = 1,
        //             y = 0,
        //             z = 0,
        //         });
        // }
    }
}

public partial struct UpdateDrawPositionsJob : IJobEntity {
    public void Execute(ref LocalTransform localTransform, ref SpriteStackComponent spriteStackComponent) {
        for(var i=0; i<spriteStackComponent.spriteDrawData.Length; i++) {
            DrawDataComponent drawDataComponent = spriteStackComponent.spriteDrawData[i];
            int drawDataIndex =  drawDataComponent.drawDataCacheIndex;
            int instanceDataIndex = drawDataComponent.instanceDataIndex;

            DrawData drawData = InstancingCache.cache[drawDataIndex];
            /*contains the transforms of the thing to draw*/
            InstanceDataClass instanceData = drawData.instanceDataObjs[instanceDataIndex];

            //update the location based on offset
            instanceData.worldMatrix = Matrix4x4.TRS(
                localTransform.Position + drawDataComponent.positionOffset,
                localTransform.Rotation,
                Vector3.one
            );
            instanceData.worldMatrixInverse = Matrix4x4.Inverse(instanceData.worldMatrix);
        }
    }
}