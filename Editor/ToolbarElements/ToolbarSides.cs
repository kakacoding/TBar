#if UNITY_EDITOR && TBAR
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace TBar.Editor
{
	[Serializable]
	[ToolbarElementDisplay("[分隔]左右区域分割", "在本项目以上的条目在运行按钮的左边区域，以下的项目在右边")]
	internal class ToolbarSides : BaseToolbarElement
	{
		protected override void OnDrawInSettings(VisualElement container)
		{
			base.OnDrawInSettings(container);
			var enableCtrl = container.Q<Toggle>("EnableCtrl");
			if (enableCtrl != null)
			{
				var label = enableCtrl.Q<Label>();
				if (label != null)
				{
					label.style.color = Color.green;
				}
			}
			container.Add(new Label
			{
				text = "-------------------------------------------------------------------------------------"
			});
		}
	}
}
#endif
