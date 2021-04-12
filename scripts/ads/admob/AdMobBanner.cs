using System;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;

namespace Wowsome.Ads {
  public class AdMobBanner : MonoBehaviour, IBanner {
    [Serializable]
    public struct Model {
      public string UnitIdIOS;
      public string AppIdIOS;
      public string UnitIdAndroid;
      public string AppIdAndroid;
      public AdPosition Position;

      public string UnitId {
        get {
          string unitId = Application.platform == RuntimePlatform.Android ? UnitIdAndroid : UnitIdIOS;
          return unitId.Trim();
        }
      }

      public string AppId {
        get {
          string appId = Application.platform == RuntimePlatform.Android ? AppIdAndroid : AppIdIOS;
          return appId.Trim();
        }
      }
    }

    public Model Data;

    BannerView m_banner;
    bool m_loading = false;
    bool m_loaded = false;
    Action m_onLoaded;

    public void InitBanner(Action onLoaded) {
      m_onLoaded = onLoaded;
      LoadBanner();
    }

    public void ShowBanner(bool flag) {
      // this stupid banner seems unable to toggle visibility,
      // the workaround is when hiding, we destroy it
      // then reload it again
      if (!flag) {
        if (m_loaded) {
          m_banner.Hide();
          m_banner.Destroy();
          m_loaded = false;
        }
      } else {
        if (m_loaded) {
          m_banner.Show();
        } else if (!m_loading) {
          LoadBanner();
        }
      }
    }

    void LoadBanner() {
      m_loading = true;
      MobileAds.Initialize(OnInit);
    }

    void OnInit(InitializationStatus initstatus) {
      // Callbacks from GoogleMobileAds are not guaranteed to be called on
      // main thread.
      // In this example we use MobileAdsEventExecutor to schedule these calls on
      // the next Update() loop.
      MobileAdsEventExecutor.ExecuteInUpdate(() => {
        //load banner
        AdRequest request = new AdRequest.Builder().Build();
        m_banner = new BannerView(Data.UnitId, AdSize.SmartBanner, Data.Position);
        m_banner.OnAdLoaded += (sender, args) => {
          m_loading = false;
          m_loaded = true;
          m_onLoaded();
        };
        m_banner.OnAdFailedToLoad += (sender, e) => {
          // reload if failed
          LoadBanner();
        };
        m_banner.LoadAd(request);
      });
    }
  }
}
