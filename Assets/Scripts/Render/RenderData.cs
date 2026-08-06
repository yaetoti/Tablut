using UnityEngine;

[ExecuteAlways]
public class RenderData : SceneSingleton<RenderData> {
  public LineRenderer LineRenderer { get; } = new();

  private void LateUpdate() {
    LineRenderer.Combine();
  }
}
