using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;

//[ExecuteAlways]
[DefaultExecutionOrder(-1000)]
public class SparkRenderer : SceneSingleton<SparkRenderer> {
  private LineRenderer m_renderer = new();

  public void AddLine(Material material, Vector3 start, Vector3 end, float thickness = 1.0f) {
    m_renderer.AddLine(start, end, material, thickness);
  }
  
  protected override void Initialize() {
  }

  protected override void Cleanup() {
    m_renderer.Cleanup();
  }

  private void OnEnable() {
    RenderPipelineManager.beginContextRendering += OnBeginFrameRendering;
    RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
  }

  private void OnDisable() {
    RenderPipelineManager.beginContextRendering -= OnBeginFrameRendering;
    RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
  }

  private void OnBeginFrameRendering(ScriptableRenderContext ctx, List<Camera> cameras) {
    m_renderer.OnFrameRender(ctx, cameras);
  }
  
  private void OnBeginCameraRendering(ScriptableRenderContext ctx, Camera camera) {
    m_renderer.OnCameraRender(ctx, camera);
  }
}
