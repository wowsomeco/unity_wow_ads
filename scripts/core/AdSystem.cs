using UnityEngine;
using Wowsome.Chrono;
using Wowsome.Core;

namespace Wowsome.Ads {
  public class AdSystem : MonoBehaviour, ISystem {
    IAdsManager[] _adsManagers;
    Timer _delayInit = null;

    public bool IsNoAds { get; set; }

    #region ISystem implementation

    public void InitSystem() {
      _adsManagers = GetComponentsInChildren<IAdsManager>(true);
      _delayInit = new Timer(2f);
    }

    public void StartSystem(CavEngine gameEngine) {
      gameEngine.OnChangeScene += scene => {
        foreach (IAdsManager m in _adsManagers) {
          m.SceneChanges(scene);
        }
      };
    }

    public void UpdateSystem(float dt) {
      if (null != _delayInit && !_delayInit.UpdateTimer(dt)) {
        _delayInit = null;
        InitAdsManager();
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
