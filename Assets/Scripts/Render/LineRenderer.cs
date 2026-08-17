using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;

public class LineRenderer {
  private struct InstanceData {
    public Matrix4x4 transform;
    public Vector3 start;
    public Vector3 end;
    public float thickness;
  }
  
  public void AddLine(Vector3 start, Vector3 end, float thickness = 1.0f) {
    // m_entries.Add(new() {
    //   transform = Matrix4x4.identity,
    //   start = start,
    //   end = end,
    //   thickness = thickness
    // });
  }

  public void AddLine(Vector3 start, Vector3 end, Color color, float thickness = 1.0f) {
    
  }

  public void AddLine(Vector3 start, Vector3 end, Material material, float thickness = 1.0f) {
    
  }
  
  void OnFrameRender() {
    
  }

  void OnCameraRender() {
    
  }
}