#if UNITY_EDITOR && TBAR
using UnityEngine.UIElements;

namespace TBar.Editor
{
    internal class ToggleCtrl : VisualElement
    {
        internal delegate string LabelGetter();
        internal delegate string TooltipGetter();
        internal delegate bool ToggleGetter();
        internal delegate void ToggleSetter(bool value);
        
        internal static VisualElement Create(LabelGetter labelGetter, ToggleGetter toggleGetter, ToggleSetter toggleSetter, TooltipGetter tooltipGetter = null)
        {
            var toggleBtn = new Toggle
            {
                name = nameof(ToggleCtrl),
                value = toggleGetter.Invoke(),
                label = labelGetter?.Invoke(),
                tooltip = tooltipGetter?.Invoke()
            };
            toggleBtn.RegisterValueChangedCallback(evt =>
            {
                toggleSetter?.Invoke(evt.newValue);
                ToolbarExtender.Reload();
            });
            
            return toggleBtn;
        }
    }
}
#endif