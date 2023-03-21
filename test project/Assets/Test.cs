using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

public class Test : MonoBehaviour
{
    public GameObject emptyPrefab;
    public Material baseMaterial;
}

public class TestBaker : Baker<Test>
{
    public override void Bake(Test authoring)
    {
        RenderComponent component = new RenderComponent{
            entity = GetEntity(authoring.emptyPrefab),
            renderInfoIndex = 0,
        };
        EntityManager eManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        eManager.SetSharedComponentManaged(component.entity, new RenderMesh {
            mesh = RenderInfo.QuadMesh(),
            material = authoring.baseMaterial,
        });
        AddComponent(component);
    }
}