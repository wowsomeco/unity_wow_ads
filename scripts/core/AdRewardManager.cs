﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wowsome.Chrono;

namespace Wowsome.Ads {
  public class AdRewardManager : MonoBehaviour, IAdsManager {
    public bool HasInitialized { get; private set; }

    List<IReward> _rewards = new List<IReward>();
    Timer _rewardedTimer;
    AdSystem _system;
    Action _onRewarded;

    public void Show(Action onRewarded) {
      // try show
      if (_rewards.Count > 0) {
        _onRewarded = onRewarded;

        foreach (IReward reward in _rewards) {
          if (reward.ShowReward()) {
            break;
          }
        }
      }
    }

    #region IAdsManager

    public void InitAdsManager(AdSystem adSystem) {
      _system = adSystem;

      if (_system.IsNoAds.Value) return;

      var rewards = GetComponentsInChildren<IReward>(true);
      foreach (IReward reward in rewards) {
        reward.InitReward();
        reward.OnRewarded += () => {
          _rewardedTimer = new Timer(.1f);
        };
        _rewards.Add(reward);
      }
      // sort by the order
      _rewards.Sort((x, y) => x.Order < y.Order ? -1 : 1);
    }

    public void UpdateAdsManager(float dt) {
      if (null != _rewardedTimer && !_rewardedTimer.UpdateTimer(dt)) {
        _rewardedTimer = null;
        _onRewarded?.Invoke();
        _onRewarded = null;
      }
    }

    public void SceneChanges(Scene curScene) { }

    #endregion    
  }
}