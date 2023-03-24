using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingTest : MonoBehaviour
{
    public Sprite sprite;
    public Material material;
    public Mesh mesh;
    
    private RenderInformation renderInfo;

    // Start is called before the first frame update
    void Start()
    {
        renderInfo = new RenderInformation(material, mesh, new Bounds(Vector3.zero, new Vector3(10,10,10)));
        Vector3 offset = new Vector3(
            UnityEngine.Random.Range(-1, 1),
            UnityEngine.Random.Range(-1, 1),
            0
        );
        renderInfo.AddTransform(Matrix4x4.TRS(offset, Quaternion.identity, Vector3.one));
    }

    // Update is called once per frame
    void Update()
    {
        renderInfo.Draw();
    }
}
