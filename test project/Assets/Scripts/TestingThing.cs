using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Physics;
using Unity.Collections;
using Unity.Mathematics;

public class TestingThing : MonoBehaviour
{
    public int3 chunkPosition;
    public int chunkMapSize = 2;
    public int chunkSize = 9;
    public int minHeight = 1;
    public int maxHeight = 8;
    public float frequency;
    public float lacunarity;
    public int octaves;
    public float persistence;
}

public class TestBaker : Baker<TestingThing>
{
    public override void Bake(TestingThing authoring)
    {
        AddComponent(GetEntity(TransformUsageFlags.None), new TestComponent {
            chunkGenerationOrigin = authoring.chunkPosition,
            chunkSize = authoring.chunkSize,
            minHeight = authoring.minHeight,
            maxHeight = authoring.maxHeight,
            frequency = authoring.frequency,
            lacunarity = authoring.lacunarity,
            octaves = authoring.octaves,
            persistence = authoring.persistence,
        });
    }
}

public struct TestComponent : IComponentData {
    public int3 chunkGenerationOrigin;
    public int chunkMapSize;
    public int chunkSize;
    public int minHeight;
    public int maxHeight;
    public float frequency;
    public float lacunarity;
    public int octaves;
    public float persistence;
}


public partial struct TestSystem : ISystem {

    public static ChunkMap test;

    public void OnCreate(ref SystemState state) {
        
    }

    public void OnDestroy(ref SystemState state) {
        
    }

    public void OnUpdate(ref SystemState state) {
        state.Dependency = new TestJob{}.ScheduleParallel(state.Dependency);
    }

    partial struct TestJob : IJobEntity {
        public void Execute(in Entity entity, in TestComponent cmp) {
            int chunkMapSize = cmp.chunkMapSize;
            int chunkSize = cmp.chunkSize;
            test = new ChunkMap(
                chunkMapSize,
                chunkSize,
                cmp.chunkGenerationOrigin,
                cmp.minHeight, cmp.maxHeight,
                cmp.frequency,
                cmp.lacunarity,
                cmp.octaves,
                cmp.persistence);
            
            //Generate the chunks based on chunk load distance
        }
    }


    public static int[,] FractalNoiseInt(int chunkX, int chunkY, int gridWidth, int gridHeight, int minHeight, int maxHeight, float frequency, int octaves, float lacunarity, float persistence) {

		int[,] grid = new int[gridHeight, gridWidth];
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
				grid[y, x] = (int)cellElevation;
			}
		}

		return grid;
	}
}