using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingTest : MonoBehaviour
{
    public Sprite sprite;

    private Material material;
    private List<Matrix4x4> matrices;
    private ComputeBuffer argsBuf;
    private Mesh mesh;
    private Bounds bounds;

    // Start is called before the first frame update
    void Start()
    {
        bounds = new Bounds(Vector3.zero, new Vector3(10, 10, 10));
        mesh = RenderInfo.QuadMesh();
        matrices = new List<Matrix4x4>();

        matrices.Add(Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale));
        ComputeBuffer matricesBuffer = new ComputeBuffer(1, sizeof(float) * 4*4);
        matricesBuffer.SetData(matrices);
        material = new Material(Shader.Find("Unlit/Transparent"));
        material.SetBuffer("matricesBuffer", matricesBuffer);
        material.mainTexture = sprite.texture;

        argsBuf = new ComputeBuffer(1, 5*sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuf.SetData(
            new uint[] {
                mesh.GetIndexCount(0),
                1,
                mesh.GetIndexStart(0),
                mesh.GetBaseVertex(0),
                0,
            }
        );
    }

    // Update is called once per frame
    void Update()
    {
        Graphics.DrawMeshInstancedIndirect(
            mesh, 0,
            material,
            bounds,
            argsBuf
        );
    }
}
