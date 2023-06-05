using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public partial struct ChunkLoaderISystem : ISystem {

    [BurstCompile]  
    public void OnCreate(ref SystemState state) {
    
    }

    public void OnUpdate(ref SystemState state) {
        if(WorldManagement.currentWorldChunks.ChunkMapArea == 0) {
            //Create chunks in GameSettings.chunkRenderDistance
        }
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
