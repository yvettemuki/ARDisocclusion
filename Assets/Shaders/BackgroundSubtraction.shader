Shader "Projector/BackgroundSubtraction"
{
    Properties
    {
        _MainTex ("HumanSprite", 2D) = "" {}  // it will be set by the script code in unity
        _SubTex("Background", 2D) = "" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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
                float4 screenPos: TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.screenPos = ComputeScreenPos(v.vertex);
                return o;
            }

            SamplerState sampler_MainTex;
            SamplerState sampler_SubTex;
            Texture2D _MainTex;
            Texture2D _SubTex;
            float4 _MainTex_TexelSize;

            fixed4 frag (v2f i) : SV_Target
            {
                // blur the images
                // blurry image f0(sub) and f1(main)
                int hw = 0;
                float n = 0.0;
                fixed4 blur_main = (0.0);
                fixed4 blur_sub = (0.0);
                for (int ii = -hw; ii <= hw; ii++)
                {
                    for (int jj = -hw; jj <= hw; jj++)
                    {
                        float2 uv_offset = float2(1.0 / (float)_ScreenParams.x * ii, 1.0 / (float)_ScreenParams.y * jj);

                        blur_main += _MainTex.Sample(sampler_MainTex, i.uv + uv_offset);
                        blur_sub += _SubTex.Sample(sampler_SubTex, i.uv + uv_offset);
                        n += 1.0;
                    }
                }

                blur_main = blur_main / n;
                blur_sub = blur_sub / n;

                // background subtraction
                //fixed4 col_human = tex2D(_MainTex, i.uv);
                //fixed4 col_background = _SubTex.Sample(sampler_SubTex, i.uv);
                fixed4 col_human = blur_main;
                fixed4 col_background = blur_sub;

                float r_diff_square = pow(col_human.r - col_background.r, 2);
                float g_diff_square = pow(col_human.g - col_background.g, 2);
                float b_diff_square = pow(col_human.b - col_background.b, 2);
                float square_diff = r_diff_square + g_diff_square + b_diff_square;

                if (square_diff > 0.5f && square_diff < 3.0f)  // 0.02 for unchanged input
                {
                    // filter the noise
                    return _MainTex.Sample(sampler_MainTex, i.uv);
                }
                else
                {
                    return fixed4(1.0, 1.0, 1.0, 0.0);
                }

                //return col_human;

                // just invert the colors
                //col.rgb = 1 - col.rgb;
                //return col_background;
            }
            ENDCG
        }
    }
}
