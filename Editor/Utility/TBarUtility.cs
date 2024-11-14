#if UNITY_EDITOR && TBAR
using System.Collections.Generic;
using UnityEngine;

namespace TBar.Editor
{
    internal static partial class TBarUtility
    {
        internal static void LogError(string log)
        {
            Debug.LogError(log);
        }

        public enum ETextTextureDisplay
        {
            TextureText,
            TextTexture,
        }

        public static readonly Dictionary<ETextTextureDisplay, string> TTDisplayMap = new()
        {
            { ETextTextureDisplay.TextureText, "左图右字" },
            { ETextTextureDisplay.TextTexture, "左字右图" },
        };

        public const string LOST_ICON = "Packages/com.kakacoding.tbar/Editor/Resources/Lost.png";
    }
}
#endif