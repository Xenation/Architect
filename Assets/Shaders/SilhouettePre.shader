// UNITY_SHADER_NO_UPGRADE
Shader "Architect/SilhouettePre"
{
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	Properties { }

	SubShader {
		Tags {
			"RenderType" = "Opaque"
			"RenderPipeline" = "LightweightPipeline"
			"Queue" = "Geometry-2"
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------------------
		// Render the object with stencil=1 to mask out the part that isn't the silhouette
		//-------------------------------------------------------------------------------------------------------------------------------------------------------------
		Pass
		{
			//ColorMask 0
			Cull Off
			ZWrite Off
			Stencil {
				Ref 1
				Comp always
				Pass replace
			}

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

			struct VertexIn {
				float4 position : POSITION;
			};

			struct FragmentIn {
				float4 position : SV_POSITION;
			};

			FragmentIn vert(VertexIn i) {
				FragmentIn o;
				o.position = UnityObjectToClipPos(i.position.xyzw);
				return o;
			}

			fixed4 frag(FragmentIn i) : SV_Target {
				return float4(1.0, 0.0, 0.0, 1.0);
			}
			ENDCG
		}
	}
}