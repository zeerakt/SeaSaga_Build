// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Transparent Mask" {
	Properties
	{
		_MainTex("Base (RGB), Alpha (A)", 2D) = "black" {}
	}

		SubShader{
		Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
	}
		Pass{
		Stencil{
		Ref 2
		Comp always
		Pass replace
		Fail keep
		ZFail decrWrap
	}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog{ Mode Off }
		Offset -1, -1
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
		sampler2D _MainTex;
		float4 _MainTex_ST;

	struct appdata {
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
		fixed4 color : COLOR;
	};
	struct v2f {
		float4 vertex : SV_POSITION;
		half2 texcoord : TEXCOORD0;
		fixed4 color : COLOR;
	};

	v2f o;

	v2f vert(appdata v)
	{
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.texcoord = v.texcoord;
		o.color = v.color;
		return o;
	}

	fixed4 frag(v2f IN) : SV_Target
	{
	//	fixed4 color = tex2D(_MainTex, IN.texcoord);
	//color.rgb *= IN.color.rgb;

//	if (color.a > 0.0) {
//		color.a = 0;
//	}
//	else {
//		color.a = IN.color.a;
//	}

	return float4(0, 0, 0, 0);
	}
		ENDCG
	}
	}
}
