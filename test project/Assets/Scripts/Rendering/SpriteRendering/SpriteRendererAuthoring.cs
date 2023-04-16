using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class SpriteRendererAuthoring : MonoBehaviour
{
    public Material spriteMaterial;
    public Mesh mesh;
    public TransformUsageFlags transformUsageFlags;
}

public class SpriteRendererBaking : Baker<SpriteRendererAuthoring>
{
    public override void Bake(SpriteRendererAuthoring authoring)
    {
        Entity thisEntity = GetEntity(authoring.transformUsageFlags);

        KeyValuePair<Texture, Sprite[]> pair = SpriteSheetCache.CacheSpriteSheet(authoring.spriteMaterial.mainTexture);

        SpriteSheetDrawInfo drawInfo = SpriteSheetCache.CacheSpriteSheetDrawInfo(new RenderArgs(
            authoring.spriteMaterial, authoring.mesh,
            new Bounds(Vector3.zero, new Vector3(10, 10, 10)),
            new MaterialPropertyBlock()
        ));
        int hashCode = drawInfo.GetHashCode();

        int instanceKey = thisEntity.GetHashCode();
            drawInfo.instances.Add(instanceKey, new InstanceData{});

        // Vector3 pos = new Vector3(
        //     Random.Range(-1f, 1f),
        //     Random.Range(-1f, 1f),
        //     0
        // );
        Matrix4x4 matrix = Matrix4x4.TRS(
            authoring.transform.position,
            Quaternion.identity,
            Vector3.one
        );

        Rect[] uvRectsArray = SpriteUtils.GetSpriteSheetUVs(pair.Key, pair.Value);
        drawInfo.uvRects = uvRectsArray;

        AddComponent(thisEntity, new SpriteSheetAnimationData {
            drawInfoHashCode = hashCode,
            currentFrame = 0,
            frameCount = pair.Value.Length,
            frameTimer = 0f,
            frameTimerMax = .5f,
            instanceData = new InstanceData {
                worldMatrix = matrix,
                worldMatrixInverse = Matrix4x4.Inverse(matrix),
                uvTiling = Vector2.one,
                uvOffset = Vector2.zero
            },
            instanceKey = instanceKey,
        });
        
    }
}