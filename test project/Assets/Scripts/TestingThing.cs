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