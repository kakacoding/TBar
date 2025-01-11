#if UNITY_EDITOR
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace TBar.Editor
{
    public class IntegerCtrl : VisualElement
    {
        public delegate string LabelGetter();
        public delegate int TextGetter();
        public delegate void TextSetter(int value);
        
        public static VisualElement Create(LabelGetter labelGetter, TextGetter textGetter, TextSetter textSetter, int[] ints)
        {
            var integerField = new IntegerField(labelGetter.Invoke())
            {
                name = nameof(IntegerCtrl),
                value = textGetter(),
            };
            integerField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue < 0)
                {
                    integerField.value = 0;
                }
                
                textSetter.Invoke(evt.newValue);
                ToolbarExtender.Reload();
            });
            if (ints is { Length: > 0 })
            {
                foreach (var num in ints)
                {
                    var btn = new Button(() =>
                    {
                        integerField.value = num;
                    })
                    {
                        text = num.ToString()
                    };
                    integerField.Add(btn);
                }
            }
            
            return integerField;
        }
    }
}
#endif