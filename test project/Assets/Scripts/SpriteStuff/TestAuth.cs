using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

public class TestAuth : MonoBehaviour
{
    public Material baseMaterial;
    public Sprite spriteSheet;
}

public class TestBaker : Baker<TestAuth>
{
    public override void Bake(TestAuth authoring)
    {
        List<Texture2D> textures = SpriteUtils.GetSlicedSpriteTextures(authoring.spriteSheet);/*get slices*/
        
        SpriteStack spriteStack = new SpriteStack {
            spriteEntities = new NativeList<Entity>(Allocator.Persistent),
            updatedParentEntity = false
        };

        for(var i=0; i<textures.Count; i++) {
            Texture2D tex = textures[i];
            Material material = new Material(authoring.baseMaterial);/*new material with slice's texture*/

            InstanceData instanceData = new InstanceData {
                worldMatrix = Matrix4x4.zero,
                worldMatrixInverse = Matrix4x4.zero
            };

            int materialCacheIndex = RenderCache.CacheInfo(material, RenderInfo.NewQuadMesh(), instanceData);

            Entity entity = CreateAdditionalEntity( /*sprite entity*/
                entityName: authoring.gameObject.name+"_sprite"+i.ToString()
            );
            AddComponent(entity, new SpriteComponent {
                renderCacheIndex = materialCacheIndex,
                instanceData = instanceData,
                positionOffset = new float3(0, 0.05f*i, 0),
                scale = new float3(1,1,1),
            });
            spriteStack.spriteEntities.Add(entity);
        }

        AddComponent(spriteStack);
    }
}



// [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
// public partial class TestBakingSystem : SystemBase
// {
//     protected override void OnUpdate()
//     {
//         Entities.ForEach((SpriteStackAspect spriteStackAspect) => {
//             if(!spriteStackAspect.spriteStack.ValueRW.updatedParentEntity) {
//                 for(var i=0; i<spriteStackAspect.spriteStack.ValueRW.spriteEntities.Length; i++) {
//                     SpriteComponent spriteComponent = EntityManager.GetComponentData<SpriteComponent>(spriteStackAspect.spriteStack.ValueRW.spriteEntities[i]);
//                     spriteComponent.parentEntity = spriteStackAspect.entity;
//                     EntityManager.SetComponentData(spriteStackAspect.spriteStack.ValueRW.spriteEntities[i], spriteComponent);
//                 }

//                 spriteStackAspect.spriteStack.ValueRW.updatedParentEntity = true;
//             }

//         }).WithoutBurst().Run();
//     }
// }