using UnityEngine;

public abstract class SceneSingleton<T> : MonoBehaviour where T : SceneSingleton<T> {
  public static T Instance { get; private set; }

  // static SceneSingleton() {
  //   DomainReloadRegistry.OnReload += () => { Instance = null; };
  // }
  
  private void Awake() {
    if (Instance == this) {
      return;
    }

    if (Instance is not null) {
      Destroy(this);
      return;
    }

    Instance = this as T;
    Initialize();
  }

  private void OnDestroy() {
    if (Instance != this) {
      return;
    }

    Instance = null;
    Cleanup();
  }

  protected abstract void Initialize();
  protected abstract void Cleanup();
}