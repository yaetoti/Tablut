using System.Collections.Generic;
using UnityEngine;

public class MatrixStack {
  private readonly List<Matrix4x4> m_stack = new();

  public Matrix4x4 Top => m_stack.Count == 0 ? Matrix4x4.identity : m_stack[^1];

  public void Push(Matrix4x4 matrix) {
    m_stack.Add(Top * matrix);
  }

  public void TRS(Vector3 offset, Quaternion rotation, Vector3 scale) {
    Push(Matrix4x4.TRS(offset, rotation, scale));
  }
  
  public void Translate(Vector3 offset) {
    Push(Matrix4x4.Translate(offset));
  }
  
  public void Rotate(Quaternion rotation) {
    Push(Matrix4x4.Rotate(rotation));
  }
  
  public void Scale(Vector3 scale) {
    Push(Matrix4x4.Scale(scale));
  }

  public void Pop() {
    if (m_stack.Count == 0) {
      return;
    }
    
    m_stack.RemoveAt(m_stack.Count - 1);
  }

  public void Clear() {
    m_stack.Clear();
  }
}