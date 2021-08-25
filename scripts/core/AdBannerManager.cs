using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Wowsome.Ads {
  /// <summary>
  /// Handles visibility of the banner ad for each scene in the game.
  /// </summary>
  public class AdBannerManager : MonoBehaviour, IAdsManager {
    [Serializable]
    public class SceneVisibility {
      /// <summary>
      /// the unity scene where the banner needs to show / hide
      /// </summary>
      public string sceneName;
      public bool flag;

      public bool Matches(Scene scene) {
        return sceneName.CompareStandard(scene.name);
      }
    }

    public List<SceneVisibility> visibilities = new List<SceneVisibility>();
    /// <summary>
    /// on change scene, 
    /// if CurVisibility returns null and this is true, 
    /// then banner will show / hide according to toggleFlag below
    /// </summary>
    public bool hasGlobalToggle;
    [ConditionalHide("hasGlobalToggle", false)]
    public bool toggleFlag;

    AdSystem _system;
    IBanner _banner = null;
    bool _bannerLoaded = false;
    bool _bannerShowing = false;
    Scene _curScene;

    public bool ShouldShowBanner => _bannerLoaded && !_system.IsNoAds.Value;

    public SceneVisibility CurVisibility => visibilities.Find(x => x.Matches(_curScene));

    #region IAdsManager

    public void InitAdsManager(AdSystem adSystem) {
      _system = adSystem;
      if (_system.IsNoAds.Value) return;
      // observe IsNoAds value, 
      // whenever it's true (normally when the ads have just been removed),
      // we hide the banner
      _system.IsNoAds.Subscribe(flag => {
        if (flag) ShowBanner(false);
      });

      _banner = GetComponent<IBanner>();
      Assert.Null(_banner, "cant find IBanner component in AdBannerManager");
      _banner.InitBanner(() => {
        _bannerLoaded = true;
        TryShowOrHideBanner();
      });
    }

    public void SceneChanges(Scene scene) {
      _curScene = scene;
      TryShowOrHideBanner();
    }

    public void UpdateAdsManager(float dt) {
      _banner?.UpdateBanner(dt);
    }

    #endregion 

    void ShowBanner(bool flag) {
      if (!_bannerLoaded || (_bannerShowing == flag)) return;

      _bannerShowing = flag;
      _banner.ShowBanner(flag);
    }

    void TryShowOrHideBanner() {
      // force hide, just in case it's still showing and it shouldn't show
      if (!ShouldShowBanner) {
        ShowBanner(false);
        return;
      }

      SceneVisibility visibility = CurVisibility;
      if (null != visibility) {
        ShowBanner(visibility.flag);
      } else if (hasGlobalToggle) {
        ShowBanner(toggleFlag);
      }
    }
  }
}