using UnityEngine;

public static class Spark {
  public static void Line(Material material, Vector3 start, Vector3 end, float thickness = 1.0f) {
    SparkRenderer.Instance.AddLine(material, start, end, thickness);
  }
}

public static class Spark2D {
  public static void Line(Material material, Vector2 start, Vector2 end, float thickness = 1.0f) {
    SparkRenderer.Instance.AddLine(material, start, end, thickness);
  }
}