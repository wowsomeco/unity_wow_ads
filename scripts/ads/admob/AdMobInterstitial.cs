using System;
using GoogleMobileAds.Api;
using UnityEngine;
using Wowsome.Chrono;

namespace Wowsome.Ads {
  public class AdMobInterstitial : MonoBehaviour, IInterstitial {
    [Serializable]
    public struct Model {
      public string UnitIdIOS;
      public string UnitIdAndroid;
      public int ShowOrder;

      public string UnitId {
        get {
          string unitId = Application.platform == RuntimePlatform.Android ? UnitIdAndroid : UnitIdIOS;
          return unitId.Trim();
        }
      }
    }

    public Model Data;

    InterstitialAd m_interstitial;
    Timer m_delayLoad = null;

    #region IInterstitial
    public int Order {
      get { return Data.ShowOrder; }
    }

    public bool ShowInterstitial() {
      if (null != m_delayLoad) return false;

      if (m_interstitial.IsLoaded()) {
        Debug.Log("show admob ads");
        m_interstitial.Show();
        return true;
      }
      ReloadAd();
      return false;
    }
    #endregion

    public void InitInterstitial() {
      // load ad
      ReloadAd();
    }

    public void UpdateInterstitial(float dt) {
      if (null != m_delayLoad && !m_delayLoad.UpdateTimer(dt)) {
        m_delayLoad = null;
        ReloadAd();
      }
    }

    void InitInternal() {
      m_interstitial = new InterstitialAd(Data.UnitId);
      // because admob is soo special, you need to delay reload
      // otherwise they will complain with their Too many recently failed requests on some random ios devices.
      m_interstitial.OnAdClosed += (object sender, System.EventArgs e) => {
        m_delayLoad = new Timer(3f);
      };
    }

    void ReloadAd() {
      Debug.Log("load admob ads");
#if UNITY_ANDROID
        if (null == m_interstitial) InitInternal();
#elif UNITY_IPHONE
      // re init intestitial on ios because it's a one time object
      InitInternal();
#endif
      // Create an empty ad request.
      AdRequest request = new AdRequest.Builder().Build();
      // Load the interstitial with the request.
      m_interstitial.LoadAd(request);
    }
  }
}
