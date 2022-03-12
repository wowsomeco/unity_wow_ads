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

    int _curIdx = 0;
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

      IAd curAd = _ads[_curIdx];
      if (curAd.IsLoaded.Value) {
        curAd.ShowAd(onDone);

        _curIdx = GetNextIdx();

        return true;
      } else {
        // if next idx is not the current ad that is trying to show...
        if (GetNextIdx() != _curIdx) {
          return Show(onDone);
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

    int GetNextIdx() {
      int next = _curIdx + 1;

      if (next >= _ads.Count) next = 0;

      return next;
    }
  }
}
