using UnityEngine;
using Wowsome.Core;
using Wowsome.Generic;

namespace Wowsome.Ads {
  public class AdSystem : MonoBehaviour, ISystem {
    public WObservable<bool> IsNoAds { get; private set; } = new WObservable<bool>();

    IAdsManager[] _adsManagers;

    #region ISystem implementation

    public virtual void InitSystem() {
      _adsManagers = GetComponentsInChildren<IAdsManager>(true);

      foreach (IAdsManager m in _adsManagers) {
        m.InitAdsManager(this);
      }
    }

    public virtual void StartSystem(WEngine gameEngine) {
      gameEngine.OnChangeScene += ev => {
        foreach (IAdsManager m in _adsManagers) {
          m.SceneChanges(ev.Scene);
        }
      };
    }

    public virtual void UpdateSystem(float dt) {
      for (int i = 0; i < _adsManagers.Length; ++i) {
        _adsManagers[i].UpdateAdsManager(dt);
      }
    }

    #endregion

    public T GetManager<T>(bool assertIfNull = true) where T : class, IAdsManager {
      foreach (IAdsManager manager in _adsManagers) {
        T t = manager as T;
        if (null != t) {
          return t;
        }
      }

      if (assertIfNull) Assert.Null<T>(null);

      return null;
    }
  }
}
