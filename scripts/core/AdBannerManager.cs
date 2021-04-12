using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Wowsome.Ads {
  /// <summary>
  /// WIP
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

    public bool ShouldShowBanner => _bannerLoaded && !_system.IsNoAds;

    public SceneVisibility CurVisibility => visibilities.Find(x => x.Matches(_curScene));

    #region IAdsManager

    public void InitAdsManager(AdSystem adSystem) {
      _system = adSystem;

      _banner = GetComponent<IBanner>();
      _banner?.InitBanner(() => {
        _bannerLoaded = true;
        TryShowOrHideBanner();
      });
    }

    public void SceneChanges(Scene scene) {
      _curScene = scene;
      TryShowOrHideBanner();
    }

    public void UpdateAdsManager(float dt) { }

    #endregion 

    void ShowBanner(bool flag) {
      if (!_bannerLoaded) return;

      _bannerShowing = flag;
      _banner.ShowBanner(flag);
    }

    void TryShowOrHideBanner() {
      SceneVisibility visibility = CurVisibility;
      if (null != visibility) {
        ShowBanner(visibility.flag);
      } else if (hasGlobalToggle) {
        ShowBanner(toggleFlag);
      }
    }
  }
}