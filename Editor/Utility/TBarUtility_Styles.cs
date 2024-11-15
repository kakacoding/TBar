﻿#if UNITY_EDITOR && TBAR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace TBar.Editor
{
    internal static partial class TBarUtility
    {
        private const string PACKAGE_NAME = "com.kakacoding.tbar";
        private static readonly List<string> StyleFiles = new() {$"Packages/{PACKAGE_NAME}/Editor/UI/Styles.uss"};

        internal static void AttachStyles(VisualElement visualElement)
        {
            if (visualElement == null) return;
            StyleFiles.ForEach(styleFile =>
            {
                if (!File.Exists(styleFile)) return;
                var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(styleFile);
                if (styleSheet != null)
                {
                    visualElement.styleSheets.Add(styleSheet);         
                }
            });
        }
    }

    internal class ToolbarElementDisplay : Attribute 
    {
        public string Display;
        public string Detail;

        public ToolbarElementDisplay(string display, string detail)
        {
            Display = display;
            Detail = detail;
        }

        public static string GetDisplay(Type t)
        {
            var attr = GetCustomAttribute(t, typeof(ToolbarElementDisplay)) as ToolbarElementDisplay;
            return attr?.Display ?? "";
        }
        
        public static string GetDetail(Type t)
        {
            var attr = GetCustomAttribute(t, typeof(ToolbarElementDisplay)) as ToolbarElementDisplay;
            return attr?.Detail ?? "";
        }
        
        public static string GetHelp(Type t)
        {
            return GetCustomAttribute(t, typeof(ToolbarElementDisplay)) is ToolbarElementDisplay attr ? $"{attr.Display}\n{attr.Detail}" : "";
        }
    }
}
#endif