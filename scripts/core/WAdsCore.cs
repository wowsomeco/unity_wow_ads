using System;
using Wowsome.Generic;

namespace Wowsome.Ads {
  public enum AdType {
    Interstitial, Rewarded, Banner
  }

  public interface IAdsProvider {
    int Priority { get; }
    void InitAdsProvider(WAdSystem adSystem);
  }

  public interface IAd {
    WObservable<bool> IsLoaded { get; }
    AdType Type { get; }
    bool ShowAd(Action onDone = null);
    void InitAd(IAdsProvider provider);
    void OnDisabled();
  }
}
