using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;

[BurstCompile]
public partial struct ChunkLoaderISystem : ISystem {

    [BurstCompile]  
    public void OnCreate(ref SystemState state) {
    
    }

    public void OnUpdate(ref SystemState state) {
        /* require local player for to load chunks based off of its position in the world */
        if(!SystemAPI.HasSingleton<PlayerCharacterTag>())
            return;
        
        RefRW<LocalTransform> playerTransform = SystemAPI.GetComponentRW<LocalTransform>(SystemAPI.GetSingletonEntity<PlayerCharacterTag>(), true);
        

        /* create chunks in chunk map */
        
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {
    
    }
    
    [BurstCompile]
    public partial struct ChunkLoaderISystemJob : IJobEntity {
        public void Execute() {
        
        }
    }
}
