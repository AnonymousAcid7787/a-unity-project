using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;

public struct WorldManagement {
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

    public static NativeArray<int> FractalNoiseInt(int chunkX, int chunkY, int gridWidth, int gridHeight, int minHeight, int maxHeight, float frequency, int octaves, float lacunarity, float persistence, Allocator allocator) {

		NativeArray<int> grid = new NativeArray<int>(gridWidth*gridHeight, allocator);
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
				grid[Utils.Flat2DArrayIndex(gridHeight, x, y)] = (int)cellElevation;
			}
		}

		return grid;
	}
}

/** <summary>
* A 3D array of all the entities that represent chunks. <br />
* Also stores the terrain gen configuration.
* </summary> 
*/
public struct ChunkMap {
    
    /* The offset added to each chunk every time a chunk is created */
    public int3 chunkGenerationOrigin;
    /* The (cubed) size of this map. This is determined by the amount of chunks that should be loaded in (currently GameSettings.chunkRenderDistance) */
    public int chunkMapSize;
    /* (Cubed) Size of each chunk in this chunk map */
    public int chunkSize;

    #region Fractal noise parameters
    /* minimum height of terrain */
    public int minHeight;
    /* max height of terrain */
    public int maxHeight;
    /*  */
    public float frequency;
    public float lacunarity;
    public int octaves;
    public float persistence;
    #endregion Fractal noise parameters

    /* Entities that contain the WorldChunk component */
    NativeList<Entity> chunks;

    public ChunkMap(int chunkMapSize, int chunkSize, int3 chunkGenerationOrigin, int minHeight, int maxHeight, float frequency, float lacunarity, int octaves, float persistence) {
        this.chunkMapSize = chunkMapSize;
        this.chunkSize = chunkSize;
        this.chunkGenerationOrigin = chunkGenerationOrigin;
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
        this.frequency = frequency;
        this.lacunarity = lacunarity;
        this.octaves = octaves;
        this.persistence = persistence;

        chunks = new NativeList<Entity>(Allocator.Persistent);
        chunks.Resize(ChunkMapArea, NativeArrayOptions.UninitializedMemory);
    }

    public int ChunkMapArea {
        get {
            return chunkMapSize*chunkMapSize*chunkMapSize;
        }
    }

    public int ChunkArea {
        get {
            return chunkSize*chunkSize*chunkSize;
        }
    }

    public int ChunkMapVoxelArea {
        get {
            return ChunkMapArea*ChunkArea;
        }
    }

    public Entity this[int x, int y, int z] {
        get {
            return chunks[x + chunkMapSize * (y + chunkMapSize * z)];
        }
        set {
            chunks[x + chunkMapSize * (y + chunkMapSize * z)] = value;
        }
    }
    
}

/** <summary>
* World chunks are going to be cubes that split the world into a 3D grid.
* 
* This struct contains info of all the Voxels in a chunk.
* </summary> 
*/
public struct WorldChunk : IComponentData {
        
    int chunkSize;
    NativeArray<Voxel> voxels;
    
    public WorldChunk(int chunkSize) {
        this.chunkSize = chunkSize;
        voxels = new NativeArray<Voxel>(chunkSize*chunkSize*chunkSize, Allocator.Persistent);
    }

    public Voxel this[int x, int y, int z] {
        get {
            return voxels[x + chunkSize * (y + chunkSize * z)];
        }
        set {
            voxels[x + chunkSize * (y + chunkSize * z)] = value;
        }
    }
}

public struct Voxel {

}
