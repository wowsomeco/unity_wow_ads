using System;
using System.Collections.Generic;
using Wowsome.Generic;

namespace Wowsome.Ads {
  public enum AdType {
    Interstitial, Rewarded, Banner
  }

  public interface IAdsProvider {
    bool IsTestMode { get; }
    void InitAdsProvider(WAdSystem adSystem);
  }

  public interface IAd {
    int Priority { get; }
    WObservable<bool> IsLoaded { get; }
    AdType Type { get; }
    bool ShowAd(Action onDone = null);
    void InitAd(IAdsProvider provider);
    void OnDisabled();
  }

  public class AdPool {
    public AdType Type { get; private set; }

    public AdPool(AdType t) {
      Type = t;
    }

    List<IAd> _ads = new List<IAd>();

    public IAd GetAvailableAd() {
      foreach (IAd ad in _ads) {
        if (ad.IsLoaded.Value) {
          return ad;
        }
      }

      return null;
    }

    public bool Show(Action onDone = null) {
      // bail if nothing to show yet
      if (_ads.Count == 0) return false;

      _ads.Sort((x, y) => x.Priority < y.Priority ? -1 : 1);

      foreach (IAd ad in _ads) {
        if (ad.IsLoaded.Value) {
          ad.ShowAd(onDone);

          return true;
        }
      }

      return false;
    }

    public bool Add(IAd ad) {
      if (ad.Type == Type) {
        _ads.Add(ad);
        return true;
      }

      return false;
    }
  }
}
