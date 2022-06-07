Shader "Unlit/SimpleObject"
{
    Properties
    {
        _Color("Color", Color) = (0.909, 0.478, 0.564, 1.0)
        _EstimateLightColor("Main Light Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _MainTex("Texture", 2D) = "white" {}
        _CompFunc("StencilCompFunc", int) = 0  // 0:Always, 3:Equal
        _LightPoint("Light Point Position", Vector) = (-2, 4, -2, 0)
        _EstimateLightDir("Main Light Direction", Vector) = (0, 0, 0, 0)
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
                float4 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPosition : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _EstimateLightColor;
            float4 _LightPoint;
            float4 _EstimateLightDir;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPosition = mul(unity_ObjectToWorld, v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 tex = tex2D(_MainTex, i.uv);
                // calcuate ambient lighting
                fixed4 ambient = _Color.rgba * tex * fixed4(0.6, 0.6, 0.6, 1.0);
                // calculate diffuse lighting
                //fixed3 lightDirection = normalize(_LightPoint.xyz - i.worldPosition);
                fixed3 lightDirection = normalize(_EstimateLightDir.xyz);
                fixed diffuse = max(dot(lightDirection, normalize(i.worldNormal)), 0.0) * _EstimateLightColor * _Color.rgba * tex; //fixed4(0.5, 0.5, 0.5, 1.0)
                fixed4 col = ambient + diffuse;
                return col;
            }
            ENDCG
        }
    }
}
