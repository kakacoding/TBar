#if UNITY_EDITOR && CUSTOM_TOOLBAR
using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomToolbar.Editor
{
	[Serializable]
	[ToolbarElementDisplay("[分隔]插入间隔", "在功能控件之间插入间隔")]
	internal class ToolbarSeparator : BaseToolbarElement
	{
		[JsonProperty]
		internal int SeparatorPx = DefaultPx;
		internal const int DefaultPx = 5;
		
		private const string StrShow = "间隔像素";
		
		protected override void OnDrawInSettings(VisualElement container)
		{
			base.OnDrawInSettings(container);
			var enableCtrl = container.Q<Toggle>("EnableCtrl");
			if (enableCtrl != null)
			{
				var label = enableCtrl.Q<Label>();
				if (label != null)
				{
					label.style.color = Color.cyan;
				}
			}
			container.Add(IntegerCtrl.Create(
				()=>StrShow,
				()=>SeparatorPx.ToString(),
				v=>
				{
					if (!int.TryParse(v, out SeparatorPx))
					{
						SeparatorPx = DefaultPx;
					}
				},
				new[]{5,10,15}));
		}

		protected override void OnDrawInToolbar(VisualElement container)
		{
			var ve = new VisualElement
			{
				style =
				{
					width = SeparatorPx,
				}
			};
			container.Add(ve);
		}
	}
}
#endif
