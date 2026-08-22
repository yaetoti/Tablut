using System;
using UnityEngine;

public static class DomainReloadRegistry {
  public static event Action OnReload;

  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
  private static void OnDomainReload() {
    OnReload?.Invoke();
  }
}