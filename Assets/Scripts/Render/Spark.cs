using UnityEngine;

public static class Spark {
  public static void Line(Vector3 start, Vector3 end, float thickness, Material material) {
    SparkRendererFeature.Instance.LineRenderPass.AddLine(start, end, thickness, material);
  }
}