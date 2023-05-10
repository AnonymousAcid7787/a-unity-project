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
        int[,] grid = new int[21, 21];
        string gridStr = TerrainGenUtils.DiamondSquare2(21, 2, 1, 8, SystemAPI.GetSingletonRW<RandomComponent>()).ToString();
        Debug.Log("Done");
    }
}