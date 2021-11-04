using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class DisplaySettingsData{
    public class DisplaySettingSelector{
        public const string NONE = "NONE";
        public const string ALL = "ALL";
        public const string SELECTED = "SELECTED";
    }
    public string showScoresEvents = DisplaySettingSelector.SELECTED;
    public string showDebugLines = DisplaySettingSelector.SELECTED;
    public bool entityShadows = true;
    public bool tileShadows = true;
    public bool particles = true;

}
