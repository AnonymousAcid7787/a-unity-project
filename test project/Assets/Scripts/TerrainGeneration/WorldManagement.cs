using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;

public struct WorldManagement {

    public static ChunkMap currentWorldChunks;

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
    private NativeList<Entity> chunks;

    /* If this chunk map wasn't initialized via constructor, this would be false. */
    private bool notNull;

    /**
    * <summary>
    * When constructed, the chunk map will not have any chunks. <br />
    * The <see cref="ChunkLoaderISystem"/> will update the current world's ChunkMap (<see cref="WorldManagement.currentWorldChunks"/>)
    * </summary>
    */
    public ChunkMap(int chunkSize, int3 chunkGenerationOrigin, int minHeight, int maxHeight, float frequency, float lacunarity, int octaves, float persistence) {
        notNull = true;

        this.chunkMapSize = 0;
        this.chunkSize = chunkSize;
        this.chunkGenerationOrigin = chunkGenerationOrigin;
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
        this.frequency = frequency;
        this.lacunarity = lacunarity;
        this.octaves = octaves;
        this.persistence = persistence;

        chunks = new NativeList<Entity>(Allocator.Persistent);
    }

    /**
    * <summary>
    * This is false when this ChunkMap hasn't been initalized via constructor.
    * </summary>
    */
    public bool NotNull {
        get {
            return notNull;
        }
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
    
    /* Chunk size in voxels */
    int chunkSizeVoxels;
    /* Chunk volume in voxels */
    int chunkVolumeVoxels;
    NativeArray<Voxel> voxels;
    
    public WorldChunk(int chunkSizeVoxels) {
        this.chunkSizeVoxels = chunkSizeVoxels;
        this.chunkVolumeVoxels = chunkSizeVoxels * chunkSizeVoxels*chunkSizeVoxels;
        voxels = new NativeArray<Voxel>(chunkSizeVoxels*chunkSizeVoxels*chunkSizeVoxels, Allocator.Persistent);
    }

    public Voxel this[int x, int y, int z] {
        get {
            return voxels[x + chunkSizeVoxels * (y + chunkSizeVoxels * z)];
        }
        set {
            voxels[x + chunkSizeVoxels * (y + chunkSizeVoxels * z)] = value;
        }
    }
    
    /** <summary>
    * Returns chunk size in units
    * </summary> 
    */
    public double ChunkSize {
        get {
            return chunkSizeVoxels * Voxel.size;
        }
    }

    /** <summary>
    * Returns chunk volume in units
    * </summary> 
    */
    public double ChunkVolume {
        get {
            return chunkVolumeVoxels * Voxel.size;
        }
    }
}

public struct Voxel {
    public static double size = 0.1;
    public static double volume = size*size*size;
}
