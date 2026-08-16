#pragma once

#include "ShaderApiReflectionSupport.hlsl"

#if defined(__INTELLISENSE__) || defined(__RESHARPER__)
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#endif

struct InstanceData {
  float3 start;
  float thickness;
  float3 end;
  float padding0;
};

StructuredBuffer<InstanceData> _InstanceBuffer;
int _InstanceOffset;

float3 TransformHClipToWorld(float4 position) {
  float4 unprojectedPoint =  mul(UNITY_MATRIX_I_VP, position);
  return unprojectedPoint.xyz / unprojectedPoint.w;
}

///<funchints>
///     <sg:ProviderKey>PixelWidthLine</sg:ProviderKey>
///     <sg:DisplayName>Pixel Width Line</sg:DisplayName>
///     <sg:SearchCategory>Spark</sg:SearchCategory>
///</funchints>
UNITY_EXPORT_REFLECTION
void PixelWidthLine(float VertexID, float InstanceID, float3 InPosition, float2 InUV, out float3 Position, out float2 UV) {
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
  
  float4 startCS = TransformWorldToHClip(data.start);
  float4 endCS = TransformWorldToHClip(data.end);
  
  // Test
  float2 startSS = (startCS.xy / startCS.w * 0.5 + 0.5) * _ScreenParams.xy;
  float2 endSS = (endCS.xy / endCS.w * 0.5 + 0.5) * _ScreenParams.xy;
  
  float2 lineDir = normalize(endSS - startSS);
  float2 perpendicular = float2(-lineDir.y, lineDir.x);
  
  float2 offsetSS = perpendicular * (data.thickness * 0.5) * (uv.y * 2.0 - 1.0);
  float2 offsetNDC = offsetSS / _ScreenParams.xy * 2.0;
  
  float4 posCS = lerp(startCS, endCS, uv.x);
  posCS.xy += offsetNDC * posCS.w;
  
  // UV normalization over length
  uv.x *= length(endSS - startSS) / data.thickness;
  
  Position = TransformHClipToWorld(posCS);
  UV = uv;
  
  #else
  
  Position = InPosition;
  UV = InUV;
  
  #endif
}
