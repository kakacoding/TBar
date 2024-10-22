#if UNITY_EDITOR && TBAR
using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace TBar.Editor
{
	[Serializable]
	[ToolbarElementDisplay("[按钮]停音", "执行一次StopAllSound，不影响声音再次播放")]
	internal class ToolbarStopSound : BaseToolbarElement
	{
		//public override string DisplayNameInToolbar => "停音";
		public override string CountingSubKey => "";

		public override void Init()
		{
		}

		protected override void OnDrawInSettings(VisualElement container)
		{
		}

		private GUIContent content = new();

		protected override void OnDrawInToolbar(VisualElement container)
		{
			EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);
			//content.text = DisplayNameInToolbar;
			content.tooltip = ToolbarElementDisplay.GetDetail(GetType());
			if (GUILayout.Button(content, Style))
			{
				//AkSoundEngine.StopAll();
				Counting();
			}

			EditorGUI.EndDisabledGroup();
		}
	}
}
#endif
