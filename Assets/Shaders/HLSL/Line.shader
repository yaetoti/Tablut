Shader "Custom/Line" {
  Properties {
    [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
    [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
  }

  SubShader {
    Tags {
      "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"
    }

    Pass {
      HLSLPROGRAM
      #pragma vertex VsMain
      #pragma fragment PsMain

      #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

      struct VsInput {
        uint vertexId : SV_VertexID;
        uint instanceId : SV_InstanceID;
      };

      struct VsOutput {
        float4 positionHCS : SV_POSITION;
        float2 uv : TEXCOORD0;
        //float4 debug : DBG;
      };

      struct InstanceData {
        float3 start;
        float thickness;
        float3 end;
        float padding0;
      };
      
      Texture2D _BaseMap;
      SamplerState sampler_BaseMap;

      cbuffer UnityPerMaterial {
        half4 _BaseColor;
        float4 _BaseMap_ST;
      }
      
      StructuredBuffer<InstanceData> _InstanceBuffer;
      int _InstanceOffset;

      VsOutput VsMain(VsInput input) {
        const float2 uvs[6] = {
          float2(0.0f, 1.0f), // TL
          float2(0.0f, 0.0f), // BL
          float2(1.0f, 1.0f), // TR
          
          float2(0.0f, 0.0f), // BL
          float2(1.0f, 0.0f), // BR
          float2(1.0f, 1.0f), // TR
        };
        
        VsOutput output;
        
        int bufferId = _InstanceOffset + input.instanceId;
        InstanceData data = _InstanceBuffer[bufferId];
        
        float4 startHCS = TransformWorldToHClip(data.start);
        float4 endHCS = TransformWorldToHClip(data.end);
        float2 startNDC = startHCS.xy / startHCS.w;
        float2 endNDC = endHCS.xy / endHCS.w;
        
        float2 lineDir = normalize(endNDC - startNDC);
        float2 normal = float2(-lineDir.y, lineDir.x);
        
        float2 uv = uvs[input.vertexId];
        bool isEndPoint = uv.x > 0.5f;
        float4 hcs = isEndPoint ? endHCS : startHCS;
        float2 ndc = isEndPoint ? endNDC : startNDC;
        
        float2 pixelToNDC = 2.0 / _ScreenParams.xy;
        float2 offset = normal * data.thickness * 0.5 * pixelToNDC;
        ndc += offset * (uv.y * 2.0 - 1.0);
        //ndc = uv;
        
        output.positionHCS = float4(ndc * hcs.w, hcs.z, hcs.w);
        output.uv = uv;
        //output.debug = float4(data.end, 1.0);
        
        return output;
      }

      half4 PsMain(VsOutput input) : SV_Target {
        half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
        return _BaseColor;
        //return input.debug;
      }
      ENDHLSL
    }
  }
}