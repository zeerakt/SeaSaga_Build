// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Sprites/Default_Fade"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		[PerRendererData]_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[PerRendererData][MaterialToggle]_FadeEnable("Fade Enable", float) = 0 // can't use bool property
		_FadeBlipSpeed("Fade Blip Speed", Range(0,02)) = 0
		
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			//"RenderType" = "Opaque"
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		// Blend BlendOp
		// Blend SrcAlpha OneMinusSrcAlpha //https://docs.unity3d.com/Manual/SL-Blend.html
		//	Blend One One //http://docs.unity3d.com/412/Documentation/Components/SL-Blend.html

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
		//	#pragma multi_compile _ PIXELSNAP_ON
		//	#pragma shader_feature ETC1_EXTERNAL_ALPHA
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;
			float _FadeBlipSpeed;
			float _FadeEnable;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
			//	OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _GlowTex1;
			sampler2D _GlowTex2;
			sampler2D _GlowTex3;
			sampler2D _AlphaTex;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if ETC1_EXTERNAL_ALPHA
				// get the color from an external texture (usecase: Alpha support for ETC1 on android)
				color.a = tex2D (_AlphaTex, uv).r;
#endif //ETC1_EXTERNAL_ALPHA

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture (IN.texcoord) ;
				c.rgb *= c.a;
				if (_FadeEnable) {
					c.a *= (-cos(_Time.w * 5 * _FadeBlipSpeed) + 1.0)*0.5;
					c.rgb *= c.a;
				}
				return c;
			}
		ENDCG
		}
	}
}
