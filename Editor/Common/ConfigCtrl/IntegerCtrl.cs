#if UNITY_EDITOR && TBAR
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace TBar.Editor
{
    public class IntegerCtrl : VisualElement
    {
        internal delegate string LabelGetter();
        internal delegate int TextGetter();
        internal delegate void TextSetter(int value);
        
        internal static VisualElement Create(LabelGetter labelGetter, TextGetter textGetter, TextSetter textSetter, int[] ints)
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