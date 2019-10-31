
uniform int _VolumeLightCount;
uniform float4 _VolumeLightExtentsBuffer[64];
uniform float4 _VolumeLightColorBuffer[64];
uniform float4x4 _VolumeLightTransformBuffer[64];


void GetVolumeLightCount(out int VolumeLightCount) {
	VolumeLightCount = _VolumeLightCount;
}

void BoxSDF(float4x4 transform, float3 extents, float3 pos, out float dist) {
	pos = mul(transform, float4(pos, 1)).xyz;
	float3 q = abs(pos) - extents;
	dist = length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

float remap(float x, float from1, float to1, float from2, float to2) {
	return (x - from1) / (to1 - from1) * (to2 - from2) + from2;
}

void ComputeVolumeLights_float(float3 worldPos, out float3 litColor) {
	float dist = float(0);
	litColor = float3(0, 0, 0);

	for (int i = 0; i < _VolumeLightCount; i++) {
		BoxSDF(_VolumeLightTransformBuffer[i], _VolumeLightExtentsBuffer[i].xyz, worldPos, dist);
		litColor += saturate(remap(dist, 0.01, 0, 0, 1)) * _VolumeLightColorBuffer[i].rgb * _VolumeLightColorBuffer[i].w;
	}

}
