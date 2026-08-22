#pragma once

#include "ShaderApiReflectionSupport.hlsl"

#if defined(__INTELLISENSE__) || defined(__RESHARPER__)
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#endif

struct InstanceData {
  float4x4 transform;
  float4 color;
  float3 start;
  float3 end;
  float thickness;
};

StructuredBuffer<InstanceData> _InstanceBuffer;
int _InstanceOffset;

///<funchints>
///     <sg:ProviderKey>Line</sg:ProviderKey>
///     <sg:DisplayName>Line</sg:DisplayName>
///     <sg:SearchCategory>Spark</sg:SearchCategory>
///</funchints>
UNITY_EXPORT_REFLECTION
void Line(
  float VertexID, float InstanceID, float3 InPosition, float2 InUV,
  out float3 Position, out float2 UV,
  out float4x4 Transform, out float4 Color, out float3 Start, out float3 End, out float Thickness)
{
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
  
  Transform = float4x4(
    1, 0, 0, 0,
    0, 1, 0, 0,
    0, 0, 1, 0,
    0, 0, 0, 1
  );
  Color = float4(1, 1, 1, 1);
  Start = float3(0, 0, 0);
  End = float3(1, 0, 0);
  Thickness = 1.0f;
  
  #else
  
  int bufferId = _InstanceOffset + instanceId;
  InstanceData data = _InstanceBuffer[bufferId];
  
  float3 start = mul(data.transform, float4(data.start, 1.0)).xyz;
  float3 end = mul(data.transform, float4(data.end, 1.0)).xyz;

  float3 lineVec = end - start;
  float3 lineDir = normalize(lineVec);
  
  float3 cameraPos = GetCameraPositionWS();
  float3 cameraVec = start - cameraPos;
  
  float3 lineCameraDir = normalize(cameraVec - dot(cameraVec, lineDir) * lineDir);
  float3 lineNormal = cross(lineDir, lineCameraDir);
  
  float3 coord = uv.x > 0.5 ? end : start;
  float3 offset = lineNormal * data.thickness * 0.5;
  coord += offset * (uv.y * 2.0 - 1.0);
  
  // Set output
  Position = coord;
  UV = uv;
  
  Transform = data.transform;
  Color = data.color;
  Start = data.start;
  End = data.end;
  Thickness = data.thickness;
  
  #endif
}
