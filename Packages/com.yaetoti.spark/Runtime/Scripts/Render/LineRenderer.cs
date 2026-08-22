using System.Collections.Generic;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.Rendering;

public class LineRenderer {
  private struct InstanceData {
    public Matrix4x4 transform;
    public Color color;
    public Vector3 start;
    public Vector3 end;
    public float thickness;
  }

  private static readonly int INSTANCE_BUFFER = Shader.PropertyToID("_InstanceBuffer");
  private static readonly int INSTANCE_OFFSET = Shader.PropertyToID("_InstanceOffset");
  
  private Batcher<InstanceData> m_batcher = new();

  public void Cleanup() {
    m_batcher.Cleanup();
  }
  
  public void AddLine(Vector3 start, Vector3 end, float thickness = 1.0f) {
    m_batcher.Add(SparkRenderer.Instance.defaultLineMaterial, new() {
      transform = SparkRenderer.Instance.MatrixStack.Top,
      color = Color.white,
      start = start,
      end = end,
      thickness = thickness
    });
  }

  public void AddLine(Vector3 start, Vector3 end, Color color, float thickness = 1.0f) {
    m_batcher.Add(SparkRenderer.Instance.defaultLineMaterial, new() {
      transform = SparkRenderer.Instance.MatrixStack.Top,
      color = color,
      start = start,
      end = end,
      thickness = thickness
    });
  }
  
  public void AddLine(Material material, Vector3 start, Vector3 end, float thickness = 1.0f) {
    m_batcher.Add(material, new() {
      transform = SparkRenderer.Instance.MatrixStack.Top,
      color = Color.white,
      start = start,
      end = end,
      thickness = thickness
    });
  }

  public void AddLine(Material material, Vector3 start, Vector3 end, Color color, float thickness = 1.0f) {
    m_batcher.Add(material, new() {
      transform = SparkRenderer.Instance.MatrixStack.Top,
      color = color,
      start = start,
      end = end,
      thickness = thickness
    });
  }
  
  public void OnFrameRender(ScriptableRenderContext ctx, List<Camera> camera) {
    m_batcher.Update();
  }

  public void OnCameraRender(ScriptableRenderContext ctx, Camera camera) {
    if (camera.cameraType != CameraType.Game && camera.cameraType != CameraType.SceneView) {
      return;
    }
    
    var ranges = m_batcher.GetRanges();
    var instanceBuffer = m_batcher.GetInstanceBuffer();
    var bounds = GetFrustumBounds(camera); 
    
    // Render
    foreach (var range in ranges) {
      var rp = new RenderParams(range.material);
      rp.camera = camera;
      rp.worldBounds = bounds;
      rp.matProps = new();
      rp.matProps.SetBuffer(INSTANCE_BUFFER, instanceBuffer);
      rp.matProps.SetInteger(INSTANCE_OFFSET, range.start);
      
      Graphics.RenderPrimitives(rp, MeshTopology.Triangles, 6, range.count);
    }
  }
  
  public static Bounds GetFrustumBounds(Camera cam) {
    Vector3[] nearCorners = new Vector3[4];
    Vector3[] farCorners = new Vector3[4];
    cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, nearCorners);
    cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, farCorners);
    
    Bounds bounds = new Bounds(cam.transform.TransformPoint(nearCorners[0]), Vector3.zero);
    bounds.Encapsulate(cam.transform.TransformPoint(farCorners[0]));

    for (int i = 1; i < 4; i++) {
      bounds.Encapsulate(cam.transform.TransformPoint(nearCorners[i]));
      bounds.Encapsulate(cam.transform.TransformPoint(farCorners[i]));
    }
    
    return bounds;
  }
}