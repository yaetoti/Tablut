using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[DefaultExecutionOrder(-1000)]
public class SparkRenderer : SceneSingleton<SparkRenderer> {
  public LineRenderer LineRenderer { get; } = new();

  public MatrixStack MatrixStack { get; } = new();

  public Material defaultLineMaterial;
  
  protected override void Initialize() {
  }

  protected override void Cleanup() {
    LineRenderer.Cleanup();
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
    LineRenderer.OnFrameRender(ctx, cameras);
  }
  
  private void OnBeginCameraRendering(ScriptableRenderContext ctx, Camera camera) {
    LineRenderer.OnCameraRender(ctx, camera);
  }
}
