using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

[DefaultExecutionOrder(-1000)]
public class SparkRenderer : SceneSingleton<SparkRenderer> {
  private static readonly int INSTANCE_BUFFER = Shader.PropertyToID("_InstanceBuffer");
  private static readonly int INSTANCE_OFFSET = Shader.PropertyToID("_InstanceOffset");

  private struct InstanceData {
    public Vector3 start;
    public float thickness;
    public Vector3 end;
    public float padding0;
  }
  
  private struct CollectedData {
    public Material material;
    public InstanceData instanceData;
  }

  private struct BatchData {
    public Material material;
    public int start;
    public int count;
  }

  private readonly List<CollectedData> m_collectedData = new();
  private GraphicsBuffer m_instanceBuffer;

  public void AddLine(Material material, Vector3 start, Vector3 end, float thickness = 1.0f) {
    m_collectedData.Add(new() {
      material = material,
      instanceData = new() {
        start = start,
        end = end,
        thickness = thickness,
      }
    });
  }
  
  protected override void Initialize() {
  }

  protected override void Cleanup() {
    m_instanceBuffer?.Dispose();
  }

  private void OnEnable() {
    RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
  }

  private void OnDisable() {
    RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
  }

  private void OnBeginCameraRendering(ScriptableRenderContext ctx, Camera camera) {
    if (camera.cameraType != CameraType.Game && camera.cameraType != CameraType.Preview) {
      return;
    }

    if (m_collectedData.Count == 0) {
      return;
    }

    // Sort data
    m_collectedData.Sort((left, right) => left.material.GetEntityId().CompareTo(right.material.GetEntityId()));

    // Prepare data
    List<InstanceData> instanceData = new(m_collectedData.Count);
    List<BatchData> batches = new();
    BatchData nextBatch = new() {
      start = 0,
      material = m_collectedData[0].material
    };
    var expectedMaterialId = m_collectedData[0].material.GetEntityId();
    
    for (int i = 0; i < m_collectedData.Count; ++i) {
      // Copy instance data to the plain buffer
      var data = m_collectedData[i];
      instanceData.Add(data.instanceData);

      // Collect batching metadata
      var material = data.material;
      var materialId = material.GetEntityId();
      if (materialId != expectedMaterialId) {
        // Add previous batch
        nextBatch.count = i - nextBatch.start;
        batches.Add(nextBatch);
        
        // Create a new batch
        nextBatch = new() {
          material = material,
          start = i,
        };
        expectedMaterialId = data.material.GetEntityId();
      }
    }
    
    // Add the last batch
    nextBatch.count = m_collectedData.Count - nextBatch.start;
    batches.Add(nextBatch);
    
    // Reallocate buffer if not enough space
    if (m_instanceBuffer is null || m_instanceBuffer.count < instanceData.Count) {
      m_instanceBuffer?.Dispose();
      m_instanceBuffer = new(GraphicsBuffer.Target.Structured, instanceData.Count, Marshal.SizeOf<InstanceData>());
    }
    
    // Set data
    m_instanceBuffer.SetData(instanceData);
    
    // Render
    foreach (var batch in batches) {
      var rp = new RenderParams(batch.material) { matProps = new() };
      rp.matProps.SetBuffer(INSTANCE_BUFFER, m_instanceBuffer);
      rp.matProps.SetInteger(INSTANCE_OFFSET, batch.start);
      
      Graphics.RenderPrimitives(rp, MeshTopology.Triangles, 6, batch.count);
    }
    
    // Cleanup
    m_collectedData.Clear();
  }
}
