using System;
using UnityEngine;

public static class DomainReloadRegistry {
  public static Action OnReload { get; set; }

  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
  private static void OnDomainReload() {
    OnReload?.Invoke();
  }
}