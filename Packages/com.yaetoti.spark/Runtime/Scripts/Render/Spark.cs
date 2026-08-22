using UnityEngine;

public static class Spark {
  public static MatrixStack Stack => SparkRenderer.Instance.MatrixStack;
  
  public static void Line(Vector3 start, Vector3 end, float thickness = 1.0f) {
    SparkRenderer.Instance.LineRenderer.AddLine(start, end, thickness);
  }
  
  public static void Line(Vector3 start, Vector3 end, Color color, float thickness = 1.0f) {
    SparkRenderer.Instance.LineRenderer.AddLine(start, end, color, thickness);
  }
  
  public static void Line(Material material, Vector3 start, Vector3 end, float thickness = 1.0f) {
    SparkRenderer.Instance.LineRenderer.AddLine(material, start, end, thickness);
  }
  
  public static void Line(Material material, Vector3 start, Vector3 end, Color color, float thickness = 1.0f) {
    SparkRenderer.Instance.LineRenderer.AddLine(material, start, end, color, thickness);
  }
}
