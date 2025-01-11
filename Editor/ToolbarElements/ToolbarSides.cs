#if UNITY_EDITOR
using System;
using UnityEngine.UIElements;

namespace TBar.Editor
{
	[Serializable]
	[ToolbarElementDisplay("MinMaxSlider@4x", "区域分割", "在本条目以上的在左边区域，以下的在右边区域")]
	internal class ToolbarSides : BaseToolbarElement
	{
		protected override void OnDrawInSettings(VisualElement container)
		{
			base.OnDrawInSettings(container);
			container.AddToClassList("RecordItemContainer-Side");
		}
	}
}
#endif
