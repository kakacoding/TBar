#if UNITY_EDITOR
using UnityEngine.UIElements;

namespace TBar.Editor
{
    public class TextFieldCtrl : VisualElement
    {
        public delegate string LabelGetter();
        public delegate string TextGetter();
        public delegate void TextSetter(string value);
        
        public static VisualElement Create(LabelGetter labelGetter, TextGetter textGetter, TextSetter textSetter)
        {
            var textField = new TextField(labelGetter.Invoke())
            {
                name = nameof(TextFieldCtrl),
                value = textGetter(),
            };
            textField.RegisterValueChangedCallback(evt =>
            {
                textSetter(evt.newValue);
                ToolbarExtender.Reload();
            });
            
            return textField;
        }
    }
}
#endif