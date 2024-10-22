#if UNITY_EDITOR && TBAR
using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace TBar.Editor
{
	[Serializable]
	[ToolbarElementDisplay("[按钮]静音", "已优化为设置后一直有效，包括重开Unity。可以开着项目听歌了~")]
	internal class ToolbarMute : BaseToolbarElement
	{
		[NonSerialized] private static int _mute = 0;
		[NonSerialized] private static bool _inited = false;
		private const string MUTE_KEY = "WWISE_EDITOR_MUTE";
		[NonSerialized] private GUIContent _content;

		private GUIContent content =>
			_content ??= new GUIContent(EditorGUIUtility.IconContent("AudioSource Icon").image,
				ToolbarElementDisplay.GetDetail(GetType()))
			{
				text = "彡"
			};

		internal override GUIStyle Style
		{
			get => _style ??= new GUIStyle(TBarUtility.CommandStyle)
			{
				fixedWidth = 40,
				stretchWidth = false,
			};
			set => _style = new GUIStyle(value)
			{
				fixedWidth = 40,
				stretchWidth = false,
			};
		}

		public override void Init()
		{
			_mute = PlayerPrefs.GetInt(MUTE_KEY);
			if (content != null)
			{
				content.text = _mute == 1 ? " " : "彡";
			}

			_inited = false;
		}

		protected override void OnDrawInSettings(VisualElement container)
		{
		}

		protected override void OnDrawInToolbar(VisualElement container)
		{
			
			// 遍历所有已加载的程序集
			// foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			// {
			// 	// 查找是否存在 AkSoundEngine 类
			// 	Type akSoundEngineType = assembly.GetType("AkSoundEngine");
			// 	if (akSoundEngineType != null)
			// 	{
			// 		found = true;
			// 		break;
			// 	}
			// }
			//
			// if (found)
			// {
			// 	Debug.Log("AkSoundEngine 类存在于项目中。");
			// }
			// else
			// {
			// 	Debug.Log("AkSoundEngine 类未找到。");
			// }
			//
			if (!_inited && EditorApplication.isPlaying)
			{
				//AkSoundEngine.PostEvent(_mute == 1 ? "Minimize" : "Maximize", null);
				_inited = true;
			}

			var bk = GUI.color;
			GUI.color = _mute == 1 ? Color.red : Color.green;
			if (GUILayout.Button(content, Style))
			{
				_mute ^= 1;
				content.text = _mute == 1 ? " " : "彡";
				if (EditorApplication.isPlaying)
				{
					//AkSoundEngine.PostEvent(_mute == 1 ? "Minimize" : "Maximize", null);
				}

				PlayerPrefs.SetInt(MUTE_KEY, _mute);
				Counting();
			}

			GUI.color = bk;
		}
	}
}
#endif
