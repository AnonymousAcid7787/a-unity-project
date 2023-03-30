using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteUtils
{
    public static List<Texture2D> GetSlicedSpriteTextures(Sprite sprite)
    {
        List<Texture2D> textures = new List<Texture2D>();
        Sprite[] sprites = Resources.LoadAll<Sprite>(sprite.texture.name);
        
        for(var i=0; i<sprites.Length; i++) {
            Sprite s = sprites[i];
            Rect sprRect = s.rect;
            Texture2D slicedTex = new Texture2D((int)sprRect.width, (int)sprRect.height);
            slicedTex.filterMode = sprite.texture.filterMode;
            
            Color[] colors = sprite.texture.GetPixels((int)sprRect.x, (int)sprRect.y, (int)sprRect.width, (int)sprRect.height);

            slicedTex.SetPixels(
                0, 0,
                (int)sprRect.width, (int)sprRect.height,
                colors
                );
            
            slicedTex.Apply();   
            slicedTex.name = s.name;    
            textures.Add(slicedTex);
        }
 
        return textures;
    }

}
