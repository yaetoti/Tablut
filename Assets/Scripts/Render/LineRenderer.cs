using System.Collections.Generic;
using UnityEngine;

public sealed class LineRenderer {
  public sealed class Batch {
    public Material material;
    public Mesh mesh;
  }
  
  public sealed class MaterialGeometry {
    public readonly List<Vector3> vertices = new();
    public readonly List<int> indices = new();
  }

  private readonly Dictionary<Material, MaterialGeometry> m_geometries = new();
  private readonly List<Batch> m_batches = new();

  public void Reset() {
    m_geometries.Clear();
    m_batches.Clear();
  }
  
  public void AddLine(Vector3 start, Vector3 end, Material material) {
    if (!m_geometries.TryGetValue(material, out var entry)) {
      m_geometries[material] = entry = new();
    }
    
    var baseIndex = entry.vertices.Count;
    
    entry.vertices.Add(start);
    entry.vertices.Add(end);
    
    entry.indices.Add(baseIndex);
    entry.indices.Add(baseIndex + 1);
  }

  public void Combine() {
    foreach (var (material, geometry) in m_geometries) {
      Batch batch = new() {
        material = material,
        mesh = new()
      };
      
      batch.mesh.SetVertices(geometry.vertices);
      batch.mesh.SetIndices(geometry.indices, MeshTopology.Lines, 0);
      m_batches.Add(batch);
    }
    
    m_geometries.Clear();
  }

  public List<Batch> GetBatchList() {
    return m_batches;
  }
}