using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;


public partial struct LockPlayerRotationSystem : ISystem
{
    public static bool lockPlayerXZRotation = true;

        
    public void OnCreate(ref SystemState state) {
        lockPlayerXZRotation = true;
    }

    
    public void OnUpdate(ref SystemState state) {
        state.Dependency = new LockPlayerRotationJob{}.ScheduleParallel(state.Dependency);
    }
    
    partial struct LockPlayerRotationJob : IJobEntity {
        public void Execute(ref LocalTransform localTransform, in PlayerInputData data) {
            if(lockPlayerXZRotation) {
                localTransform.Rotation.value.x = 0;
                localTransform.Rotation.value.y = 0;
                localTransform.Rotation.value.z = 0;
            }
        }
    }
}
