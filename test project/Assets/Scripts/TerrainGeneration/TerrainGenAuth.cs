using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class TerrainGenAuth : MonoBehaviour 
{
    public int xSize = 20;
    public int zSize = 20;
}

public class TerrainGenBaker : Baker<TerrainGenAuth>
{
    public override void Bake(TerrainGenAuth authoring)
    {
        int xSize = authoring.xSize - 1;
        int zSize = authoring.zSize - 1;

        int vertexCount = (xSize + 1) * (zSize + 1);
        Vector3[] vertices = new Vector3[vertexCount];
        
        //Vertices
        for(int i = 0,z = 0; z <= zSize; z++) {
            for(int x = 0; x <= xSize; x++) {
                float y = Mathf.PerlinNoise(x * 0.3f, z * 0.3f) * 2f;
                vertices[i] = new float3(x, y, z);
                i++;
            }
        }

        //Triangles
        int vert = 0;
        int tris = 0;
        int triangleCount = xSize * zSize * 6;
        int[] triangles = new int[triangleCount];
        for (int z = 0; z < zSize; z++) {
            for(int x = 0; x < xSize; x++) {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++; 
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        authoring.gameObject.GetComponent<MeshFilter>().mesh = mesh;

        AddComponent(GetEntity(TransformUsageFlags.None), new TerrainGenTag{});
    }
}

public struct TerrainGenTag : IComponentData {}

public class TerrainGenUtils {
    public static void DiamondSquare(ref int[,] heightValues, int roughness, int defaultVal, RefRW<RandomComponent> randomCmp) {
        int gridWidth = heightValues.GetUpperBound(1)+1;
        int gridHeight = heightValues.GetUpperBound(0)+1;

        #region error checking
        if(gridWidth != gridHeight)
            throw new System.InvalidOperationException("Inputted grid is not a square shape!");
        
        if(gridHeight < 2)
            throw new System.InvalidOperationException("Grid is too small!");
        
        if(gridHeight % 2 == 0) 
            throw new System.InvalidOperationException("Grid size is even!");
        #endregion error checking

        int maxHeight = roughness;
        
        int size = gridHeight;
        //Set four corners
        heightValues[0     , 0     ] = defaultVal;
        heightValues[0     , size-1] = defaultVal;
        heightValues[size-1, 0     ] = defaultVal;
        heightValues[size-1, size-1] = defaultVal;

        int chunkSize = size-1;
        while(chunkSize >= 2) {
            chunkSize /= 2;
            roughness /= 2;
            
            #region square step
            int halfSide = chunkSize/2;
            for(var r = 0; r < size-1; r += chunkSize) {
                for(var c = 0; c < size-1; c += chunkSize) {
                    int currentCell = heightValues[r, c];

                    //avg
                    heightValues[r+halfSide, c+halfSide] =
                        heightValues[r           , c             ] +
                        heightValues[r           , c + chunkSize] +
                        heightValues[r+chunkSize, c             ] +
                        heightValues[r+chunkSize, c+chunkSize  ];
                    heightValues[r+halfSide, c+halfSide] /= 4;
                    
                    double randVal = randomCmp.ValueRW.random.NextDouble(0,1);
                    heightValues[r+halfSide, c+halfSide] += (int)(randVal * 2 * roughness);
                }
            }
            #endregion square step

            #region diamond step
            for(var r = 0; r < size; r += halfSide) {
                for(var c = (r + halfSide) % chunkSize; c < size; c += chunkSize) {

                    //avg
                    heightValues[r, c] = 
                        heightValues[r-halfSide, c         ] +
                        heightValues[r         , c-halfSide] +
                        heightValues[r         , c+halfSide] +
                        heightValues[r+halfSide, c         ];
                    heightValues[r, c] /= 4;

                    double randVal = randomCmp.ValueRW.random.NextDouble(0,1);
                    heightValues[r, c] += (int)(randVal * 2 * roughness);
                }
            }
            #endregion diamond step


            GUIUtility.systemCopyBuffer = heightValues.ToString();
        }
    }

    public static int[,] DiamondSquare2(int gridSize, int roughness, int minHeight, int maxHeight, RefRW<RandomComponent> randomCmp) {
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
                for(int y = 0; y < half; y += half) {
                    for(int x = (y+half) % chunkSize; x < chunkSize; x += chunkSize) {
                        int up = y-half >= 0 ? grid[y-half, x] : 0;
                        int down = y+half < gridSize ? grid[y, x-half] : 0;
                        int left = 

                        grid[y, x] =
                            up +
                             +
                            grid[y     , x+half] +
                            grid[y+half, x     ];
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
}
