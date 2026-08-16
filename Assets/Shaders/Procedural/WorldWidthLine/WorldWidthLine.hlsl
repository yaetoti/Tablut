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

///<funchints>
///     <sg:ProviderKey>WorldWidthLine</sg:ProviderKey>
///     <sg:DisplayName>World Width Line</sg:DisplayName>
///     <sg:SearchCategory>Spark</sg:SearchCategory>
///</funchints>
UNITY_EXPORT_REFLECTION
void WorldWidthLine(float VertexID, float InstanceID, float3 InPosition, float2 InUV, out float3 Position, out float2 UV) {
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
  
  #if defined(SHADERGRAPH_PREVIEW) || defined(SHADERGRAPH_PREVIEW_MAIN)
  
  Position = InPosition;
  UV = InUV;
  
  #else
  
  int bufferId = _InstanceOffset + instanceId;
  InstanceData data = _InstanceBuffer[bufferId];
  
  float3 lineVec = data.end - data.start;
  float3 lineDir = normalize(lineVec);
  
  float3 cameraPos = GetCameraPositionWS();
  float3 cameraVec = data.start - cameraPos;
  
  float3 lineCameraDir = normalize(cameraVec - dot(cameraVec, lineDir) * lineDir);
  float3 lineNormal = cross(lineDir, lineCameraDir);
  
  float3 coord = uv.x > 0.5 ? data.end : data.start;
  float3 offset = lineNormal * data.thickness * 0.5;
  coord += offset * (uv.y * 2.0 - 1.0);
  
  // Normalize UV
  uv.x *= length(lineVec) / data.thickness;
  
  Position = coord;
  UV = uv;
  
  #endif
}
