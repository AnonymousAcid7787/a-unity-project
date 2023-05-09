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
        if(gridWidth != gridHeight)
            throw new System.InvalidOperationException("Inputted grid is not a square shape!");
        
        if(gridHeight < 2)
            throw new System.InvalidOperationException("Grid is too small!");
        
        if(gridHeight % 2 == 0) 
            throw new System.InvalidOperationException("Grid size is odd!");

        int maxHeight = height;
        
        int size = gridHeight;
        //Set four corners
        heightValues[0     , 0     ] = defaultVal;
        heightValues[0     , size-1] = defaultVal;
        heightValues[size-1, size-1] = defaultVal;
        heightValues[size-1, 0     ] = defaultVal;

        int sideLength = size-1;
        while(sideLength >= 2) {
            sideLength /= 2;
            height /= 2;

            int halfSide = sideLength/2;

            #region square step
            for(var sX = 0; sX < size-1; sX += sideLength) {
                for(var sY = 0; sY < size-1; sY += sideLength) {
                    int currentCell = heightValues[sX,sY];

                    if(currentCell == -1 || currentCell == 0)
                        continue;
                    
                    int avg = currentCell + 
                            heightValues[sX+sideLength, sY           ] +
                            heightValues[sX           , sY+sideLength] +
                            heightValues[sX+sideLength, sY+sideLength];
                    avg /= 4;

                    double randVal = randomCmp.ValueRW.random.NextDouble(0,1);

                    heightValues[sX+halfSide, sY+halfSide] =
                        (int)(avg + randVal * 2 * height);
                }
            }
            #endregion square step

            #region diamond step
            for(var dX = 0; dX < size-1; dX += halfSide) {
                for(var dY = (dX+halfSide)%sideLength; dY < size-1; dY += sideLength) {
                    int currentCell = heightValues[dX,dY];

                    if(currentCell == -1 || currentCell == 0)
                        continue;

                    int avg = heightValues[(dX-halfSide+size-1) % (size-1), dY] +
                              heightValues[(dX+halfSide) % (size-1)       , dY] +
                              heightValues[dX                             ,(dY+halfSide) % (size-1)] +
                              heightValues[dX                             ,(dY-halfSide+size-1) % (size-1)];
                    avg /= 4;

                    double randVal = randomCmp.ValueRW.random.NextDouble(0,1);
                    avg = (int)(avg + (randVal * 2 * height) - height);
                    
                    if(dX == 0)  
                        heightValues[size-1, dY] = avg;
	                if(dY == 0)  
                        heightValues[dX, size-1] = avg;
                }

            }
            #endregion diamond step
        
            for(var j = 0; j < size; j++) {
                for(var i = 0; i < size; i++) {
                    int currentCell = heightValues[i, j];

                    if(currentCell < 0) 
                        heightValues[i, j] = 0;
                    
                    if(currentCell > maxHeight) 
                        heightValues[i, j] = maxHeight;
                }
            }
        }
    }
}
