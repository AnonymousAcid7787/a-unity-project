using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

/** <summary>
* 
* </summary> 
*/
public struct ChunkMap {
    
    int width;
    int height;
    int depth;

    NativeList<WorldChunk> chunks;

    public ChunkMap(int width, int height, int depth) {
        this.width = width;
        this.height = height;
        this.depth = depth;

        chunks = new NativeList<WorldChunk>(Allocator.Persistent);
        chunks.Resize(width*height*depth, NativeArrayOptions.UninitializedMemory);
    }

    public WorldChunk this[int x, int y, int z] {
        get {
            return chunks[x + height * (y + depth * z)];
        }
        set {
            chunks[x + height * (y + depth * z)] = value;
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
