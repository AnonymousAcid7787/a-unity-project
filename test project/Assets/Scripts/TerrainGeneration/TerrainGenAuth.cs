using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;
using UnityEngine;

public class TerrainGenAuth : MonoBehaviour 
{
    public int size = 20;
    public int roughness = 2;
    public int minHeight = 1;
    public int maxHeight = 8;
    public int seed;
}

public class TerrainGenBaker : Baker<TerrainGenAuth>
{
    public override void Bake(TerrainGenAuth authoring)
    {
        int size = authoring.size % 2 == 0 ? authoring.size+1 : authoring.size;
        
        Unity.Mathematics.Random random = new Unity.Mathematics.Random((uint)authoring.seed);
        int[,] grid = TerrainGenUtils.DiamondSquare(size, authoring.roughness, authoring.minHeight, authoring.maxHeight, random);

        int vertexCount = (size + 1) * (size + 1);
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[size * size * 6];

        //Vertices
        for(int i = 0,z = 0; z <= size; z++) {
            for(int x = 0; x <= size; x++) {
                // float y = Mathf.PerlinNoise(x*0.3f, z*0.3f)*2;
                vertices[i] = new float3(x, 0, z);
                i++;
            }
        }

        //Triangles
        int vert = 0;
        int tris = 0;
        for (int z = 0; z < size; z++) {
            for(int x = 0; x < size; x++) {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + size + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + size + 1;
                triangles[tris + 5] = vert + size + 2;

                vert++;
                tris += 6;
            }
            vert++; 
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        authoring.gameObject.GetComponent<MeshFilter>().mesh = mesh;

        AddComponent(GetEntity(TransformUsageFlags.None), new TerrainGenTag{});
    }
}

public struct TerrainGenTag : IComponentData {}

public class TerrainGenUtils {
    public static int[,] DiamondSquare(int gridSize, int roughness, int minHeight, int maxHeight, RefRW<RandomComponent> randomCmp) {
        #region error checking
        if(gridSize < 2)
            throw new System.InvalidOperationException("Grid is too small!");
        
        if(gridSize % 2 == 0) 
            throw new System.InvalidOperationException("Grid size is even!");
        #endregion error checking

        int[,] grid = new int[gridSize, gridSize];

        #region set four corners
        grid[0         , 0         ] = randomCmp.ValueRW.random.NextInt(minHeight, maxHeight);
        grid[0         , gridSize-1] = randomCmp.ValueRW.random.NextInt(minHeight, maxHeight);
        grid[gridSize-1, 0         ] = randomCmp.ValueRW.random.NextInt(minHeight, maxHeight);
        grid[gridSize-1, gridSize-1] = randomCmp.ValueRW.random.NextInt(minHeight, maxHeight);
        #endregion set four corners

        #region the algorithm
        int chunkSize = gridSize - 1;
        
        while(chunkSize > 1) {
            int half = chunkSize / 2;
            
            //This is good
            #region square step
            for(var y = 0; y < gridSize-1; y += chunkSize) {
                for(var x = 0; x < gridSize-1; x += chunkSize) {
                    grid[y+half, x+half] =
                        grid[y          , x          ] +
                        grid[y          , x+chunkSize] +
                        grid[y+chunkSize, x          ] +
                        grid[y+chunkSize, x+chunkSize];
                    grid[y+half, x+half] /= 4;

                    grid[y+half, x+half] += randomCmp.ValueRW.random.NextInt(-roughness, roughness);
                }
            }
            #endregion square step

            #region diamond step   
            for(var y = 0; y < gridSize; y += half) {
                for(var x = (y+half) % chunkSize; x < gridSize; x += chunkSize) {
                    int up = y-half >= 0         ? grid[y-half, x] : 0;
                    int down = y+half < gridSize ? grid[y+half, x] : 0;
                    int left = x-half >= 0       ? grid[y, x-half] : 0;
                    int right = x+half < gridSize? grid[y, x+half] : 0;
                    
                    grid[y, x] = up+down+left+right;
                    grid[y, x] /= 4;

                    grid[y, x] += randomCmp.ValueRW.random.NextInt(-roughness, roughness);
                }
            }
            #endregion diamond step

            chunkSize /= 2;
            roughness /= 2;
        }
        #endregion the algorithm

        return grid;
    }

    public static int[,] DiamondSquare(int gridSize, int roughness, int minHeight, int maxHeight, Unity.Mathematics.Random random) {
        #region error checking
        if(gridSize < 2)
            throw new System.InvalidOperationException("Grid is too small!");
        
        if(gridSize % 2 == 0) 
            throw new System.InvalidOperationException("Grid size is even!");
        #endregion error checking

        int[,] grid = new int[gridSize, gridSize];

        #region set four corners
        grid[0         , 0         ] = random.NextInt(minHeight, maxHeight);
        grid[0         , gridSize-1] = random.NextInt(minHeight, maxHeight);
        grid[gridSize-1, 0         ] = random.NextInt(minHeight, maxHeight);
        grid[gridSize-1, gridSize-1] = random.NextInt(minHeight, maxHeight);
        #endregion set four corners

        #region the algorithm
        int chunkSize = gridSize - 1;
        
        while(chunkSize > 1) {
            int half = chunkSize / 2;
            
            //This is good
            #region square step
            for(var y = 0; y < gridSize-1; y += chunkSize) {
                for(var x = 0; x < gridSize-1; x += chunkSize) {
                    grid[y+half, x+half] =
                        grid[y          , x          ] +
                        grid[y          , x+chunkSize] +
                        grid[y+chunkSize, x          ] +
                        grid[y+chunkSize, x+chunkSize];
                    grid[y+half, x+half] /= 4;

                    grid[y+half, x+half] += random.NextInt(-roughness, roughness);
                }
            }
            #endregion square step

            #region diamond step   
            for(var y = 0; y < gridSize; y += half) {
                for(var x = (y+half) % chunkSize; x < gridSize; x += chunkSize) {
                    int up = y-half >= 0         ? grid[y-half, x] : 0;
                    int down = y+half < gridSize ? grid[y+half, x] : 0;
                    int left = x-half >= 0       ? grid[y, x-half] : 0;
                    int right = x+half < gridSize? grid[y, x+half] : 0;
                    
                    grid[y, x] = up+down+left+right;
                    grid[y, x] /= 4;

                    grid[y, x] += random.NextInt(-roughness, roughness);
                }
            }
            #endregion diamond step

            chunkSize /= 2;
            roughness /= 2;
        }
        #endregion the algorithm

        return grid;
    }
}
