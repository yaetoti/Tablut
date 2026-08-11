using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

// TODO create shader
// TODO draw N meshes

// TODO call this method from static class Spark
// TODO clear data at the end

public class LineRenderPass : ScriptableRenderPass {
  public struct InstanceData {
    public Vector3 start;
    public float thickness;
    public Vector3 end;
    public float padding0;
  }

  public sealed class Batch {
    public readonly List<InstanceData> instanceData = new();
  }
  
  public sealed class CollectedData {
    public sealed class BatchData {
      public Material material;
      public int instanceCount;
      public int instanceOffset;
    }
    
    public readonly List<InstanceData> data = new();
    public readonly List<BatchData> batchData = new();
  }
  
  public sealed class PassData {
    public CollectedData collectedData; 
    public GraphicsBuffer instanceBuffer;
    public Matrix4x4 viewMatrix;
    public Matrix4x4 projectionMatrix;
  }

  private static readonly int INSTANCE_BUFFER = Shader.PropertyToID("_InstanceBuffer");
  private static readonly int INSTANCE_OFFSET = Shader.PropertyToID("_InstanceOffset");
  private readonly Dictionary<Material, Batch> m_batches = new();
  private GraphicsBuffer m_instanceBuffer;
  
  public LineRenderPass() {
    renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
  }
  
  public void AddLine(Vector3 start, Vector3 end, float thickness, Material material) {
    if (!m_batches.TryGetValue(material, out var batch)) {
      m_batches[material] = batch = new();
    }
    
    batch.instanceData.Add(new() { start = start, end = end, thickness = thickness });
  }
  
  private static void ExecutePass(PassData data, RasterGraphContext ctx) {
    ctx.cmd.SetViewProjectionMatrices(data.viewMatrix, data.projectionMatrix);
    
    var props = new MaterialPropertyBlock();
    props.SetBuffer(INSTANCE_BUFFER, data.instanceBuffer);
    
    foreach (var batchData in data.collectedData.batchData) {
      props.SetInteger(INSTANCE_OFFSET, batchData.instanceOffset);
      
      ctx.cmd.DrawProcedural(Matrix4x4.identity, batchData.material, 0, MeshTopology.Triangles, 6, batchData.instanceCount, props);
    }
  }
  
  public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData) {
    // Collect data and update buffers
    var collectedData = CollectInstanceData();
    if (collectedData.data.Count == 0) {
      return;
    }

    UpdateInstanceBuffer(collectedData.data);
    m_batches.Clear();

    // Import buffers
    var instanceBufferHandle = renderGraph.ImportBuffer(m_instanceBuffer);
    
    // Add pass
    using var builder = renderGraph.AddRasterRenderPass<PassData>(nameof(LineRenderPass), out var passData);
    var resourceData = frameData.Get<UniversalResourceData>();
    var cameraData = frameData.Get<UniversalCameraData>();
    
    builder.UseBuffer(instanceBufferHandle);
    builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
    builder.SetRenderFunc(static (PassData data, RasterGraphContext ctx) => ExecutePass(data, ctx));
    builder.AllowPassCulling(false);
    builder.AllowGlobalStateModification(true);

    // Set data
    passData.instanceBuffer = m_instanceBuffer;
    passData.collectedData = collectedData;
    passData.viewMatrix = cameraData.GetViewMatrix();
    passData.projectionMatrix = cameraData.GetProjectionMatrix();
  }

  public void Cleanup() {
    m_instanceBuffer?.Dispose();
    m_instanceBuffer = null;
  }
  
  private CollectedData CollectInstanceData() {
    CollectedData result = new();
    foreach (var (material, batch) in m_batches) {
      result.batchData.Add(new() {
        material = material,
        instanceCount = batch.instanceData.Count,
        instanceOffset = result.data.Count,
      });
      result.data.AddRange(batch.instanceData);
    }
    
    return result;
  }
  
  private void EnsureInstanceBufferCapacity(int capacity) {
    if (m_instanceBuffer is not null && m_instanceBuffer.count >= capacity) {
      return;
    }
    
    m_instanceBuffer?.Dispose();
    m_instanceBuffer = new(
      GraphicsBuffer.Target.Structured,
      capacity,
      Marshal.SizeOf<InstanceData>()
    );
  }

  private void UpdateInstanceBuffer(List<InstanceData> data) {
    EnsureInstanceBufferCapacity(data.Count);
    m_instanceBuffer.SetData(data);
  }
}