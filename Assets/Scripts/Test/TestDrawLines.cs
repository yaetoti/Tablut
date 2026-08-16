using UnityEngine;

[ExecuteAlways]
public class TestDrawLines : MonoBehaviour {
  public Material material;

  private void Update() {
    const float thickness = 3.0f;
    for (float z = 0.0f; z < 1000.0f; z += 100.0f) {
      //Spark2D.Line(material, );
      
      Spark.Line(material, new(-5.0f,  1.5f, z), new(-5.0f, -1.5f, z), thickness);
      Spark.Line(material, new(-3.5f,  1.5f, z), new(-3.5f, -1.5f, z), thickness);
      Spark.Line(material, new(-5.0f,  0.0f, z), new(-3.5f,  0.0f, z), thickness);

      // E
      Spark.Line(material, new(-2.8f,  1.5f, z), new(-2.8f, -1.5f, z), thickness);
      Spark.Line(material, new(-2.8f,  1.5f, z), new(-1.0f,  1.5f, z), thickness);
      Spark.Line(material, new(-2.8f,  0.0f, z), new(-1.2f,  0.0f, z), thickness);
      Spark.Line(material, new(-2.8f, -1.5f, z), new(-1.0f, -1.5f, z), thickness);

      // L
      Spark.Line(material, new(-0.3f,  1.5f, z), new(-0.3f, -1.5f, z), thickness);
      Spark.Line(material, new(-0.3f, -1.5f, z), new( 1.2f, -1.5f, z), thickness);

      // L
      Spark.Line(material, new( 1.7f,  1.5f, z), new( 1.7f, -1.5f, z), thickness);
      Spark.Line(material, new( 1.7f, -1.5f, z), new( 3.2f, -1.5f, z), thickness);

      // O
      Spark.Line(material, new( 3.8f,  1.5f, z), new( 5.3f,  1.5f, z), thickness);
      Spark.Line(material, new( 5.3f,  1.5f, z), new( 5.3f, -1.5f, z), thickness);
      Spark.Line(material, new( 5.3f, -1.5f, z), new( 3.8f, -1.5f, z), thickness);
      Spark.Line(material, new( 3.8f, -1.5f, z), new( 3.8f,  1.5f, z), thickness);
    }
  }
}