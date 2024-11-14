#if UNITY_EDITOR && TBAR
using Newtonsoft.Json;
using UnityEngine.UIElements;

namespace TBar.Editor
{
	internal abstract class BaseToolbarElement
	{
		[JsonProperty]
		protected bool IsEnabled = true;
		[JsonIgnore]
		public virtual string CountingSubKey => "";

		protected BaseToolbarElement() 
		{
		}
        
		public void DrawInToolbar(VisualElement container)
		{
			if (IsEnabled) OnDrawInToolbar(container);
		}

		public void DrawInSettings(VisualElement container) => OnDrawInSettings(container);

		public virtual void Init()
		{
		}

		protected virtual void OnDrawInSettings(VisualElement container)
		{
			container.Clear();
			container.Add(EnableCtrl.Create(
				() => ToolbarElementDisplay.GetDisplay(GetType()),
				() => IsEnabled,
				v=> IsEnabled = v,
				() => ToolbarElementDisplay.GetDetail(GetType())
				));
		}

		protected virtual void OnDrawInToolbar(VisualElement container)
		{
		}

		internal virtual void OnUpdate()
		{
		}

		protected void Counting()
		{
		}
	}
}
#endif