using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class RenderAuth3D : MonoBehaviour
{
    public Material material;
    public Mesh mesh;
    public UnityEngine.Rendering.ShadowCastingMode shadowCastingMode;
    public bool recieveShadows;
}

public class Render3DBaker : Baker<RenderAuth3D>
{
    public override void Bake(RenderAuth3D authoring)
    {
        DrawInfo3D drawInfo = RenderCache3D.Cache3DDrawInfo(new RenderArgs(
            authoring.material, authoring.mesh,
            new Bounds(Vector3.zero, new Vector3(10,10,10)),
            new MaterialPropertyBlock(),
            authoring.shadowCastingMode,
            authoring.recieveShadows
        ));

        #region component info
        int drawInfoHashCode = drawInfo.GetHashCode();

        int instanceKey = GetEntity().GetHashCode();
            drawInfo.instances.Add(instanceKey, new InstanceData{});

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
        #endregion component info

        AddComponent(new RenderData3D{
            drawInfoHashCode = drawInfoHashCode,
            instanceData = new InstanceData{
                worldMatrix = matrix,
                worldMatrixInverse = Matrix4x4.Inverse(matrix)
                /*no need to add uv tiling and stuff, because the 3d shader graphs won't be using such data*/
            },     
            instanceKey = instanceKey
        });
    }
}
