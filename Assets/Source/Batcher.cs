using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class Batcher<TData>
where TData : unmanaged {
  public readonly struct BatchRange {
    public readonly Material material;
    public readonly int start;
    public readonly int count;

    public BatchRange(Material material, int start, int count) {
      this.material = material;
      this.start = start;
      this.count = count;
    }
  }
  
  // Frame data
  private NativeList<TData> m_data = new(Allocator.Persistent);
  private NativeList<EntityId> m_materialIds = new(Allocator.Persistent);
  private readonly Dictionary<EntityId, Material> m_materials = new();
  
  // Result data
  private GraphicsBuffer m_instanceBuffer;
  private readonly List<BatchRange> m_ranges = new();
  
  public GraphicsBuffer GetInstanceBuffer() {
    return m_instanceBuffer;
  }
  
  public IReadOnlyList<BatchRange> GetRanges() {
    return m_ranges;
  }

  public void Cleanup() {
    m_data.Dispose();
    m_materialIds.Dispose();
    m_instanceBuffer?.Dispose();
    m_instanceBuffer = null;
  } 
  
  public void Add(Material material, TData data) {
    // Add material lookup
    var materialId = material.GetEntityId();
    m_materials.TryAdd(materialId, material);

    // Add values
    m_data.Add(data);
    m_materialIds.Add(materialId);
  }
  
  public void Update() {
    // Always reallocate. Assume worst case scenario - totally different amount of geometry every frame. 
    ClearResultData();
    
    // Nothing to collect
    int bufferSize = m_materialIds.Length;
    if (bufferSize == 0) {
      ClearCollectedData();
      return;
    }
    
    // Sort arrays by material id
    using (var indices = m_materialIds.GetSortedIndices()) {
      m_materialIds.ApplyIndices(indices);
      m_data.ApplyIndices(indices);
    }
    
    // Set instance buffer data
    if (m_instanceBuffer is null || m_instanceBuffer.count < bufferSize) {
      m_instanceBuffer?.Dispose();
      m_instanceBuffer = new(GraphicsBuffer.Target.Structured, bufferSize, UnsafeUtility.SizeOf<TData>());
    }
    
    m_instanceBuffer.SetData(m_data.AsArray());
    
    // Collect ranges
    EntityId materialId = m_materialIds[0];
    int start = 0;
    int count = 1;

    while (start + count < bufferSize) {
      var currentMaterialId = m_materialIds[start + count];

      // New batch
      if (currentMaterialId != materialId) {
        m_ranges.Add(new(m_materials[materialId], start, count));

        materialId = currentMaterialId; 
        start += count;
        count = 1;
        continue;
      }

      // Next element
      ++count;
    }

    m_ranges.Add(new(m_materials[materialId], start, count));
    
    // Clear collected data
    ClearCollectedData();
  }
  
  private void ClearCollectedData() {
    m_data.Clear();
    m_materialIds.Clear();
    m_materials.Clear();
  }
  
  private void ClearResultData() {
    m_ranges.Clear();
    //m_instanceBuffer?.Dispose();
    //m_instanceBuffer = null;
  }
}