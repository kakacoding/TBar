using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CustomToolbar.Editor
{
    internal class IntegerCtrl : VisualElement
    {
        internal delegate string LabelGetter();
        internal delegate string TextGetter();
        internal delegate void TextSetter(string value);
        
        internal static VisualElement Create(LabelGetter labelGetter, TextGetter textGetter, TextSetter textSetter, int[] ints)
        {
            var integerField = new IntegerField(labelGetter.Invoke())
            {
                name = nameof(IntegerCtrl),
                value = string.IsNullOrEmpty(textGetter())?0:int.TryParse(textGetter(), out var n) ? n : 0,
            };
            integerField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue < 0)
                {
                    integerField.value = 0;
                }
                
                textSetter.Invoke(evt.newValue.ToString());
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