#if UNITY_EDITOR
using UnityEngine.UIElements;

namespace TBar.Editor
{
    public class ToggleCtrl : VisualElement
    {
        public delegate string LabelGetter();
        public delegate string TooltipGetter();
        public delegate bool ToggleGetter();
        public delegate void ToggleSetter(bool value);
        
        public static VisualElement Create(LabelGetter labelGetter, ToggleGetter toggleGetter, ToggleSetter toggleSetter, TooltipGetter tooltipGetter = null)
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