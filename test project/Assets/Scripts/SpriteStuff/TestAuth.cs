using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

public class TestAuth : MonoBehaviour
{
    public Mesh mesh;
    public Material material;
    public Sprite sprite;

    private EntityManager entityManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }
}


public class TestBaker : Baker<TestAuth>
{
    public override void Bake(TestAuth authoring)
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entity entity = entityManager.CreateEntity(
            typeof(RenderMesh),
            typeof(RenderBounds),
            typeof(LocalToWorld)
        );

        entityManager.SetComponentData(entity, new LocalToWorld {
            Value = Matrix4x4.TRS(authoring.transform.position, authoring.transform.rotation, Vector3.one)
        });

        entityManager.SetSharedComponentManaged(entity, new RenderMesh {
            mesh = authoring.mesh,
            material = authoring.material
        });
        
        entityManager.SetComponentData(entity, new RenderBounds {
            Value = new Unity.Mathematics.AABB {
                Center = Vector3.zero,
                Extents = new Vector3(10,10,10)
            }
        });
    }
}