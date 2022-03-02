using System;
using System.Collections.Generic;
using UnityEngine;
using Wowsome.Core;
using Wowsome.Generic;

namespace Wowsome.Ads {
  public class WAdSystem : MonoBehaviour, ISystem {
    public Action<IAd> OnAdLoaded { get; set; }
    public Action<IAd> OnAdUnloaded { get; set; }
    public WObservable<bool> IsDisabled { get; private set; } = new WObservable<bool>();

    List<WAdsProviderBase> _adsProviders = new List<WAdsProviderBase>();

    public bool IsAvailable(AdType type) {
      return GetLoadedAd(type) != null;
    }

    public IAd GetLoadedAd(AdType type) {
      foreach (WAdsProviderBase ap in _adsProviders) {
        if (ap.IsInitialized.Value && ap.GetAdByType(type) is IAd ad) {
          if (ad.IsLoaded.Value) {
            return ad;
          }
        }
      }

      return null;
    }

    public bool Show(AdType type, Action onComplete = null) {
      if (GetLoadedAd(type) is IAd ad) {
        Print.Log(() => "cyan", "Show Ad:", ad, type);

        return ad.ShowAd(onComplete);
      }

      return false;
    }

    public bool IsLoaded(AdType type) {
      if (GetLoadedAd(type) is IAd ad) return true;

      return false;
    }

    public T GetProvider<T>(bool assertIfNull = true) where T : class, IAdsProvider {
      foreach (WAdsProviderBase provider in _adsProviders) {
        T t = provider as T;
        if (null != t) {
          return t;
        }
      }

      if (assertIfNull) Assert.Null<T>(null);

      return null;
    }

    #region ISystem implementation

    public virtual void InitSystem() { }

    public virtual void StartSystem(WEngine gameEngine) {
      _adsProviders = gameObject.GetComponentsInChildrenWithCallback<WAdsProviderBase>(true, ap => {
        ap.InitAdsProvider(this);
      });

      // sort by priority
      _adsProviders.Sort((x, y) => x.Priority < y.Priority ? -1 : 1);
    }

    public virtual void UpdateSystem(float dt) { }

    #endregion    
  }
}
