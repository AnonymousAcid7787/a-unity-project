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
        float[,] grid = TerrainGenUtils.DiamondSquareFloat(size, authoring.roughness, authoring.minHeight, authoring.maxHeight, random);

        int vertexCount = (size + 1) * (size + 1);
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[size * size * 6];


        #region vertices
        //Set vertex values
        for(int i = 0,z = 0; z <= size; z++) {
            for(int x = 0; x <= size; x++) {
                vertices[i] = new float3(x, 0, z);
                i++;
            }
        }
        for(int i = 0,z = 0; z < size; z++) {
            for(int x = 0; x < size; x++) {
                vertices[i].y = grid[z,x];
                i++;
            }
        }
        #endregion vertices

        #region triangles
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
        #endregion triangles

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

        if(Mathf.Log((float)gridSize-1, 2)%1 != 0)
            throw new System.InvalidOperationException("Grid size is is not \"(n^2)+1!\"");
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
                    int up = 0;
                    int down = 0;
                    int left = 0;
                    int right = 0;

                    int count = 0;
                    #region diamond corners
                    if(y-half >= 0) {//up
                        up = grid[y-half, x];
                        count ++;
                    }
                    if(y+half < gridSize) {//down
                        down = grid[y+half, x];
                        count++;
                    }
                    if(x-half >= 0) {//left
                        left = grid[y, x-half];
                        count++;
                    }
                    if(x+half < gridSize) {//right
                        right = grid[y, x+half];
                        count++;
                    }
                    #endregion diamond corners
                    
                    grid[y, x] = up+down+left+right;
                    grid[y, x] /= count;

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

        if(Mathf.Log((float)gridSize-1, 2)%1 != 0)
            throw new System.InvalidOperationException("Grid size is is not \"(n^2)+1!\"");
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
                    int up = 0;
                    int down = 0;
                    int left = 0;
                    int right = 0;

                    int count = 0;
                    #region diamond corners
                    if(y-half >= 0) {//up
                        up = grid[y-half, x];
                        count ++;
                    }
                    if(y+half < gridSize) {//down
                        down = grid[y+half, x];
                        count++;
                    }
                    if(x-half >= 0) {//left
                        left = grid[y, x-half];
                        count++;
                    }
                    if(x+half < gridSize) {//right
                        right = grid[y, x+half];
                        count++;
                    }
                    #endregion diamond corners
                    
                    grid[y, x] = up+down+left+right;
                    grid[y, x] /= count;

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

    public static float[,] DiamondSquareFloat(int gridSize, int roughness, int minHeight, int maxHeight, Unity.Mathematics.Random random) {
        #region error checking
        if(gridSize < 2)
            throw new System.InvalidOperationException("Grid is too small!");
        
        if(gridSize % 2 == 0) 
            throw new System.InvalidOperationException("Grid size is even!");

        if(Mathf.Log((float)gridSize-1, 2)%1 != 0)
            throw new System.InvalidOperationException("Grid size is is not \"(n^2)+1!\"");
        #endregion error checking

        float[,] grid = new float[gridSize, gridSize];

        #region set four corners
        grid[0         , 0         ] = random.NextFloat(minHeight, maxHeight);
        grid[0         , gridSize-1] = random.NextFloat(minHeight, maxHeight);
        grid[gridSize-1, 0         ] = random.NextFloat(minHeight, maxHeight);
        grid[gridSize-1, gridSize-1] = random.NextFloat(minHeight, maxHeight);
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
                        (grid[y          , x          ] +
                        grid[y          , x+chunkSize] +
                        grid[y+chunkSize, x          ] +
                        grid[y+chunkSize, x+chunkSize])/4;

                    grid[y+half, x+half] += random.NextFloat(-roughness, roughness);
                }
            }
            #endregion square step

            #region diamond step   
            for(var y = 0; y < gridSize; y += half) {
                for(var x = (y+half) % chunkSize; x < gridSize; x += chunkSize) {
                    float up = 0;
                    float down = 0;
                    float left = 0;
                    float right = 0;

                    int count = 0;
                    #region diamond corners
                    if(y-half >= 0) {//up
                        up = grid[y-half, x];
                        count++;
                    }
                    if(y+half < gridSize) {//down
                        down = grid[y+half, x];
                        count++;
                    }
                    if(x-half >= 0) {//left
                        left = grid[y, x-half];
                        count++;
                    }
                    if(x+half < gridSize) {//right
                        right = grid[y, x+half];
                        count++;
                    }
                    #endregion diamond corners
                    
                    grid[y, x] = (up+down+left+right)/count;
                    
                    grid[y, x] += random.NextFloat(-roughness, roughness);
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
