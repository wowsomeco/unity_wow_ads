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
    Dictionary<AdType, AdPool> _adPools = new Dictionary<AdType, AdPool> {
      { AdType.Banner, new AdPool(AdType.Banner) },
      { AdType.Interstitial, new AdPool(AdType.Interstitial) },
      { AdType.Rewarded, new AdPool(AdType.Rewarded) }
    };

    public bool IsAvailable(AdType type) {
      return _adPools[type].GetAvailableAd() != null;
    }

    public bool Show(AdType type, Action onComplete = null) {
      if (IsDisabled.Value) return false;

      return _adPools[type].Show(onComplete);
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
      OnAdLoaded += ad => {
        _adPools[ad.Type].Add(ad);
      };

      _adsProviders = gameObject.GetComponentsInChildrenWithCallback<WAdsProviderBase>(true, ap => {
        ap.InitAdsProvider(this);
      });
    }

    public virtual void UpdateSystem(float dt) { }

    #endregion    
  }
}
