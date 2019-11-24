Shader "Hidden/Custom/OutlineUnlit" {
    SubShader {
        Tags { "RenderType"="Opaque" }
        ZWrite Off
		ZTest Always

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
            };

			fixed4 _OutlineColor;
			int _FIX_IsSecondEye;
			float4x4 _FIX_SecondEyeV;
			float4x4 _FIX_SecondEyeP;

            v2f vert (appdata v) {
                v2f o;
				if (_FIX_IsSecondEye != 0) {
					o.vertex = mul(_FIX_SecondEyeP, mul(_FIX_SecondEyeV, mul(unity_ObjectToWorld, v.vertex)));
				} else {
					o.vertex = UnityObjectToClipPos(v.vertex);
				}
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                return _OutlineColor;
            }
            ENDCG
        }
    }
}
