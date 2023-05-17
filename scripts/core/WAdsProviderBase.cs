using System.Collections.Generic;
using UnityEngine;

namespace Wowsome.Ads {
  public abstract class WAdsProviderBase : MonoBehaviour, IAdsProvider {
    public bool IsTestMode => isTestMode;
    public abstract string Id { get; }

    public bool isDisabled;
    public bool isTestMode;

    protected WAdSystem _adSystem;
    protected List<IAd> _ads = new List<IAd>();

    public IAd GetAdByType(AdType adType) {
      return _ads.Find(x => x.Type == adType);
    }

    public virtual void InitAdsProvider(WAdSystem adSystem) {
      _adSystem = adSystem;

      _ads = GetComponentsInChildren<IAd>(true).ToList();

      _adSystem.IsDisabled.Subscribe(flag => {
        if (flag) {
          _ads.ForEach(ad => ad.OnDisabled());
        }
      });
    }

    protected void InitAds() {
      _adSystem.OnAdManagerInitialized?.Invoke(Id);

      for (int i = 0; i < _ads.Count; ++i) {
        IAd ad = _ads[i];

        ad.IsLoaded.Subscribe(flag => {
          if (flag) {
            _adSystem.OnAdLoaded?.Invoke(ad);
          } else {
            _adSystem.OnAdUnloaded?.Invoke(ad);
          }
        });

        ad.InitAd(this);
      }
    }
  }
}