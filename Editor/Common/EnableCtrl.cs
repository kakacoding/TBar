#if UNITY_EDITOR && TBAR
using UnityEngine.UIElements;

namespace TBar.Editor
{
    internal class EnableCtrl : VisualElement
    {
        internal delegate string LabelGetter();
        internal delegate bool ToggleGetter();
        internal delegate void ToggleSetter(bool value);
        
        internal static VisualElement Create(LabelGetter labelGetter, ToggleGetter toggleGetter, ToggleSetter toggleSetter)
        {
            var toggleBtn = new Toggle
            {
                name = nameof(EnableCtrl),
                value = toggleGetter.Invoke(),
                label = labelGetter.Invoke()
            };
            toggleBtn.RegisterValueChangedCallback(evt =>
            {
                toggleSetter.Invoke(evt.newValue);
                ToolbarExtender.Reload();
            });
            
            return toggleBtn;
        }
    }
}
#endif