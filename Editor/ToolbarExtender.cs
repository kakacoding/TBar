#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TBar.Editor
{
	[InitializeOnLoad]
	public static class ToolbarExtender
	{
		private static readonly Type ToolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
		private static ScriptableObject CurrentToolbar { get; set; }
		private static VisualElement LeftZoneContainer { get; set; }
		private static VisualElement RightZoneContainer { get; set; }
		
		private static TBarConfig _config;
		
		public static Action<string, string> TrackingCallback; 

		static ToolbarExtender()
		{
			EditorApplication.update -= OnUpdate;
			EditorApplication.update += OnUpdate;
			EditorApplication.playModeStateChanged += OnChangePlayMode;
		}
		private static void OnUpdate()
		{
			if (!EnableTBar) return;
			if (CurrentToolbar != null)
			{
				_config?.Elements.ForEach(element => element.OnUpdate());
				return;
			}
			
			var toolbars = Resources.FindObjectsOfTypeAll(ToolbarType);
			CurrentToolbar = toolbars.Length > 0 ? (ScriptableObject) toolbars[0] : null;
			if (CurrentToolbar == null) return;
			
			var root = (VisualElement)CurrentToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(CurrentToolbar);
			TBarUtility.AttachStyles(root);
			LeftZoneContainer = GetOrInsertToolbarZoneContainer(root, "ToolbarZoneLeftAlign", "ToolbarLeftZone", ""); 
			RightZoneContainer = GetOrInsertToolbarZoneContainer(root, "ToolbarZoneRightAlign", "ToolbarRightZone", "History");
			RefreshAdjustZone();
			Reload();
		}

		internal static void Reload()
		{
			InitElements();
			GUILeft(LeftZoneContainer);
			GUIRight(RightZoneContainer);
		}

		static void RefreshAdjustZone()
		{
			AdjustLeftZoneWidth(EnableAdjustLeftZone ? LeftZoneWidth : StyleKeyword.Auto);
			AdjustRightZoneWidth(EnableAdjustRightZone ? RightZoneWidth : StyleKeyword.Auto);
			//启动后未创建过TBar，在开启TBar后，需要在下一帧刷新区域底色
			EnableZoneBackgroundColor = EnableZoneBackgroundColor;
		}

		internal static bool EnableTBar
		{
			get
			{
				_enableTBar ??= EditorPrefs.GetBool("EnableTBar", false);
				return _enableTBar.Value;
			}
			set
			{
				EditorPrefs.SetBool("EnableTBar", value);
				_enableTBar = value;
				//已创建过TBar后，开关TBar需要刷新区域底色
				EnableZoneBackgroundColor = EnableZoneBackgroundColor;
			}
		}
		private static bool? _enableTBar = null;
		internal static bool EnableAdjustLeftZone
		{
			get => EditorPrefs.GetBool("EnableAdjustLeftZone", false);
			set
			{
				EditorPrefs.SetBool("EnableAdjustLeftZone", value); 
				AdjustLeftZoneWidth(value ? LeftZoneWidth : StyleKeyword.Auto);
			}
		}
		internal static bool EnableAdjustRightZone
		{
			get => EditorPrefs.GetBool("EnableAdjustRightZone", false);
			set
			{
				EditorPrefs.SetBool("EnableAdjustRightZone", value); 
				AdjustRightZoneWidth(value ? RightZoneWidth : StyleKeyword.Auto);
			}
		}

		internal static int LeftZoneWidth
		{
			get => EditorPrefs.GetInt("LeftZoneWidth", 0);
			set
			{
				EditorPrefs.SetInt("LeftZoneWidth", value); 
				AdjustLeftZoneWidth(value);
			}
		}
		internal static int RightZoneWidth
		{
			get => EditorPrefs.GetInt("RightZoneWidth", 0);
			set
			{
				EditorPrefs.SetInt("RightZoneWidth", value); 
				AdjustRightZoneWidth(value);
			}
		}

		private static void AdjustLeftZoneWidth(StyleLength length)
		{
			if (LeftZoneContainer != null)
			{
				//不设置minwidth，可以在unity窗口缩放时自动缩小TBar的LeftZone区域，与CustomToolbar的左边栏完美适配。
				LeftZoneContainer.style.width = length;
				LeftZoneContainer.style.maxWidth = length == StyleKeyword.Auto ? StyleKeyword.None : length;
			}
		}

		private static void AdjustRightZoneWidth(StyleLength length)
		{
			if (RightZoneContainer != null)
			{
				//在兼容CustomToolbar的模式下，无法完美缩放右区域，会挤占CustommToolbar的右边栏位置。目前没有解决办法。
				RightZoneContainer.style.width = length;
				RightZoneContainer.style.maxWidth = length == StyleKeyword.Auto ? StyleKeyword.None : length;
			}
		}

		internal static bool EnableZoneBackgroundColor
		{
			get => _enableZoneBackgroundColor;
			set
			{
				_enableZoneBackgroundColor = value;
				if (_enableZoneBackgroundColor && EnableTBar)
				{
					LeftZoneContainer?.RemoveFromClassList("ToolbarZone-Background");
					LeftZoneContainer?.AddToClassList("ToolbarZone-Editing-Background");
					
					RightZoneContainer?.RemoveFromClassList("ToolbarZone-Background");
					RightZoneContainer?.AddToClassList("ToolbarZone-Editing-Background");
				}
				else
				{
					LeftZoneContainer?.RemoveFromClassList("ToolbarZone-Editing-Background");
					LeftZoneContainer?.AddToClassList("ToolbarZone-Background");
					
					RightZoneContainer?.RemoveFromClassList("ToolbarZone-Editing-Background");
					RightZoneContainer?.AddToClassList("ToolbarZone-Background");
				}
			}
		}

		private static bool _enableZoneBackgroundColor;
		private static VisualElement GetOrInsertToolbarZoneContainer(VisualElement root, string zoneName, string containerName, string insertBefore)
		{
			var zone = root.Q(zoneName); 
			var container = zone.Q(containerName) ?? new VisualElement
			{
				name = containerName
			};
			if (string.IsNullOrEmpty(insertBefore))
			{
				zone.Add(container);
			}
			else
			{
				var count = zone.childCount;
				var idx = 0;
				for (var i = 0; i < count; ++i)
				{
					if(zone.ElementAt(i).name.Equals(insertBefore))
					{
						idx = i + 1;
						break;
					}
				}
				zone.Insert(idx, container);
			}
			return container;
		}

		private static void OnChangePlayMode(PlayModeStateChange state) 
		{
			if (state == PlayModeStateChange.EnteredPlayMode)
			{
				InitElements();
				Reload();
			}
		}

		private static void InitElements() 
		{
			_config = TBarConfig.Instance; 
			_config.Elements.ForEach(element => element.Init());
		}

		private static void GUILeft(VisualElement container)
		{
			if (container == null) return;
			container.Clear();
			if (!EnableTBar) return;
			if (_config != null && _config.Elements != null)
			{
				var idx = _config.Elements.FindIndex(element => element is ToolbarSides);
				DrawInToolbar(container, 0, idx != -1 ? idx : _config.Elements.Count);
			}
		}
		
		private static void GUIRight(VisualElement container)
		{
			if (container == null) return;
			container.Clear();
			if (!EnableTBar) return;
			if (_config != null && _config.Elements != null)
			{
				var idx = _config.Elements.FindIndex(element => element is ToolbarSides);
				if (idx < _config.Elements.Count && idx != -1)
				{
					DrawInToolbar(container, idx, _config.Elements.Count);
				}
			}
		}

		private static void DrawInToolbar(VisualElement container, int from, int to)
		{
			for (var i = from; i < to; ++i)
			{
				_config.Elements[i].DrawInToolbar(container);
			}
		}
	}
}
#endif
