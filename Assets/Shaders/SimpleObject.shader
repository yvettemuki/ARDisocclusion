Shader "Unlit/SimpleObject"
{
    Properties
    {
        _Color("Color", Color) = (0.909, 0.478, 0.564, 1.0)
        _MainTex("Texture", 2D) = "white" {}
        _CompFunc("StencilCompFunc", int) = 0  // 0:Always, 3:Equal
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Stencil
        {
            Ref 1
            Comp [_CompFunc]
            Pass Keep
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed4 col = tex * _Color.rgba;
                return col;
            }
            ENDCG
        }
    }
}
