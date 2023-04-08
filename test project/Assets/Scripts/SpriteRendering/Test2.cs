using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : MonoBehaviour
{
    public Material material;
    public Mesh mesh;

    private SpriteSheetDrawInfo drawInfo;
    // Start is called before the first frame update
    void Start()
    {
        drawInfo = new SpriteSheetDrawInfo(
            material, mesh ,
            new Bounds(Vector3.zero, new Vector3(10,10,10))
        );
        Matrix4x4 matrix = Matrix4x4.TRS(
            transform.position,transform.rotation,Vector3.one
        );

        // drawInfo.AddInstance(new InstanceData {
        //     worldMatrix = matrix,
        //     worldMatrixInverse = Matrix4x4.Inverse(matrix)
        // });
    }

    // Update is called once per frame
    void Update()
    {
        drawInfo.Draw();
    }
}
