using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

public partial struct IsGroundedSystem : ISystem {
    private static SimulationSingleton simulationSingleton;
    private static bool getSingleton;
    public void OnCreate(ref SystemState state) {

    }
    public void OnDestroy(ref SystemState state) {

    }
    public void OnUpdate(ref SystemState state) {
        if(!getSingleton) {
            simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
            getSingleton = true;
        }
        JobHandle handle = new IsGroundedJob{}.Schedule(simulationSingleton, state.Dependency);
        handle.Complete();
    }
}

// public partial struct GroundCheckSystem : ISystem
// {
//     private SimulationSingleton simulationSingleton;
//     private bool getSingleton;

//     public void OnCreate(ref SystemState state)
//     {
        
//     }

//     public void OnUpdate(ref SystemState state) {
//         if(!getSingleton) {
//             simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
//             getSingleton = true;
//         }
//         JobHandle handle = new IsGroundedJob{}.Schedule(simulationSingleton, state.Dependency);
//         handle.Complete();
//     }

//     public void OnDestroy(ref SystemState state) {}
// }

public partial struct IsGroundedJob : ICollisionEventsJob {
    public void Execute(CollisionEvent collisionEvent) {
        // if(collisionEvent.Normal.y < 0)
        //     Debug.Log("on ground");
        // else
        //     Debug.Log("not on ground");
        Debug.Log("test");
    }
}
