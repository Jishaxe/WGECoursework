// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader modified from this post https://forum.unity.com/threads/invert-colors-shader.205244/

Shader "UI/Desaturate" {
	Properties
	{
		_EffectAmount("Effect Amount", Range(0, 1)) = 1.0
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" }
		GrabPass { "_GrabTexture" }
		Pass
		{
		   ZWrite Off
		   ColorMask 0
		}

		Pass
		{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform float4 _Color;
			sampler2D _GrabTexture;
			struct vertexInput
			{
				float4 vertex: POSITION;
				float4 color : COLOR;

			};

			struct fragmentInput
			{
				float4 pos : SV_POSITION;
				float4 color : COLOR0;
				float4 uvgrab : TEXCOORD1;
			};

			fragmentInput vert(vertexInput i)
			{
				fragmentInput o;
				o.pos = UnityObjectToClipPos(i.vertex);
				o.uvgrab = ComputeGrabScreenPos(o.pos);
				return o;
			}

			uniform float _EffectAmount;
			half4 frag(fragmentInput i) : COLOR
			{
				fixed4 col = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
				col.rgb = lerp(col.rgb, dot(col.rgb, float3(0.3, 0.59, 0.11)), _EffectAmount);
				return col;
			}

			ENDCG
			}
	}
}
