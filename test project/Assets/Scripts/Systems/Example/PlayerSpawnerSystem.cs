using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public partial class PlayerSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        return;
        EntityQuery playerEntityQuery = EntityManager.CreateEntityQuery(typeof(PlayerTag));

        PlayerSpawnerComponent spawner = SystemAPI.GetSingleton<PlayerSpawnerComponent>();
        RefRW<RandomComponent> randComponent = SystemAPI.GetSingletonRW<RandomComponent>();

        EntityCommandBuffer entityCommandBuffer = 
            SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(
                World.Unmanaged
            );


        int playerSpawnCount = 2100;
        if(playerEntityQuery.CalculateEntityCount() < playerSpawnCount) {
            Entity entity = entityCommandBuffer.Instantiate(spawner.playerPrefab);
            entityCommandBuffer.SetComponent(entity, new Speed {
                value = randComponent.ValueRW.random.NextFloat(1f, 5f)
            });
        }
    }
}
