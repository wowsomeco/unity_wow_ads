using UnityEngine;
using Wowsome.Chrono;
using Wowsome.Core;
using Wowsome.Generic;

namespace Wowsome.Ads {
  public class AdSystem : MonoBehaviour, ISystem {
    IAdsManager[] _adsManagers;
    Timer _delayInit = null;

    public WObservable<bool> IsNoAds { get; private set; } = new WObservable<bool>();

    #region ISystem implementation

    public void InitSystem() {
      _adsManagers = GetComponentsInChildren<IAdsManager>(true);
      // some ads needs to delay the instantiation a tad. 
      _delayInit = new Timer(1f);
    }

    public void StartSystem(CavEngine gameEngine) {
      gameEngine.OnChangeScene += ev => {
        foreach (IAdsManager m in _adsManagers) {
          m.SceneChanges(ev.Scene);
        }
      };
    }

    public void UpdateSystem(float dt) {
      if (null != _delayInit) {
        if (!_delayInit.UpdateTimer(dt)) {
          _delayInit = null;
          InitAdsManager();
        } else {
          return;
        }
      }

      for (int i = 0; i < _adsManagers.Length; ++i) {
        _adsManagers[i].UpdateAdsManager(dt);
      }
    }

    #endregion

    public T GetManager<T>() where T : class, IAdsManager {
      foreach (IAdsManager manager in _adsManagers) {
        T t = manager as T;
        if (null != t) {
          return t;
        }
      }
      return null;
    }

    void InitAdsManager() {
      foreach (IAdsManager m in _adsManagers) {
        m.InitAdsManager(this);
      }
    }
  }
}
