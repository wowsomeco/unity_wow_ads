using System;
using UnityEngine;
using UnityEngine.Advertisements;

namespace Wowsome.Ads {
  public class UnityAdInterstitial : MonoBehaviour, IInterstitial {
    [Serializable]
    public struct Model {
      public string GameIdIOS;
      public string GameIdAndroid;
      public int ShowOrder;

      public string GameId {
        get {
          string gameId = Application.platform == RuntimePlatform.Android ? GameIdAndroid : GameIdIOS;
          return gameId.Trim();
        }
      }
    }

    public Model Data;

    #region IInterstitial
    public int Order {
      get { return Data.ShowOrder; }
    }

    public void InitInterstitial() {
      Advertisement.Initialize(Data.GameId);
    }

    public void UpdateInterstitial(float dt) { }

    public bool ShowInterstitial() {
      if (Advertisement.IsReady()) {
        Advertisement.Show();
        return true;
      }
      return false;
    }
    #endregion
  }
}
