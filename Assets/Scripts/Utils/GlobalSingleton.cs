using UnityEngine;

public abstract class GlobalSingleton<T> : MonoBehaviour where T : GlobalSingleton<T> {
  public static T Instance { get; private set; }

  static GlobalSingleton() {
    DomainReloadRegistry.OnReload += () => { Instance = null; };
  }
  
  protected virtual void Awake() {
    if (Instance == this) {
      return;
    }

    if (Instance is not null) {
      Destroy(this);
      return;
    }

    Instance = this as T;
    DontDestroyOnLoad(this);
  }

  protected virtual void OnDestroy() {
    if (Instance == this) {
      Instance = null;
    }
  }
}