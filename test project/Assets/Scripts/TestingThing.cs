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
    protected override void OnUpdate()
    {
        if(!SystemAPI.HasSingleton<RandomComponent>())
            return;
        int[,] heightValues = new int[(21) , (21)];
        TerrainGenUtils.DiamondSquare(ref heightValues, 10, 4, SystemAPI.GetSingletonRW<RandomComponent>());
        GUIUtility.systemCopyBuffer = heightValues.ToString();
    }
}