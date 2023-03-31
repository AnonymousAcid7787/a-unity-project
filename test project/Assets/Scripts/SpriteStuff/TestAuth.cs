using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Collections;

public class TestAuth : MonoBehaviour
{
    public Material baseMaterial;
    public Sprite spriteSheet;
}

public class TestBaker : Baker<TestAuth>
{
    public override void Bake(TestAuth authoring)
    {
        NativeList<Entity> spriteEntities = new NativeList<Entity>(Allocator.Persistent);
        List<Texture2D> textures = SpriteUtils.GetSlicedSpriteTextures(authoring.spriteSheet);/*get slices*/
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entity thisEntity = GetEntityWithoutDependency();

        for(var i=0; i<textures.Count; i++) {
            Texture2D tex = textures[i];
            Material material = new Material(authoring.baseMaterial);/*new material with slice's texture*/

            Entity entity = entityManager.CreateEntity(typeof(SpriteComponent));/*sprite entity*/

            Matrix4x4 worldMatrix = Matrix4x4.TRS(/*transform of sprite (will update every frame)*/
                authoring.transform.position,
                authoring.transform.rotation,
                Vector3.one
            );

            InstanceData instanceData = new InstanceData{
                worldMatrix = worldMatrix,
                worldMatrixInverse = Matrix4x4.Inverse(worldMatrix)
            };

            int materialCacheIndex = RenderCache.CacheInfo(material, RenderInfo.NewQuadMesh(), instanceData);

            entityManager.SetComponentData(entity, new SpriteComponent {
                materialCacheIndex = materialCacheIndex,
                instanceData = instanceData,
                parentEntity = thisEntity
            });
            spriteEntities.Add(entity);
        }

        AddComponent(new SpriteStack {
            spriteEntities = spriteEntities
        });
    }
}