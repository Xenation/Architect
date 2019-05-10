Shader "Hidden/Custom/OutlineCompositor" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader {
        Cull Off
		ZWrite Off
		ZTest Always

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
			sampler2D _OutlineTex;
			sampler2D _BlurTex;
			sampler2D _TmpTex;

            fixed4 frag (v2f i) : SV_Target {
				fixed4 blurColor = tex2D(_BlurTex, i.uv);
                fixed4 outlineColor = tex2D(_OutlineTex, i.uv);
				fixed4 col = tex2D(_MainTex, i.uv);

				blurColor.a = clamp(clamp(blurColor.a + 0.5, 0, 1) - outlineColor.a, 0, 1);
                return fixed4(blurColor.rgb * blurColor.a + col.rgb * (1 - blurColor.a), 1);
            }
            ENDCG
        }
    }
}
