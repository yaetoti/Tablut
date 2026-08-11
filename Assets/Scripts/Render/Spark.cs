using UnityEngine;

public static class Spark {
  public static void Line(Material material, Vector3 start, Vector3 end, float thickness = 1.0f) {
    //SparkRendererFeature.Instance.LineRenderPass.AddLine(start, end, thickness, material);
    SparkRenderer.Instance.AddLine(material, start, end, thickness);
  }
}