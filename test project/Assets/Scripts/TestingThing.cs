using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

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
    public void OnCreate(ref SystemState state) {

    }

    public void OnDestroy(ref SystemState state) {
        
    }

    public void OnUpdate(ref SystemState state) {
        state.Dependency = new TestJob{}.ScheduleParallel(state.Dependency);
    }

    partial struct TestJob : IJobEntity {
        public void Execute(in Entity entity, in TerrainGeneratorComponent terrainGenCmp) {
            Debug.Log(entity.ToString());
        }
    }
}