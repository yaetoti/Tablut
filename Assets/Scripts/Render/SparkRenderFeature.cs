using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SparkRenderFeature : ScriptableRendererFeature {
  private LineRenderPass m_pass;
  
  public override void Create() {
    m_pass = new() {
      renderPassEvent = RenderPassEvent.AfterRenderingTransparents
    };
  }

  public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
    var camera = renderingData.cameraData.camera;
    if (camera.cameraType != CameraType.Game && camera.cameraType != CameraType.SceneView) {
      return;
    }
    
    renderer.EnqueuePass(m_pass);
  }
}