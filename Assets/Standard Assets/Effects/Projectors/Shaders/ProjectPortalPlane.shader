Shader "Projector/ProjectPortalPlane"
{
	Properties
	{
		_Color("Color", Color) = (1.0, 1.0, 1.0, 0.0)
		_F0Tex("Human", 2D) = "" {}
	}
		SubShader
	{
		Tags { "Queue" = "Transparent" }

		Pass {
			ZWrite Off
			ColorMask RGBA
			Blend SrcAlpha OneMinusSrcAlpha
		//Offset -1, -1

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"

		struct v2f {
			float4 uvF0 : TEXCOORD0;
			float4 pos : SV_POSITION;
		};

		float4x4 unity_Projector;
		float4x4 unity_ProjectorClip;

		v2f vert(float4 vertex : POSITION)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(vertex);
			o.uvF0 = mul(unity_Projector, vertex);
			return o;
		}

		fixed4 _Color;
		sampler2D _F0Tex;

		fixed4 frag(v2f i) : COLOR
		{
			fixed4 texS = tex2Dproj(_F0Tex, UNITY_PROJ_COORD(i.uvF0));
			texS.rgb *= _Color.rgb;

			return texS;

			/*fixed4 texF = tex2Dproj (_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));
			fixed4 res = texS * texF.a;

			UNITY_APPLY_FOG_COLOR(i.fogCoord, res, fixed4(0,0,0,0));
			return res;*/
		}
		ENDCG
	}
	}

}
