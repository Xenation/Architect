// UNITY_SHADER_NO_UPGRADE
Shader "Architect/SilhouettePost"
{
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	Properties
	{
		_OutlineColor( "Outline Color", Color ) = ( .5, .5, .5, 1 )
		_OutlineWidth( "Outline width", Range ( .001, 0.03 ) ) = .005
		_CornerAdjust( "Corner Adjustment", Range( 0, 2 ) ) = .5
	}

	SubShader {
		Tags {
			"RenderType" = "Opaque"
			"RenderPipeline" = "LightweightPipeline"
			"Queue" = "Geometry-1"
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------------------
		// Render the outline by extruding along vertex normals and using the stencil mask previously rendered. Only render depth, so that the final pass executes
		// once per fragment (otherwise alpha blending will look bad).
		//-------------------------------------------------------------------------------------------------------------------------------------------------------------
		Pass
		{
			Cull Off
			ZWrite On
			Stencil {
				Ref 1
				Comp notequal
				Pass keep
				Fail keep
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform float4 _OutlineColor;
			uniform float _OutlineWidth;
			uniform float _CornerAdjust;

			struct VertexIn {
				float4 position : POSITION;
				float3 normal : NORMAL;
			};

			struct VertexOut {
				float4 positionObject : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float4 position : SV_POSITION;
			};

			VertexOut vert(VertexIn i) {
				VertexOut o;
				o.positionObject.xyzw = i.position;
				o.normal.xyz = i.normal.xyz;
				o.position = UnityObjectToClipPos(i.position.xyzw);

				return o;
			}

			VertexOut extrude(VertexOut v) {
				VertexOut extruded = v;

				float3 normalView = mul((float3x3) UNITY_MATRIX_IT_MV, v.normal.xyz);
				float2 offsetProj = TransformViewToProjection(normalView.xy);
				offsetProj.xy = normalize(offsetProj.xy);

				extruded.position = UnityObjectToClipPos(v.positionObject.xyzw);
				extruded.position.xy += offsetProj.xy * extruded.position.w * _OutlineWidth;

				return extruded;
			}

			[maxvertexcount(18)]
			void geom(triangle VertexOut inTriangle[3], inout TriangleStream<VertexOut> outStream) {
				float3 v0v1 = normalize(inTriangle[0].position.xyz - inTriangle[1].position.xyz);
				float3 v1v2 = normalize(inTriangle[1].position.xyz - inTriangle[2].position.xyz);
				float3 v2v0 = normalize(inTriangle[2].position.xyz - inTriangle[0].position.xyz);

				inTriangle[0].normal = inTriangle[0].normal - normalize(v0v1 - v2v0) * _CornerAdjust;
				inTriangle[1].normal = inTriangle[1].normal - normalize(v1v2 - v0v1) * _CornerAdjust;
				inTriangle[2].normal = inTriangle[2].normal - normalize(v2v0 - v1v2) * _CornerAdjust;

				VertexOut extruded0 = extrude(inTriangle[0]);
				VertexOut extruded1 = extrude(inTriangle[1]);
				VertexOut extruded2 = extrude(inTriangle[2]);

				outStream.Append(inTriangle[0]);
				outStream.Append(extruded0);
				outStream.Append(inTriangle[1]);
				outStream.Append(extruded0);
				outStream.Append(extruded1);
				outStream.Append(inTriangle[1]);

				outStream.Append(inTriangle[1]);
				outStream.Append(extruded1);
				outStream.Append(extruded2);
				outStream.Append(inTriangle[1]);
				outStream.Append(extruded2);
				outStream.Append(inTriangle[2]);

				outStream.Append(inTriangle[2]);
				outStream.Append(extruded2);
				outStream.Append(inTriangle[0]);
				outStream.Append(extruded2);
				outStream.Append(extruded0);
				outStream.Append(inTriangle[0]);
			}

			fixed4 frag(VertexOut i) : SV_TARGET {
				return _OutlineColor;
			}

			ENDCG
		}
	}
}