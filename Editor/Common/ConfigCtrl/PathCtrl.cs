#if UNITY_EDITOR && TBAR
using UnityEditor;
using UnityEngine.UIElements;

namespace TBar.Editor
{
    internal class PathCtrl : VisualElement
    {
        internal delegate string LabelGetter();
        internal delegate string TextGetter();
        internal delegate void TextSetter(string value);
        
        internal static VisualElement Create(LabelGetter labelGetter, TextGetter textGetter, TextSetter textSetter)
        {
            var textField = new TextField(labelGetter.Invoke())
            {
                name = nameof(PathCtrl),
                value = textGetter(),
            };
            textField.RegisterValueChangedCallback(evt =>
            {
                textSetter.Invoke(evt.newValue);
                ToolbarExtender.Reload();
            });
            var btnBrowser = new Button(() =>
            {
                var path = EditorUtility.OpenFilePanel("选择文件", "", "");
                if (!string.IsNullOrEmpty(path))
                {
                    textField.value = path;
                }
            })
            {
                text = "选择文件"
            };
            textField.Add(btnBrowser);
            
            return textField;
        }
    }
}
#endif