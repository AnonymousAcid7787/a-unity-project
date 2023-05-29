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
        float[,] noiseGrid = FractalNoise(
            authoring.chunkPosition.x, authoring.chunkPosition.y,
            authoring.chunkWidth, authoring.chunkHeight,
            authoring.minHeight, authoring.maxHeight,
            authoring.frequency,
            authoring.octaves,
            authoring.lacunarity,
            authoring.persistence
        );

        //loop through noiseGrid
        //the current x and y value will be the x and z value of the voxels
        //the value of the cells in the noiseGrid will be the y value

        // Mesh mesh = new Mesh();
        // mesh.vertices = vertices;
        // mesh.triangles = triangles;
        // mesh.RecalculateBounds();
        // mesh.RecalculateNormals();
        // authoring.gameObject.GetComponent<MeshFilter>().mesh = mesh;
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
					cellElevation +=  noise.snoise(sampleVec) * tAmplitude;

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