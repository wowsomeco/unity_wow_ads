using System;
using UnityEngine.SceneManagement;

namespace Wowsome.Ads {
  public delegate void OnSuccessShowingAd();

  #region Interstitial

  public interface IInterstitial {
    int Order { get; }
    void InitInterstitial();
    bool ShowInterstitial();
    void UpdateInterstitial(float dt);
  }

  #endregion

  #region Rewarded

  public interface IReward {
    int Order { get; }
    bool ShowReward(IRewardAdListener listener);
  }

  public interface IRewardAdListener {
    void OnRewarded();
  }

  #endregion

  #region Banner

  public interface IBanner {
    void InitBanner(Action onLoaded);
    void ShowBanner(bool flag);
    void UpdateBanner(float dt);
  }

  #endregion

  public interface IAdsManager {
    void InitAdsManager(AdSystem adSystem);
    void SceneChanges(Scene curScene);
    void UpdateAdsManager(float dt);
  }
}
