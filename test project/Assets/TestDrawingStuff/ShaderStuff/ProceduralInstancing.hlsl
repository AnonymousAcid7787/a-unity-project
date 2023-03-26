#ifndef CUSTOM_INSTANCING_INCLUDED
#define CUSTOM_INSTANCING_INCLUDED

#pragma multi_compile_instancing

struct InstanceData {
	float4x4 worldMatrix;
	float4x4 worldMatrixInverse;
};

StructuredBuffer<InstanceData> _PerInstanceData;

void instancingSetup() {
	#ifndef SHADERGRAPH_PREVIEW
		#if UNITY_ANY_INSTANCING_ENABLED
			unity_ObjectToWorld = mul(unity_ObjectToWorld, _PerInstanceData[unity_InstanceID].worldMatrix);
			unity_WorldToObject = mul(unity_WorldToObject, _PerInstanceData[unity_InstanceID].worldMatrixInverse);
		#endif
	#endif
}

void GetInstanceID_float(out float Out){
	Out = 0;
	#ifndef SHADERGRAPH_PREVIEW
		#if UNITY_ANY_INSTANCING_ENABLED
			Out = unity_InstanceID;
		#endif
	#endif
}

void GetInstanceID_half(out float Out){
	Out = 0;
	#ifndef SHADERGRAPH_PREVIEW
		#if UNITY_ANY_INSTANCING_ENABLED
			Out = unity_InstanceID;
		#endif
	#endif
}

void InstancingLink_float(float3 Position, out float3 Out) {
	Out = Position;
}

void InstancingLink_half(float3 Position, out float3 Out) {
	Out = Position;
}

#endif