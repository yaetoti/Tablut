using UnityEngine;

[ExecuteAlways]
public class TestDrawLines : MonoBehaviour {
  public Material material;
  
  private void Update() {
    RenderData.Instance.LineRenderer.AddLine(new(0.0f, 0.0f, 0.0f), new(1.0f, 1.0f, 1.0f), material);
  }
}