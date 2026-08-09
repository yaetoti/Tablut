using UnityEngine;

public class TestDrawLines : MonoBehaviour {
  public Material material;

  private void Update() {
    Spark.Line(new(0.0f, 0.0f, 0.0f), new(1.0f, 0.0f, 0.0f), 5.0f, material);
    Spark.Line(new(0.0f, 0.1f, 0.0f), new(1.0f, 0.1f, 0.0f), 4.0f, material);
    Spark.Line(new(0.0f, 0.2f, 0.0f), new(1.0f, 0.2f, 0.0f), 3.0f, material);
    Spark.Line(new(0.0f, 0.3f, 0.0f), new(1.0f, 0.3f, 0.0f), 2.0f, material);
    Spark.Line(new(0.0f, 0.4f, 0.0f), new(1.0f, 0.4f, 0.0f), 1.0f, material);
    Spark.Line(new(0.0f, 0.5f, 0.0f), new(1.0f, 0.5f, 0.0f), 0.5f, material);
  }
}