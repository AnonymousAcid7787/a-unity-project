using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSheetCache
{
    public static Dictionary<int, SpriteSheetDrawInfo> cache = new Dictionary<int, SpriteSheetDrawInfo>();
    public static Dictionary<Texture, Sprite[]> cachedSpriteSheets = new Dictionary<Texture, Sprite[]>();

    public static SpriteSheetDrawInfo CacheSpriteSheet(RenderArgs args) {
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
}
