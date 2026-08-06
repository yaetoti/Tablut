using UnityEngine;

public abstract class SceneSingleton<T> : MonoBehaviour where T : SceneSingleton<T> {
  public static T Instance { get; private set; }

  // static SceneSingleton() {
  //   DomainReloadRegistry.OnReload += () => { Instance = null; };
  // }
  
  protected virtual void Awake() {
    if (Instance == this) {
      return;
    }

    if (Instance is not null) {
      Destroy(this);
      return;
    }

    Instance = this as T;
  }

  protected virtual void OnDestroy() {
    if (Instance == this) {
      Instance = null;
    }
  }
}