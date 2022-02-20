using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Wowsome.Ads {
  public class AdInterstitialManager : MonoBehaviour, IAdsManager {
    AdSystem _system;
    List<IInterstitial> _interstitials = new List<IInterstitial>();

    public void Show() {
      if (_system.IsNoAds.Value) return;

      // try show
      foreach (IInterstitial inter in _interstitials) {
        if (inter.ShowInterstitial()) {
          break;
        }
      }
    }

    #region IAdsManager

    public void InitAdsManager(AdSystem adSystem) {
      _system = adSystem;

      if (_system.IsNoAds.Value) return;

      var interstitials = GetComponentsInChildren<IInterstitial>(true);
      foreach (IInterstitial inter in interstitials) {
        inter.InitInterstitial();
        _interstitials.Add(inter);
      }
      // sort by the order
      _interstitials.Sort((x, y) => x.Order < y.Order ? -1 : 1);
    }

    public void SceneChanges(Scene scene) {
    }

    public void UpdateAdsManager(float dt) {
      foreach (IInterstitial inter in _interstitials) {
        inter.UpdateInterstitial(dt);
      }
    }

    #endregion
  }
}
