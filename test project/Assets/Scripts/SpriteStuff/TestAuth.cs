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

    void Start() {
        material = new Material(material);
        material.mainTexture = sprite.texture;

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entity entity = entityManager.CreateEntity(
            typeof(RenderMesh),
            typeof(RenderBounds),
            typeof(LocalTransform)
        );

        entityManager.SetComponentData(entity, new LocalTransform {
            _Position = transform.position,
            _Scale = 1,
            _Rotation = transform.rotation
        });


        entityManager.SetSharedComponentManaged(entity, new RenderMesh {
            mesh = mesh,
            material = material
        });
        
        entityManager.SetComponentData(entity, new RenderBounds {
            Value = new Unity.Mathematics.AABB {
                Center = Vector3.zero,
                Extents = new Vector3(10,10,10)
            }
        });
    }
}