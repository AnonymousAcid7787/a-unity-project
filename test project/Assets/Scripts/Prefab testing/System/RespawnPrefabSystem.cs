using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using Unity.Transforms;
using Unity.Burst;

[BurstCompile]
public partial struct RespawnPrefabSystem : ISystem {
    EntityQuery prefabsQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<RespawnPrefabComponent>();

        prefabsQuery = SystemAPI.QueryBuilder()
            .WithAll<PrefabInfoComponent>()
            .Build();
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state) {}
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        //var is BeginSimulationEntityCommandBufferSystem.Singleton if ur confused
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();

        new RespawnPrefabJob {
            ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged),
            prefabs = prefabsQuery.ToComponentDataArray<PrefabInfoComponent>(Allocator.TempJob),
        }.Schedule();
    }
}

[BurstCompile]
public partial struct RespawnPrefabJob : IJobEntity {
    public EntityCommandBuffer ecb;

    [ReadOnly]
    [DeallocateOnJobCompletion]
    public NativeArray<PrefabInfoComponent> prefabs;

    [BurstCompile]
    public void Execute(in Entity self, in RespawnPrefabComponent respawnData) {
        Entity prefab = Entity.Null;

        for(var i = 0; i < prefabs.Length; i++) {
            if(prefabs[i].prefabType == respawnData.prefabType) {
                prefab = prefabs[i].prefab;
                break;
            }
        }

        if(prefab == Entity.Null) {
            ecb.DestroyEntity(self);
            return;
        }

        Entity newEntity = ecb.Instantiate(prefab);
        ecb.SetComponent(newEntity, LocalTransform.FromPosition(respawnData.position));

        ecb.DestroyEntity(self);
    }
}