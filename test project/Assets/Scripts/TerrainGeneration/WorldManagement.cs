using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;

/** <summary>
* A 3d array of all the chunks
* </summary> 
*/
public struct ChunkMap {
    
    public int3 chunkGenerationOrigin;
    public int chunkMapSize;
    public int chunkSize;
    public int minHeight;
    public int maxHeight;
    public float frequency;
    public float lacunarity;
    public int octaves;
    public float persistence;

    NativeList<WorldChunk> chunks;

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

        chunks = new NativeList<WorldChunk>(Allocator.Persistent);
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

    public WorldChunk this[int x, int y, int z] {
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
* These are what populate the chunks in a ChunkMap.
* 
* This struct contains info of all the Voxels in a chunk.
* </summary> 
*/
public struct WorldChunk {
        
    int chunkSize;
    NativeArray<Voxel> voxels;

    public Voxel this[int x, int y, int z] {
        get {
            return voxels[x + chunkSize * (y + chunkSize * z)];
        }
        set {
            voxels[x + chunkSize * (y + chunkSize * z)] = value;
        }
    }
    
    public WorldChunk(int chunkSize) {
        this.chunkSize = chunkSize;
        voxels = new NativeArray<Voxel>(chunkSize*chunkSize*chunkSize, Allocator.Persistent);
    }
}

public struct Voxel {

}
