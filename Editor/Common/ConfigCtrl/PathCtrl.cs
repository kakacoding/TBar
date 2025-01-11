#if UNITY_EDITOR
using System;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace TBar.Editor
{
    public class PathCtrl : VisualElement
    {
        public delegate string LabelGetter();
        public delegate string TextGetter();
        public delegate void TextSetter(string value);
        public delegate string ButtonTextGetter();
        
        public static VisualElement Create(LabelGetter labelGetter, TextGetter textGetter, TextSetter textSetter, ButtonTextGetter buttonTextGetter,  Func<Task<string>> onClick)
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
            var btnBrowser = new Button(async () =>
            {
                var path = await onClick.Invoke(); // 异步获取值（支持同步和异步）
                if (!string.IsNullOrEmpty(path))
                {
                    textField.value = path; // 更新 TextField
                }
            })
            {
                text = buttonTextGetter()
            };
            textField.Add(btnBrowser);
            
            return textField;
        }
    }
}
#endif