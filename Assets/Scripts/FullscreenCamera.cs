using UnityEngine;

public class FullscreenCamera : MonoBehaviour {
  private Camera m_camera;
  
  private void OnEnable() {
    m_camera = GetComponent<Camera>();
    UpdateCamera();
  }

  private void Update() {
    UpdateCamera();
  }

  private void UpdateCamera() {
    if (!m_camera.orthographic) {
      return;
    }

    m_camera.orthographicSize = Screen.height * 0.5f;
  }
}