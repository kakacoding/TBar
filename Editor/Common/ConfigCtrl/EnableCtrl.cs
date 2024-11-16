#if UNITY_EDITOR && TBAR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TBar.Editor
{
    internal class EnableCtrl : VisualElement
    {
        internal delegate string IconNameGetter();
        internal delegate string LabelGetter();
        internal delegate string TooltipGetter();
        internal delegate bool ToggleGetter();
        internal delegate void ToggleSetter(bool value);
        
        internal static VisualElement Create(IconNameGetter iconNameGetter, LabelGetter labelGetter, ToggleGetter toggleGetter, ToggleSetter toggleSetter, TooltipGetter tooltipGetter = null)
        {
            var toggleBtn = new Toggle
            {
                name = nameof(EnableCtrl),
                value = toggleGetter.Invoke(),
                label = labelGetter?.Invoke(),
                tooltip = tooltipGetter?.Invoke(),
                pickingMode = PickingMode.Ignore,
                labelElement =
                {
                    pickingMode = PickingMode.Ignore
                }
            };
            var icon = new VisualElement
            {
                name = "Icon",
                pickingMode = PickingMode.Ignore,
            };
            var iconName = iconNameGetter();
            if (!string.IsNullOrEmpty(iconName))
            {
                icon.style.backgroundImage = new StyleBackground(EditorGUIUtility.Load(iconNameGetter()) as Texture2D);
            }
            toggleBtn.Insert(1, icon);
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