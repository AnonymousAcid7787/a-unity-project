using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawInfo : MonoBehaviour
{
    public Material material;
    public Mesh mesh;

    public ComputeBuffer argsBuffer;
    public ComputeBuffer instancesBuffer;
    public List<InstanceData> instanceDatas;
    public Bounds renderBounds;
    public MaterialPropertyBlock materialPropertyBlock;
    public UnityEngine.Rendering.ShadowCastingMode shadowCastingMode;
    public bool recieveShadows;
}
