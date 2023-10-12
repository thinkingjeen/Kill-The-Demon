using System.Collections.Generic;
using UnityEngine;

namespace MobileConsole
{
    internal class StyleSettings : ScriptableObject
    {
        public Color LightBackgroundColor = Color.black;
        public Color DarkBackgroundColor = Color.black;
        public Color SelectedBackgroundColor = Color.black;
        public Color TextColor = Color.black;
        public Color SelectedTextColor = Color.black;
        public Color ScrollbarBackgroundColor = Color.black;
        public Color ScrollbarHandleColor = Color.black;
        public Color ScrollBackgroundColor = Color.black;
        public Sprite LogBackgroundSprite = null;
        public Color LogCountBackgroundColor = Color.black;
    }
}