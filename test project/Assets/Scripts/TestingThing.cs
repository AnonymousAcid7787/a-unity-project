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
    protected override void OnUpdate()
    {
        if(foundSingleton)
            return;
        if(!SystemAPI.HasSingleton<RandomComponent>())
            return;
        
        foundSingleton = true;
        int[,] grid = TerrainGenUtils.DiamondSquare(21, 2, 1, 8, SystemAPI.GetSingletonRW<RandomComponent>());
        string str = "";
        for(var y = 0; y < 21; y++) {
            for(var x = 0; x < 21; x++) {
                str += grid[y, x] + ",";
            }
            str += "\n";
        }
        GUIUtility.systemCopyBuffer = str;
        Debug.Log("Done");
    }
}