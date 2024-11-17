#if UNITY_EDITOR && TBAR
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TBar.Editor
{
    public class TextureTextCtrl : VisualElement
    {
        internal delegate TBarUtility.ETextTextureDisplay DisplayGetter();
        internal delegate void DisplaySetter(TBarUtility.ETextTextureDisplay value);
        internal delegate string TextGetter();
        internal delegate void TextSetter(string value);
        internal delegate string TextureGetter();
        internal delegate void TextureSetter(string value);
        
        internal static VisualElement Create(DisplayGetter displayGetter, DisplaySetter displaySetter, 
            TextGetter textGetter, TextSetter textSetter, TextureGetter textureGetter, TextureSetter textureSetter)
        {
            var container = new VisualElement
            {
                name = nameof(TextureTextCtrl)
            };

            var enumField = new PopupField<string>(TBarUtility.TTDisplayMap.Values.ToList(), 0);
            enumField.RegisterValueChangedCallback(evt =>
            {
                var newValue = evt.newValue;
                displaySetter(TBarUtility.TTDisplayMap.First(kv => kv.Value.Equals(newValue)).Key);
                var textureTextContainer = container.Children().FirstOrDefault(child => child.name.Equals("TextureTextContainer"));
                if (textureTextContainer != null)
                {
                    textureTextContainer.style.flexDirection = displayGetter() == TBarUtility.ETextTextureDisplay.TextureText ? FlexDirection.Row : FlexDirection.RowReverse;
                }
                ToolbarExtender.Reload();
            });
            container.Add(enumField);

            var textureTextContainer = new VisualElement
            {
                name = "TextureTextContainer"
            };
            
            var objField = new ObjectField
            {
                objectType = typeof(Texture2D),
                value = AssetDatabase.LoadAssetAtPath<Texture2D>(textureGetter()),
            };
            objField.RegisterValueChangedCallback(evt =>
            {
                textureSetter(evt.newValue != null ? AssetDatabase.GetAssetPath(evt.newValue) : "");
                ToolbarExtender.Reload();
            });
            textureTextContainer.Add(objField);
            
            var txtBtnText = new TextField
            {
                value = textGetter(),
            };
            txtBtnText.RegisterValueChangedCallback(evt =>
            {
                textSetter(evt.newValue);
                ToolbarExtender.Reload();
            });
            textureTextContainer.Add(txtBtnText);
            
            container.Add(textureTextContainer);
            return container;
        }
    }
}
#endif