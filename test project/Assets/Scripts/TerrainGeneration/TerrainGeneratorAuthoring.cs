using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class TerrainGeneratorAuthoring : MonoBehaviour
{
    public int2 chunkPosition;
    public int chunkWidth = 10;
    public int chunkHeight = 10;
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
        int xSize = authoring.chunkWidth;
        int zSize = authoring.chunkHeight;
        float[,] grid = FractalNoise(
            authoring.chunkPosition.x, authoring.chunkPosition.y,
            authoring.chunkWidth, authoring.chunkHeight,
            authoring.minHeight, authoring.maxHeight,
            authoring.frequency,
            authoring.octaves,
            authoring.lacunarity,
            authoring.persistence
        );



        // Mesh mesh = new Mesh();
        // mesh.vertices = vertices;
        // mesh.triangles = triangles;
        // mesh.RecalculateBounds();
        // mesh.RecalculateNormals();
        // authoring.gameObject.GetComponent<MeshFilter>().mesh = mesh;
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
    

    public static float[,] FractalNoise(int chunkX, int chunkY, int gridWidth, int gridHeight, int minHeight, int maxHeight, float frequency, int octaves, float lacunarity, float persistence) {

		float[,] grid = new float[gridHeight, gridWidth];
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
				grid[y, x] = cellElevation;
			}
		}

		return grid;
	}
}