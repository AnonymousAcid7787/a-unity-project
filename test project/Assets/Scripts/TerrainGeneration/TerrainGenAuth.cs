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

public struct TerrainGenUtils {
    public static void DiamondSquare(ref int[,] heightValues, int height, int defaultVal, RefRW<RandomComponent> randomCmp) {
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

        int maxHeight = height;
        
        int size = gridHeight;
        //Set four corners
        heightValues[0     , 0     ] = defaultVal;
        heightValues[0     , size-1] = defaultVal;
        heightValues[size-1, 0     ] = defaultVal;
        heightValues[size-1, size-1] = defaultVal;

        int sideLength = size-1;
        while(sideLength >= 2) {
            sideLength /= 2;
            height /= 2;
            
            #region square step
            int halfSide = sideLength/2;
            for(var r = 0; r < size-1; r += sideLength) {
                for(var c = 0; c < size-1; c += sideLength) {
                    int currentCell = heightValues[r, c];

                    //avg
                    heightValues[r+halfSide, c+halfSide] =
                        heightValues[r           , c             ] +
                        heightValues[r           , c + sideLength] +
                        heightValues[r+sideLength, c             ] +
                        heightValues[r+sideLength, c+sideLength  ];
                    heightValues[r+halfSide, c+halfSide] /= 4;
                    
                    double randVal = randomCmp.ValueRW.random.NextDouble(0,1);
                    heightValues[r+halfSide, c+halfSide] += (int)(randVal * 2 * height);
                }
            }
            #endregion square step

            #region diamond step
            for(var r = 0; r < size-1; r += halfSide) {
                for(var c = (r + halfSide) % sideLength; c < size-1; c += sideLength) {

                    //avg
                    heightValues[r, c] = 
                        heightValues[r-halfSide, c         ] +
                        heightValues[r         , c-halfSide] +
                        heightValues[r         , c+halfSide] +
                        heightValues[r+halfSide, c         ];
                    heightValues[r, c] /= 4;

                    double randVal = randomCmp.ValueRW.random.NextDouble(0,1);
                    heightValues[r, c] += (int)(randVal * 2 * height);
                }
            }
            #endregion diamond step
        }
    }
}
