﻿using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace Wowsome.Ads {
  using EU = EditorUtils;

  [CustomEditor(typeof(AdMobHelper))]
  public class AdMobHelperEditor : Editor {
    public override void OnInspectorGUI() {
      DrawDefaultInspector();
      AdMobHelper tgt = (AdMobHelper)target;

      EU.VPadding(() => EditorGUILayout.LabelField($"Cur Android Platform: {AppSettings.AndroidPlatform}", EditorStyles.boldLabel));
      EU.Btn("Switch Android Platform", () => {
        SwitchAppSettingsPlatform(tgt);
        SwitchAdmobSettings(tgt);
      });
    }

    void SwitchAppSettingsPlatform(AdMobHelper tgt) {
      string path = tgt.appSettingsPath;
      List<string> lines = File.ReadAllLines(path).ToList();

      string cur = AppSettings.AndroidPlatform == AndroidPlatform.Google ? "AndroidPlatform.Google" : "AndroidPlatform.Amazon";
      string dest = AppSettings.AndroidPlatform == AndroidPlatform.Google ? "AndroidPlatform.Amazon" : "AndroidPlatform.Google";

      int platformIdx = lines.FindIndex(x => x.Contains("AndroidPlatform"));
      lines[platformIdx] = lines[platformIdx].Replace(cur, dest);

      File.WriteAllLines(path, lines);
      EU.Refresh();
    }

    void SwitchAdmobSettings(AdMobHelper tgt) {
      string path = tgt.admobSettingsPath;
      List<string> lines = File.ReadAllLines(path).ToList();

      string googleAppId = tgt.googleAppId.Trim();
      string amazonAppId = tgt.amazonAppId.Trim();
      string cur = AppSettings.AndroidPlatform == AndroidPlatform.Google ? googleAppId : amazonAppId;
      string dest = AppSettings.AndroidPlatform == AndroidPlatform.Google ? amazonAppId : googleAppId;

      int androidAppIdIdx = lines.FindIndex(x => x.Contains("adMobAndroidAppId"));
      lines[androidAppIdIdx] = lines[androidAppIdIdx].Replace(cur, dest);

      File.WriteAllLines(path, lines);
      EU.Refresh();
    }
  }
}