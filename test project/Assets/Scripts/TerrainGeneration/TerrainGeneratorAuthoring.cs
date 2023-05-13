using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class TerrainGeneratorAuthoring : MonoBehaviour
{
    public int width;
    public int height;
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
        int xSize = authoring.width;
        int zSize = authoring.height;
        float[,] grid = FractalNoise(
            xSize,
            zSize,
            authoring.minHeight,
            authoring.maxHeight,
            authoring.frequency,
            authoring.octaves,
            authoring.lacunarity,
            authoring.persistence);

        int vertexCount = (xSize + 1) * (zSize + 1);
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[xSize * zSize * 6];

        #region vertices
        //Set vertex values
        for(int i = 0,z = 0; z <= zSize; z++) {
            for(int x = 0; x <= xSize; x++) {
                vertices[i] = new Vector3(x, 0, z);
                i++;
            }
        }
        #endregion vertices

        #region triangles
        int vert = 0;
        int tris = 0;
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
        #endregion triangles

        for(int i = 0,z = 0; z < zSize; z++) {
            for(int x = 0; x < xSize; x++) {
                vertices[i].y = grid[z, x];
                i++;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        authoring.gameObject.GetComponent<MeshFilter>().mesh = mesh;
    }

    
    public static float[,]  DiamondSquareFloat(int gridSize, int roughness, int minHeight, int maxHeight, Unity.Mathematics.Random random) {
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
    

    public static float[,] FractalNoise(int gridWidth, int gridHeight, int minHeight, int maxHeight, float frequency, int octaves, float lacunarity, float persistence) {

        float[,] grid = new float[gridHeight, gridWidth];
        float amplitude = maxHeight/2f;

        for(int y = 0; y < gridHeight; y++) {
            for(int x = 0; x < gridWidth; x++) {
                float cellElevation = amplitude;
                float cellFrequency = frequency;
                float cellAmplitude = amplitude;

                //Perlin noise octaves
                for(int octave = 0; octave < octaves; octave++) {
                    float sampleX = x * cellFrequency;
                    float sampleY = y * cellFrequency;
                    cellElevation += Mathf.PerlinNoise(sampleX, sampleY) * cellAmplitude;

                    cellFrequency *= lacunarity;
                    cellAmplitude *= persistence;
                }

                cellElevation = Mathf.Clamp(cellElevation, minHeight, maxHeight);
                grid[y, x] = cellElevation;
            }
        }

        return grid;
    }
}