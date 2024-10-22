#if UNITY_EDITOR && TBAR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine.Windows;
using Directory = System.IO.Directory;

namespace TBar.Editor
{
	internal class TBarSettingsProvider : SettingsProvider
	{
		private const string UXML = "Packages/com.kakacoding.tbar/Editor/UI/ProviderSettings.uxml";
		private const string SETTING_PROVIDER_PATH = "Project/TBar";
		private const string DEFAULT_CONFIG_NAME = "Default";
		private const string CONFIG_DIR = "ProjectSettings/TBar";
		private SerializedObject _toolbarSettings;
		private static ListView _toolbarListView;

		private static readonly Type[] ToolbarElementTypes =
		{
			typeof(ToolbarMenuInvoke),
			typeof(ToolbarThirdPartyInvoke),
			typeof(ToolbarSeparator),
			typeof(ToolbarSides),
			typeof(ToolbarSceneOpen),
			typeof(ToolbarWorkspace),
		};

		private TBarSettingsProvider(string path, SettingsScope scopes = SettingsScope.User) : base(path, scopes)
		{
		}

		[SettingsProvider]
		public static SettingsProvider CreateProvider()
		{
			return new SettingsProvider(SETTING_PROVIDER_PATH, SettingsScope.Project)
			{
				activateHandler = ActivateHandler
			};
		}

		private static SortedDictionary<string, string> ConfigMap 
		{
			get
			{
				var files = Directory.GetFiles(CONFIG_DIR);
				var map = new SortedDictionary<string, string>();
				foreach (var file in files)
				{
					var name = Path.GetFileNameWithoutExtension(file);
					map[name] = file;
				}
				return map;
			}
		}

		private static void ActivateHandler(string _, VisualElement rootElement)
		{
			var window = EditorGUIUtility.Load(UXML) as VisualTreeAsset;
			if (window == null) return;
			window.CloneTree(rootElement);
			TBarUtility.AttachStyles(rootElement);

			var dropdownField = rootElement.Q<DropdownField>("selectConfig");
			dropdownField.choices = ConfigMap.Keys.ToList();
			dropdownField.value = dropdownField.choices[0];
			rootElement.Q<Button>("btnCreate").clicked += OnCreateConfig;
			rootElement.Q<Button>("btnDel").clicked += OnDelConfig;
			rootElement.Q<Button>("btnCopy").clicked += OnCopyConfig;
			rootElement.Q<Button>("btnRename").clicked += OnRenameConfig;
			
			var sv = rootElement.Q<ScrollView>("toolbarScrollView");
			_toolbarListView = new ListView(TBarConfig.Instance.Elements, 20, () =>
			{
				var container = new VisualElement
				{
					name = "ToolbarSettingRecordContainer"
				};
				return container;
			}, (container, i) =>
			{
				if (i < TBarConfig.Instance.Elements.Count)
				{
					TBarConfig.Instance.Elements[i].DrawInSettings(container);
				}
			}) { name = "ToolbarSettingListView", showAddRemoveFooter = true, reorderMode = ListViewReorderMode.Animated, reorderable = true, };

			_toolbarListView.itemIndexChanged += (_, _) =>
			{
				ToolbarExtender.Reload();
			};
			_toolbarListView.itemsRemoved += _ => { _toolbarListView.Rebuild(); };
			_toolbarListView.Q<Button>("unity-list-view__add-button").clickable = new Clickable(() =>
			{
				var menu = new GenericMenu();
				foreach (var toolbarElementType in ToolbarElementTypes)
				{
					if (toolbarElementType == null)
					{
						menu.AddSeparator("");
					}
					else
					{
						var display = ToolbarElementDisplay.GetDisplay(toolbarElementType);
						if (IsToolbarElementValid(toolbarElementType))
						{
							menu.AddItem(new GUIContent(display), false, target =>
							{
								if (target is ToolbarSceneSelection selection)
								{
									selection.Init();
								}

								var idx = _toolbarListView.selectedIndex == -1 ? 0 : _toolbarListView.selectedIndex + 1;
								TBarConfig.Instance.Elements.Insert(idx, target as BaseToolbarElement);
								_toolbarListView.Rebuild();
							}, Activator.CreateInstance(toolbarElementType));
						}
						else
						{
							menu.AddDisabledItem(new GUIContent(display), false);
						}
					}
				}

				menu.ShowAsContext();
			});

			sv.Add(_toolbarListView);
			
			
			rootElement.Q<Button>("btnApply").clicked += TBarConfig.Instance.Save;
		}

		private static void CreateConfig(string name)
		{
			var path = Path.Combine(CONFIG_DIR, $"{name}.json");
			new TBarConfig().Save(path);
		}
		private static void OnCreateConfig()
		{
			var defaultNewName = "NewConfig";
			var names = Directory.GetFiles(CONFIG_DIR).Select(Path.GetFileNameWithoutExtension).ToList();
			if (names.Contains(defaultNewName))
			{
				for (var i = 0; i <= names.Count;++i)
				{
					var name = $"{defaultNewName}{i + 1}";
					if (names.Contains(name)) continue;
					CreateConfig(name);
					return;
				}
			}
			CreateConfig(defaultNewName);
			
		}
		
		private static void OnDelConfig()
		{
			
		}
		
		private static void OnCopyConfig()
		{
			
		}
		
		private static void OnRenameConfig()
		{
			
		}

		internal static bool IsToolbarElementValid(Type t)
		{
			var methodInfo = t.GetMethod("IsValid", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (methodInfo == null) return true;
			var result = methodInfo.Invoke(null, null);
			return (bool)result;
		}
	}
}
#endif