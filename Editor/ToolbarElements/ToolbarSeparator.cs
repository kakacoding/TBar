#if UNITY_EDITOR
using System;
using Newtonsoft.Json;
using UnityEngine.UIElements;

namespace TBar.Editor
{
	[Serializable]
	[ToolbarElementDisplay("d_PauseButton On", "插入间隔", "在功能控件之间插入间隔")]
	internal class ToolbarSeparator : BaseToolbarElement
	{
		[JsonProperty]
		internal int SeparatorPx = DefaultPx;
		internal const int DefaultPx = 5;

		private const string StrShow = "间隔像素";
		
		protected override void OnDrawInSettings(VisualElement container)
		{
			base.OnDrawInSettings(container);
			container.Q<Toggle>("EnableCtrl")?.AddToClassList("EnableCtrl-Separator");
			container.Add(IntegerCtrl.Create(
				() => StrShow,
				() => SeparatorPx,
				v => SeparatorPx = v,
				new[] { 5, 10, 15, 20 }));
		}

		protected override void OnDrawInToolbar(VisualElement container)
		{
			var ve = new VisualElement
			{
				style =
				{
					flexShrink = 1,
					width = SeparatorPx,
				}
			};
			container.Add(ve);
		}
	}
}
#endif
