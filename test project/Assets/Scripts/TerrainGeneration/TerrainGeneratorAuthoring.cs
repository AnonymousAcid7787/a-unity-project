using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class TerrainGeneratorAuthoring : MonoBehaviour
{
    public int meshDivisions;
    public float meshSize;
    public float meshHeight;
}

public class TerrainGeneratorBaker : Baker<TerrainGeneratorAuthoring>
{
    public override void Bake(TerrainGeneratorAuthoring authoring)
    {
        int mDivisions = authoring.meshDivisions;
        float mSize = authoring.meshSize;
        float mHeight = authoring.meshHeight;
        #region error checking
        if(mDivisions < 2)
            throw new System.InvalidOperationException("Grid is too small!");

        if(Mathf.Log((float)mDivisions-1, 2)%1 != 0)
            throw new System.InvalidOperationException("Grid size is is not \"(n^2)+1!\"");
        #endregion error checking

        int vertexCount = (mDivisions+1)*(mDivisions+1);
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[mDivisions*mDivisions*6];
        
        float halfSize = mSize/2f;
        float divisionSize = mSize/mDivisions;

        Mesh mesh = new Mesh();
        authoring.gameObject.GetComponent<MeshFilter>().mesh = mesh;

        for(int y = 0; y <= mDivisions; y++) {
            for(int x = 0; x <= mDivisions; x++) {
                vertices[x+(mDivisions+1)+y] = new Vector3(-halfSize+y*divisionSize, 0, halfSize-x*divisionSize);
            }
        }
    }
}