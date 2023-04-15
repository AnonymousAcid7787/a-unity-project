using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteUtils
{
    private static Mesh spriteMeshVar = null;

    public static Mesh spriteMesh {
        get {
            if(spriteMeshVar == default || spriteMeshVar == null)
                spriteMeshVar = NewQuadMesh();
            return spriteMeshVar;
        }
    }

    public static Mesh NewQuadMesh() {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(1, 0, 0);
        vertices[2] = new Vector3(0, 1, 0);
        vertices[3] = new Vector3(1, 1, 0);
        mesh.vertices = vertices;

        int[] tri = new int[6];
        tri[0] = 0;
        tri[1] = 2;
        tri[2] = 1;
        tri[3] = 2;
        tri[4] = 3;
        tri[5] = 1;
        mesh.triangles = tri;

        Vector3[] normals = new Vector3[4];
        normals[0] = -Vector3.forward;
        normals[1] = -Vector3.forward;
        normals[2] = -Vector3.forward;
        normals[3] = -Vector3.forward;
        mesh.normals = normals;

        Vector2[] uv = new Vector2[4];
        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);
        mesh.uv = uv;

        return mesh;
    }

    public static Rect[] GetSpriteSheetUVs(Texture spriteSheetTexture, Sprite[] sprites) {
        Rect[] uvRects = new Rect[sprites.Length];

        for(var i=0; i<sprites.Length; i++) {
            Rect r = sprites[i].rect;

            //calculate rectangles into UV dimensions (in decimal percents like 0.20 for 20%)
            Rect uvRect = new Rect(
                r.x/spriteSheetTexture.width,
                r.y/spriteSheetTexture.height,
                r.width/spriteSheetTexture.width,
                r.height/spriteSheetTexture.height
            );
            uvRects[i] = uvRect;
        }

        return uvRects;
    }
}
