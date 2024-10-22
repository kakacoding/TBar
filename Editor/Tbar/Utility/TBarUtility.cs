using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TBar.Editor
{
    internal static partial class TBarUtility
    {
        [MenuItem("Tools/1")]
        static void Func1() => EditorUtility.DisplayDialog("测试菜单", "Tools/1", "确定", "取消");
        
        [MenuItem("Tools/2")]
        static void Func2() => EditorUtility.DisplayDialog("测试菜单", "Tools/2", "确定", "取消");
        
        [MenuItem("Tools/3")]
        static void Func3() => EditorUtility.DisplayDialog("测试菜单", "Tools/3", "确定", "取消");
        internal static void LogError(string log)
        {
            Debug.LogError(log);
        }
        private static readonly Dictionary<VisualElement, Dictionary<Type, Delegate>> VisualElementEventCache = new ();

        internal static void RegisterCallback<T>(this VisualElement visualElement, EventCallback<T> callback) where T : EventBase<T>, new()
        {
            if (!VisualElementEventCache.TryGetValue(visualElement, out var eventDictionary))
            {
                eventDictionary = new Dictionary<Type, Delegate>();
                VisualElementEventCache[visualElement] = eventDictionary;
            }

            if (eventDictionary.TryGetValue(typeof(T), out var existingCallback))
            {
                visualElement.UnregisterCallback((EventCallback<T>)existingCallback);
            }

            visualElement.RegisterCallback(callback);
            eventDictionary[typeof(T)] = callback;
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

        

        public const string LOST_ICON = "Packages/com.kakacoding.tbar/Editor/TBar/Resources/Lost.png";
    }
}