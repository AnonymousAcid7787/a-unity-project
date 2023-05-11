using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class TestingThing : MonoBehaviour
{
    public Mesh mesh;
}

public class TestBaker : Baker<TestingThing>
{
    public override void Bake(TestingThing authoring)
    {
        authoring.gameObject.GetComponent<MeshFilter>().mesh =
            authoring.mesh;
    }
}

public partial class TestSystem : SystemBase
{
    private static bool foundSingleton;
    protected override void OnCreate() {
        foundSingleton = false;
    }

    protected override void OnUpdate()
    {
        return;
        if(foundSingleton)
            return;
        if(!SystemAPI.HasSingleton<RandomComponent>())
            return;
        
        foundSingleton = true;
        uint seed = SystemAPI.GetSingletonRW<RandomComponent>().ValueRW.random.state;
        int[,] grid = TerrainGenUtils.DiamondSquare(129, 2, 1, 28, new Unity.Mathematics.Random(seed));
        string str = "";
        for(var y = 0; y < 129; y++) {
            for(var x = 0; x < 129; x++) {
                str += grid[y, x] + ",";
            }
            str += "\n";
        }
        GUIUtility.systemCopyBuffer = str;
        Debug.Log("Done");
    }
}