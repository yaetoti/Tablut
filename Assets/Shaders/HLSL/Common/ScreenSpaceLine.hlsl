#pragma once

//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

struct InstanceData {
  float3 start;
  float thickness;
  float3 end;
  float padding0;
};

StructuredBuffer<InstanceData> _InstanceBuffer;
int _InstanceOffset;

void ScreenSpaceLine_float(float VertexID, float InstanceID, float3 InPosition, float2 InUV, out float3 Position, out float2 UV) {
  const float2 uvs[6] = {
    float2(0.0f, 1.0f), // TL
    float2(0.0f, 0.0f), // BL
    float2(1.0f, 1.0f), // TR
          
    float2(0.0f, 0.0f), // BL
    float2(1.0f, 0.0f), // BR
    float2(1.0f, 1.0f), // TR
  };
  
  uint vertexId = (uint)VertexID;
  uint instanceId = (uint)InstanceID;
  float2 uv = uvs[vertexId];
  
  #ifndef SHADERGRAPH_PREVIEW
  
  int bufferId = _InstanceOffset + instanceId;
  InstanceData data = _InstanceBuffer[bufferId];
  
  float4 startHCS = TransformWorldToHClip(data.start);
  float4 endHCS = TransformWorldToHClip(data.end);
  float2 startNDC = startHCS.xy / startHCS.w;
  float2 endNDC = endHCS.xy / endHCS.w;
  
  float2 lineDir = normalize(endNDC - startNDC);
  float2 normal = float2(-lineDir.y, lineDir.x);
  
  bool isEndPoint = uv.x > 0.5f;
  float4 hcs = isEndPoint ? endHCS : startHCS;
  float2 ndc = isEndPoint ? endNDC : startNDC;
  
  float2 pixelToNDC = 2.0 / _ScreenParams.xy;
  float2 offset = normal * data.thickness * 0.5 * pixelToNDC;
  ndc += offset * (uv.y * 2.0 - 1.0);
  
  float4 world = mul(UNITY_MATRIX_I_VP, float4(ndc * hcs.w, hcs.z, hcs.w));
  world.xyz /= world.w;
  
  Position = world.xyz;
  UV = uv;
  
  #else
  
  Position = InPosition;
  UV = InUV;
  
  #endif
}
