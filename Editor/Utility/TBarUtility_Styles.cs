#if UNITY_EDITOR && TBAR
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
        
        public static readonly GUIStyle DropDownStyle = new("DropDown")
        {
            fontSize = 12,
            stretchWidth = true,
            fixedWidth = 0,
            fixedHeight = 26,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            imagePosition = ImagePosition.ImageLeft,
        };
        public static readonly GUIStyle CommandStyle = new("AppCommand")
        {
            fontSize = 12,
            stretchWidth = true,
            fixedWidth = 0,
            fixedHeight = 22,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            imagePosition = ImagePosition.ImageLeft,
        };
        public static readonly GUIStyle CommandLeftOnStyle = new("AppCommandLeftOn")
        {
            fontSize = 12,
            stretchWidth = true,
            fixedWidth = 0,
            fixedHeight = 22,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            imagePosition = ImagePosition.ImageLeft,
        };
        public static readonly GUIStyle CommandLeftStyle = new("AppCommandLeft")
        {
            fontSize = 12,
            stretchWidth = true,
            fixedWidth = 0,
            fixedHeight = 22,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            imagePosition = ImagePosition.ImageLeft,
        };
        public static readonly GUIStyle CommandMidStyle = new("AppCommandMid")
        {
            fontSize = 12,
            stretchWidth = true,
            fixedWidth = 0,
            fixedHeight = 22,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            imagePosition = ImagePosition.ImageLeft
        };
        public static readonly GUIStyle commandRightStyle = new("AppCommandRight")
        {
            fontSize = 12,
            stretchWidth = true,
            fixedWidth = 0,
            fixedHeight = 22,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            imagePosition = ImagePosition.ImageLeft
        };
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
