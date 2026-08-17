using System;
using System.Collections.Generic;
using Unity.Collections;

public static class NativeListExtensions {
  private struct SortIndicesComparer<T> : IComparer<int> where T : unmanaged, IComparable<T> {
    private NativeArray<T> m_keys;

    public SortIndicesComparer(NativeArray<T> keys) {
      m_keys = keys;
    }

    public int Compare(int x, int y) {
      return m_keys[x].CompareTo(m_keys[y]);
    }
  }
  
  public static NativeArray<int> GetSortedIndices<T, TComp>(this NativeList<T> arr, TComp comparer)
  where T : unmanaged
  where TComp : IComparer<int> {
    var indices = new NativeArray<int>(arr.Length, Allocator.Temp);
    for (int i = 0; i < indices.Length; ++i) {
      indices[i] = i;
    }
    
    indices.Sort(comparer);
    return indices;
  }
  
  public static NativeArray<int> GetSortedIndices<T>(this NativeList<T> arr)
  where T : unmanaged, IComparable<T> {
    var indices = new NativeArray<int>(arr.Length, Allocator.Temp);
    for (int i = 0; i < indices.Length; ++i) {
      indices[i] = i;
    }
    
    indices.Sort(new SortIndicesComparer<T>(arr.AsArray()));
    return indices;
  }
  
  public static void ApplyIndices<T>(this NativeList<T> elements, in NativeArray<int> indices)
  where T : unmanaged {
    int size = elements.Length;
    var temp = new NativeArray<T>(size, Allocator.Temp);
    for (int i = 0; i < size; ++i) {
      temp[i] = elements[indices[i]];
    }
    
    elements.AsArray().CopyFrom(temp);
    temp.Dispose();
  }
}