using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;

/** <summary>
* Overwrites the terrain gen configuration for the current world.
* When a save system is added, this will save the current world configuration if there is any
* </summary> 
*/
public class TerrainInitializeAuthoring : MonoBehaviour
{
    public int3 chunkGenerationOrigin;
    public int chunkSize = 9;
	public int chunkRenderDistance = 1;
	[Space]
    public int minHeight = 0;
    public int maxHeight = 8;
    public float frequency;
    public float lacunarity;
    public int octaves;
    public float persistence;
}

public class TerrainInitializeBaker : Baker<TerrainInitializeAuthoring>
{
    public override void Bake(TerrainInitializeAuthoring authoring)
    {
        int chunkSize = authoring.chunkSize;
		int chunkMapSize = authoring.chunkRenderDistance;
		GameSettings.chunkRenderDistance = authoring.chunkRenderDistance;

        ChunkMap chunkMap = new ChunkMap(
            chunkMapSize,
            authoring.chunkSize,
			authoring.chunkGenerationOrigin,
			authoring.minHeight, authoring.maxHeight,
			authoring.frequency,
			authoring.lacunarity,
			authoring.octaves,
			authoring.persistence);
    }
}