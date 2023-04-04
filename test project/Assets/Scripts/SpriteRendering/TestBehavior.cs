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
        Vector3 pos = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            0
        );

        Matrix4x4 matrix = Matrix4x4.TRS(
            pos,
            Quaternion.identity,
            Vector3.one
        );

        int instanceDataIndex = drawInfo.AddInstance(new InstanceData {
            worldMatrix = matrix,
            worldMatrixInverse = Matrix4x4.Inverse(matrix),
            uvOffset = Vector2.one*UnityEngine.Random.Range(0f, 1f),
            uvTiling = Vector2.one
        });

        Entity entity = CreateAdditionalEntity(entityName: authoring.name + "_sprite");
        AddComponent(entity, new SpriteSheetAnimationData {
            drawInfoHashCode = drawInfo.GetHashCode(),
            instanceDataIndex = instanceDataIndex,
            currentFrame = 0,
            frameCount = sprites.Length,
            frameTimer = 0f,
            frameTimerMax = .5f
        });
    }
}