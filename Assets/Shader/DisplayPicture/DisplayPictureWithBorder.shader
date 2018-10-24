Shader "Custom/DisplayPictureWithBorder" 
{
    Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
        _Color ("Border Color", Color) = (0.714, 1.0, 0.737, 1.0)
        _Range ("Border Range", Range(0.0, 0.3)) = 0.15
    }

    SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
        
        Pass 
		{
			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform float _Range;
            uniform float4 _Color;

            struct appdata_t 
			{
                float4 vertex : POSITION;
            };

            struct v2f 
			{
                float4 vertex : POSITION;
                half4 color : COLOR;
            };

            v2f vert (appdata_t v) 
			{
                v2f o;
                v.vertex.xyz += _Range * v.vertex.xyz;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = _Color;
                return o;
            }

            half4 frag (v2f IN) : COLOR 
			{
                return IN.color;
            } 
            ENDCG
        }
        
        Pass 
		{
            Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
            Blend One Zero
            Offset -1,-1
            ColorMaterial AmbientAndDiffuse
            
            SetTexture [_MainTex] 
			{
                Combine Texture
            }
        }
    }
}