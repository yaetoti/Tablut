using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// TODO fix first frame nullptr
// TODO better batching (sorting by material id)
// TODO measure time

public class SparkRendererFeature : ScriptableRendererFeature {
  public static SparkRendererFeature Instance { get; private set; }

  public LineRenderPass LineRenderPass { get; private set; }

  public override void Create() {
    if (Instance is not null && Instance != this) {
      throw new("Only one SparkRendererFeature is supported");
    }

    // Create passes
    LineRenderPass ??= new();
    
    Instance ??= this;
  }

  private void OnDestroy() {
    Dispose(true);
  }

  protected override void Dispose(bool disposing) {
    if (Instance != this) {
      return;
    }
    
    // Cleanup
    LineRenderPass.Cleanup();
    
    Instance = null;
  }

  public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
    var camera = renderingData.cameraData.camera;
    if (camera.cameraType != CameraType.Game && camera.cameraType != CameraType.SceneView) {
      return;
    }
    
    renderer.EnqueuePass(LineRenderPass);
  }
}