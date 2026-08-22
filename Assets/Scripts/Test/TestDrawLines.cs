using UnityEngine;

//[ExecuteAlways]
public class TestDrawLines : MonoBehaviour {
  private void Update() {
    const float thickness = 0.1f;
    Color color = Color.blue;
    for (float z = 0.0f; z < 1000.0f; z += 2.0f) {
      Spark.Stack.Translate(new(0.1f, 0.0f, 0.0f));
      Spark.Stack.Rotate(Quaternion.Euler(0, 0, 5));
      
      Spark.Line(new(-5.0f,  1.5f, z), new(-5.0f, -1.5f, z), color, thickness);
      Spark.Line(new(-3.5f,  1.5f, z), new(-3.5f, -1.5f, z), color, thickness);
      Spark.Line(new(-5.0f,  0.0f, z), new(-3.5f,  0.0f, z), color, thickness);

      // E
      Spark.Line(new(-2.8f,  1.5f, z), new(-2.8f, -1.5f, z), color, thickness);
      Spark.Line(new(-2.8f,  1.5f, z), new(-1.0f,  1.5f, z), color, thickness);
      Spark.Line(new(-2.8f,  0.0f, z), new(-1.2f,  0.0f, z), color, thickness);
      Spark.Line(new(-2.8f, -1.5f, z), new(-1.0f, -1.5f, z), color, thickness);

      // L
      Spark.Line(new(-0.3f,  1.5f, z), new(-0.3f, -1.5f, z), color, thickness);
      Spark.Line(new(-0.3f, -1.5f, z), new( 1.2f, -1.5f, z), color, thickness);

      // L
      Spark.Line(new( 1.7f,  1.5f, z), new( 1.7f, -1.5f, z), color, thickness);
      Spark.Line(new( 1.7f, -1.5f, z), new( 3.2f, -1.5f, z), color, thickness);

      // O
      Spark.Line(new( 3.8f,  1.5f, z), new( 5.3f,  1.5f, z), color, thickness);
      Spark.Line(new( 5.3f,  1.5f, z), new( 5.3f, -1.5f, z), color, thickness);
      Spark.Line(new( 5.3f, -1.5f, z), new( 3.8f, -1.5f, z), color, thickness);
      Spark.Line(new( 3.8f, -1.5f, z), new( 3.8f,  1.5f, z), color, thickness);
    }
    
    Spark.Stack.Clear();
  }
}