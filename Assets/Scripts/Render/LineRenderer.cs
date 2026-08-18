using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;

public class LineRenderer {
  private struct InstanceData {
    //public Matrix4x4 transform;
    public Vector3 start;
    public float thickness;//swap
    public Vector3 end;
    public float padding0;//remove
  }

  private static readonly int INSTANCE_BUFFER = Shader.PropertyToID("_InstanceBuffer");
  private static readonly int INSTANCE_OFFSET = Shader.PropertyToID("_InstanceOffset");
  
  private Batcher<InstanceData> m_batcher = new();

  public void Cleanup() {
    m_batcher.Cleanup();
  }
  
  public void AddLine(Vector3 start, Vector3 end, float thickness = 1.0f) {
  }

  public void AddLine(Vector3 start, Vector3 end, Color color, float thickness = 1.0f) {
  }

  public void AddLine(Vector3 start, Vector3 end, Material material, float thickness = 1.0f) {
    m_batcher.Add(material, new() {
      //transform = Matrix4x4.identity,
      start = start,
      end = end,
      thickness = thickness
    });
  }
  
  public void OnFrameRender(ScriptableRenderContext ctx, List<Camera> camera) {
    m_batcher.Update();
  }

  public void OnCameraRender(ScriptableRenderContext ctx, Camera camera) {
    //if (camera.cameraType != CameraType.Game) {
    if (camera.cameraType != CameraType.Game && camera.cameraType != CameraType.SceneView) {
      return;
    }
    
    var ranges = m_batcher.GetRanges();
    var instanceBuffer = m_batcher.GetInstanceBuffer();
    var bounds = GetFrustumBounds(camera); 
      
    // Render
    foreach (var range in ranges) {
      var rp = new RenderParams(range.material) { matProps = new() };
      rp.camera = camera;
      // TODO crutch
      rp.worldBounds = bounds;
      rp.matProps.SetBuffer(INSTANCE_BUFFER, instanceBuffer);
      rp.matProps.SetInteger(INSTANCE_OFFSET, range.start);
      
      Graphics.RenderPrimitives(rp, MeshTopology.Triangles, 6, range.count);
    }
  }
  
  public static Bounds GetFrustumBounds(Camera cam) {
    // 1. Get the 4 corners of the near clip plane and 4 corners of the far clip plane
    Vector3[] nearCorners = new Vector3[4];
    Vector3[] farCorners = new Vector3[4];

    // Rect(0,0,1,1) represents the full screen view
    cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, nearCorners);
    cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, farCorners);

    // 2. Convert corners from local camera space to world space and initialize Bounds
    Vector3 firstWorldCorner = cam.transform.TransformPoint(nearCorners[0]);
    Bounds bounds = new Bounds(firstWorldCorner, Vector3.zero);

    // 3. Encapsulate all remaining 7 points into the bounding box
    for (int i = 1; i < 4; i++) {
      bounds.Encapsulate(cam.transform.TransformPoint(nearCorners[i]));
    }
    
    for (int i = 0; i < 4; i++) {
      bounds.Encapsulate(cam.transform.TransformPoint(farCorners[i]));
    }
    
    return bounds;
  }
}