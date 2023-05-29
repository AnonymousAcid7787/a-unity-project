using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;

public class TerrainGeneratorAuthoring : MonoBehaviour
{
    public int3 chunkPosition;
    public int chunkSize = 10;
    public int minHeight = 1;
    public int maxHeight = 8;
    public float frequency;
    public float lacunarity;
    public int octaves;
    public float persistence;
}

public class TerrainGeneratorBaker : Baker<TerrainGeneratorAuthoring>
{
    public override void Bake(TerrainGeneratorAuthoring authoring)
    {
        int chunkSize = authoring.chunkSize;

        int[,,] voxelMap = new int[chunkSize, chunkSize, chunkSize];

        int[,] noiseGrid = FractalNoiseInt(
            authoring.chunkPosition.x, authoring.chunkPosition.y,
            chunkSize, chunkSize,
            authoring.minHeight, authoring.maxHeight,
            authoring.frequency,
            authoring.octaves,
            authoring.lacunarity,
            authoring.persistence
        );

        for(var x = 0 ; x < chunkSize; x ++) {
            for(var z = 0 ; z < chunkSize; z ++) {
                //"create" voxels at the heights specified by the noiseGrid.
                voxelMap[x, noiseGrid[z,x], z] = 1;
            }
        }
        
        int arrayLength = chunkSize*chunkSize*chunkSize;
        int[] flatArrayManaged = Utils.Flatten3DArray(voxelMap, chunkSize, chunkSize, chunkSize);
        NativeArray<int> voxelMapFlattened = new NativeArray<int>(flatArrayManaged, Allocator.Persistent);

        AddComponent(GetEntity(TransformUsageFlags.None), new TerrainGeneratorComponent{
            chunkSize = chunkSize,
            mapSize = chunkSize,
            voxelMapFlattened = voxelMapFlattened
        });
    }
    
    public static float[,] FractalNoise(int chunkX, int chunkY, int gridWidth, int gridHeight, int minHeight, int maxHeight, float frequency, int octaves, float lacunarity, float persistence) {

		float[,] grid = new float[gridHeight, gridWidth];
		float amplitude = maxHeight/2f;

		for(int y = chunkX; y < gridHeight; y++) {
			for(int x = chunkY; x < gridWidth; x++) {
				float cellElevation = amplitude;
				float tFrequency = frequency;
				float tAmplitude = amplitude;

				for(int octave = 0; octave < octaves; octave++) {
                    float2 sampleVec = new float2(x * tFrequency, y * tFrequency);
					cellElevation +=  noise.snoise(sampleVec) * tAmplitude;

					tFrequency *= lacunarity;
					tAmplitude *= persistence;
				}

				cellElevation = Mathf.Clamp(cellElevation, minHeight, maxHeight);
				grid[y, x] = cellElevation;
			}
		}

		return grid;
	}

    public static int[,] FractalNoiseInt(int chunkX, int chunkY, int gridWidth, int gridHeight, int minHeight, int maxHeight, float frequency, int octaves, float lacunarity, float persistence) {

		int[,] grid = new int[gridHeight, gridWidth];
		float amplitude = maxHeight/2f;

		for(int y = chunkX; y < gridHeight; y++) {
			for(int x = chunkY; x < gridWidth; x++) {
				float cellElevation = amplitude;
				float tFrequency = frequency;
				float tAmplitude = amplitude;

				for(int octave = 0; octave < octaves; octave++) {
                    float2 sampleVec = new float2(x * tFrequency, y * tFrequency);
					cellElevation += noise.snoise(sampleVec) * tAmplitude;

					tFrequency *= lacunarity;
					tAmplitude *= persistence;
				}

				cellElevation = Mathf.Clamp(cellElevation, minHeight, maxHeight);
				grid[y, x] = (int)cellElevation;
			}
		}

		return grid;
	}
}

public struct TerrainGeneratorComponent : IComponentData {
    public NativeArray<int> voxelMapFlattened;/*flattened 3D array*/
    public int mapSize;
    public int chunkSize;
}
