using System;
using UnityEngine;

namespace Wowsome.Ads {
  public class AppLovinInterstitial : MonoBehaviour, IInterstitial {
    [Serializable]
    public struct Model {
      public string UnitId;
      public int ShowOrder;
    }

    public Model Data;

    #region IInterstitial
    public int Order {
      get { return Data.ShowOrder; }
    }

    public bool ShowInterstitial() {
      Debug.Log("show applovin ads intertitial");
#if !UNITY_EDITOR
        AppLovin.ShowInterstitial();
#endif
      return true;
    }

    public void InitInterstitial() {
      Debug.Log("init applovin");
#if !UNITY_EDITOR
        AppLovin.SetSdkKey(Data.UnitId.Trim());
        AppLovin.InitializeSdk();
#endif
    }

    public void UpdateInterstitial(float dt) { }
    #endregion
  }
}
