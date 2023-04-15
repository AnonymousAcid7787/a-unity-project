using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Struct that can be passed in as a parameter for DrawInfo constructors/draw info caching.
public struct RenderArgs {
    public Material material;
    public Mesh mesh;
    public Bounds renderBounds;
    public MaterialPropertyBlock materialPropertyBlock;
    public UnityEngine.Rendering.ShadowCastingMode shadowCastingMode;
    public bool recieveShadows;

    public RenderArgs(Material material,Mesh mesh,
                      Bounds renderBounds,
                      MaterialPropertyBlock materialPropertyBlock,
                      UnityEngine.Rendering.ShadowCastingMode shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                      bool recieveShadows = true
                     )
    {
        this.material = material;
        this.mesh = mesh;
        this.renderBounds = renderBounds;
        this.materialPropertyBlock = materialPropertyBlock;
        this.shadowCastingMode = shadowCastingMode;
        this.recieveShadows = recieveShadows;
    }
}