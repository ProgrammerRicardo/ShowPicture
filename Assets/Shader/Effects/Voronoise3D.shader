Shader "CustomEffect/Voronoise3D"
{
	Properties
	{
		_Tint("Tint Color", Color) = (1.0, 1.0, 1.0, 1.0)
	}

	CGINCLUDE

	#include "UnityCG.cginc"   
	#pragma target 3.0      

	#define iTime _Time.y

	#define PI 3.1415926
	#define NUM_OCTAVES 2

	uniform float4 _Tint;

	struct appdata
	{
		float4 vertex : POSITION;
		float4 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		float4 texcoord : TEXCOORD0;
	};

	float4 main(float2 fragCoord);

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.texcoord = v.texcoord;
		return o;
	}

	fixed4 frag(v2f _iParam) : SV_Target
	{
		float3 p = normalize(_iParam.texcoord.xzy);
		float theta = acos(0.5 * p.z);
		float phi = atan2(p.y, p.x);
		theta /= PI;
		phi = (phi + PI) / (2 * PI);

		return main(float2(phi, theta));
	}

	//-------------------------------------------------------------------------------------------------------

	// 1D random numbers
	float rand(float n)
	{
		return frac(sin(n) * 43758.5453123);
	}

	// 2D random numbers
	float2 rand2(in float2 p)
	{
		return frac(float2(sin(p.x * 591.32 + p.y * 154.077), cos(p.x * 391.32 + p.y * 49.077)));
	}

	// 1D noise
	float noise1(float p)
	{
		float fl = floor(p);
		float fc = frac(p);
		return lerp(rand(fl), rand(fl + 1.0), fc);
	}

	//voronoi distance noise, based on iq's articles: http://www.iquilezles.org/www/articles/voronoilines/voronoilines.htm
	float voronoiDistance(in float2 x)
	{
		float2 p = floor(x);
		float2 f = frac(x);

		float2 res = 8.0;
		for (int j = -1; j <= 1; j++)
			for (int i = -1; i <= 1; i++)
			{
				float2 b = float2(i, j);
				float2 r = float2(b)-f + rand2(p + b) * 0.9;

				float d = max(abs(r.x), abs(r.y));

				if (d < res.x)
				{
					res.y = res.x;
					res.x = d;
				}
				else if (d < res.y)
				{
					res.y = d;
				}
			}

		return res.y - res.x;
	}

	float4 main(float2 fragCoord)
	{
		float2 uv = 2.0 * fragCoord.xy - 1.0;

		float v = 0.0;

		// add some noise octaves
		float a = 0.1, f = 4.0;

		for (int i = 0; i < NUM_OCTAVES; i++)
		{
			float v1 = voronoiDistance(uv * f + 5.0);
			float v2 = voronoiDistance(uv * f + iTime + 50.0);
			float va = 1.0 - smoothstep(0.0, 0.2, v1);
			float vb = 1.0 - smoothstep(0.0, 0.2, v2);
			v1 = 1.0 - smoothstep(0.0, 0.2, v1);
			v2 = a * (noise1(v1 * 5.5 + 0.1));
			v += a * pow(va * (0.5 + vb), 1.0) * 0.5 + v2;

			f *= 3.0;
			a *= 2.0;
		}

		// slight vignetting
		v *= exp(-0.6 * length(uv)) * 0.8;

		fixed3 cexp = float3(8.0, 2.5, 2.0);
		cexp *= 1.0;

		float3 col = float3(pow(v, cexp.x), pow(v, cexp.y), pow(v, cexp.z)) * 2.0;

		float4 fragColor = float4(col, 1.0);
		fragColor *= _Tint;
		fragColor = clamp(fragColor, 0.0, 1.0);

		return fragColor;
	}

	//-------------------------------------------------------------------------------------------------------

	ENDCG

	SubShader
	{
		Pass
		{
			CGPROGRAM

			#pragma vertex vert    
			#pragma fragment frag

			ENDCG
		}
	}

	FallBack Off
}