using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

// Batch
// Scissors

public class LineRenderPass : ScriptableRenderPass {
  private sealed class PassData {
    public List<LineRenderer.Batch> batches;
  }
  
  private static void ExecutePass(PassData data, RasterGraphContext ctx) {
    foreach (var batch in data.batches) {
      ctx.cmd.DrawMesh(batch.mesh, Matrix4x4.identity, batch.material, 0, 0);
    }
  }
  
  public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData) {
    if (RenderData.Instance is null) {
      return;
    }
    
    const string name = "LineRenderPass";

    using var builder = renderGraph.AddRasterRenderPass<PassData>(name, out var passData);
    passData.batches = RenderData.Instance.LineRenderer.GetBatchList();
    
    var resourceData = frameData.Get<UniversalResourceData>();
    builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
    builder.AllowPassCulling(false);
    builder.SetRenderFunc(static (PassData data, RasterGraphContext ctx) => ExecutePass(data, ctx));
  }
}