Shader "Unlit/DepthMask" {

    SubShader {
        // Render the mask after regular geometry, but before masked geometry and
        // transparent things.
        Tags {"RenderType" = "Opaque" "Queue" = "Geometry-10" }
 
        // Don't draw in the RGBA channels; just the depth buffer
        Cull Off
        ColorMask 0
 
        // Do nothing specific in the pass:
        Pass {
            ZWrite Off
        }
    }
}