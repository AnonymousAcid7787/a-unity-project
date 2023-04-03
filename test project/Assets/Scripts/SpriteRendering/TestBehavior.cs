using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class TestBehavior : MonoBehaviour
{
    public Material spriteMaterial;
    public Mesh mesh;
}

public class TestBaker : Baker<TestBehavior>
{
    public override void Bake(TestBehavior authoring)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(authoring.spriteMaterial.mainTexture.name);

        SpriteSheetDrawInfo drawInfo = SpriteSheetCache.CacheSpriteSheet(new RenderArgs(
            authoring.spriteMaterial, authoring.mesh,
            new Bounds(Vector3.zero, new Vector3(10, 10, 10)),
            new MaterialPropertyBlock()
        ));

        int instanceDataHash = drawInfo.AddInstance(new InstanceData {
            worldMatrix = Matrix4x4.zero,
            worldMatrixInverse = Matrix4x4.zero,
            uvOffset = Vector2.zero,
            uvTiling = Vector2.one
        });

        Entity entity = CreateAdditionalEntity(entityName: authoring.name + "_sprite");
        AddComponent(entity, new SpriteSheetAnimationData {
            drawInfoHashCode = drawInfo.GetHashCode(),
            instanceDataHash = instanceDataHash,
            currentFrame = 0,
            frameCount = sprites.Length,
            frameTimer = 0f,
            frameTimerMax = .1f
        });
    }
}