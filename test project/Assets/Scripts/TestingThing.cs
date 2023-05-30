using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Physics;
using Unity.Collections;

public class TestingThing : MonoBehaviour
{
    
}

public class TestBaker : Baker<TestingThing>
{
    public override void Bake(TestingThing authoring)
    {
        
    }
}

public partial struct TestSystem : ISystem {

    public static ChunkMap test;

    public void OnCreate(ref SystemState state) {
        test = new ChunkMap(10, 10, 10);

        EntityManager manager = state.EntityManager;
        Entity testEntity = manager.CreateEntity(typeof(TestComponent));
        manager.AddComponentData(testEntity, new TestComponent{});
    }

    public void OnDestroy(ref SystemState state) {
        
    }

    public void OnUpdate(ref SystemState state) {
        state.Dependency = new TestJob{}.ScheduleParallel(state.Dependency);
    }

    partial struct TestJob : IJobEntity {
        public void Execute(in Entity entity, in TestComponent terrainMap) {
            Debug.Log(test);
        }
    }
}


public struct TestComponent : IComponentData {
    
}