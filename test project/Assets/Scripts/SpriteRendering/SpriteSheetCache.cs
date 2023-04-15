using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSheetCache
{
    public static Dictionary<int, SpriteSheetDrawInfo> cache = new Dictionary<int, SpriteSheetDrawInfo>();
    public static Dictionary<Texture, Sprite[]> cachedSpriteSheets = new Dictionary<Texture, Sprite[]>();

    public static SpriteSheetDrawInfo CacheSpriteSheetDrawInfo(RenderArgs args) {
        foreach(KeyValuePair<int, SpriteSheetDrawInfo> cachedInfo in cache) { //if material with same sprite sheet exists, return its hash code.
            if(cachedInfo.Value.spriteSheetMaterial.mainTexture == args.material.mainTexture) {
                return cachedInfo.Value;
            }
        }
        
        if(!cachedSpriteSheets.ContainsKey(args.material.mainTexture))
            cachedSpriteSheets.Add(args.material.mainTexture, Resources.LoadAll<Sprite>(args.material.mainTexture.name));

        SpriteSheetDrawInfo drawInfo = new SpriteSheetDrawInfo(args);
        cache.Add(drawInfo.GetHashCode(), drawInfo);
        return drawInfo;
    }

    public static KeyValuePair<Texture, Sprite[]> CacheSpriteSheet(Sprite spriteSheet) {
        foreach(KeyValuePair<Texture, Sprite[]> cachedSpriteSheet in cachedSpriteSheets) {
            if(cachedSpriteSheet.Key == spriteSheet.texture)
                return cachedSpriteSheet;
        }

        Sprite[] sprites = Resources.LoadAll<Sprite>(spriteSheet.texture.name); /*get all slices*/
        cachedSpriteSheets.Add(spriteSheet.texture, sprites);
        Rect r = sprites[0].rect;
        return new KeyValuePair<Texture, Sprite[]>(spriteSheet.texture, sprites);
    }

    public static KeyValuePair<Texture, Sprite[]> CacheSpriteSheet(Texture texture) {
        foreach(KeyValuePair<Texture, Sprite[]> cachedSpriteSheet in cachedSpriteSheets) {
            if(cachedSpriteSheet.Key == texture)
                return cachedSpriteSheet;
        }

        Sprite[] sprites = Resources.LoadAll<Sprite>(texture.name); /*get all slices*/
        cachedSpriteSheets.Add(texture, sprites);
        Rect r = sprites[0].rect;
        return new KeyValuePair<Texture, Sprite[]>(texture, sprites);
    }


    public static void ClearCache() {
        foreach(KeyValuePair<int, SpriteSheetDrawInfo> cachedInfo in cache) {
            cachedInfo.Value.DestroyBuffers();
        }
        cache.Clear();
    }
}
