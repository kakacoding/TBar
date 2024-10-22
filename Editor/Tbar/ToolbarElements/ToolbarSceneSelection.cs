#if UNITY_EDITOR && TBAR
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;

namespace TBar.Editor
{
	[Serializable]
	[ToolbarElementDisplay("[下拉框]场景选择", "在toolbar上增加一个下拉框用来选择需要打开的场景")]
	internal class ToolbarSceneSelection : BaseToolbarElement
	{
		[SerializeField] private bool showSceneFolder = false;
		SceneData[] _scenesPopupDisplay;
		string[] _scenesPath;
		int _selectedSceneIndex;

		internal override GUIStyle Style
		{
			get => _style ??= new GUIStyle(TBarUtility.DropDownStyle);
			set { }
		}

		public override void Init()
		{
			RefreshScenesList();
			EditorSceneManager.sceneOpened -= HandleSceneOpened;
			EditorSceneManager.sceneOpened += HandleSceneOpened;
		}

		protected override void OnDrawInSettings(VisualElement container)
		{
			// //position.x += position.width + FieldSizeSpace*3;
			// //position.width = DefaultSectionWidth;
			// //WidthInToolbar = EditorGUI.IntField(position, "工具栏中所占宽度", WidthInToolbar);
			// position.x += position.width + FieldSizeSpace * 3;
			// position.width = DefaultSectionWidth;
			// showSceneFolder = EditorGUI.Toggle(position, "按文件夹分组", showSceneFolder);
			// if (GUI.changed)
			// {
			// 	RefreshScenesList();
			// 	//ToolbarCallback.RefreshToolbar();
			// }
		}

		protected override void OnDrawInToolbar(VisualElement container)
		{
			EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
			DrawSceneDropdown();
			EditorGUI.EndDisabledGroup();
		}

		private string lastOpenScenePath = "";

		private void DrawSceneDropdown()
		{
			_selectedSceneIndex = EditorGUILayout.Popup(_selectedSceneIndex,
				_scenesPopupDisplay.Select(e => e.popupDisplay).ToArray(), TBarUtility.DropDownStyle);
			if (GUI.changed && 0 <= _selectedSceneIndex && _selectedSceneIndex < _scenesPopupDisplay.Length)
			{
				if (!EditorApplication.isPlaying && EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
				{
					foreach (var scenePath in _scenesPath)
					{
						if (scenePath == _scenesPopupDisplay[_selectedSceneIndex].path)
						{
							if (string.IsNullOrEmpty(lastOpenScenePath) || (!string.IsNullOrEmpty(lastOpenScenePath) &&
							                                                !lastOpenScenePath.Equals(scenePath)))
							{
								lastOpenScenePath = scenePath;
								EditorSceneManager.OpenScene(scenePath);
								Counting();
							}

							break;
						}
					}
				}
			}
		}

		void RefreshScenesList()
		{
			_selectedSceneIndex = -1;
			var toDisplay = new List<SceneData>();
			var sceneGuids = AssetDatabase.FindAssets("t:scene", new[] { "Assets" });
			_scenesPath = new string[sceneGuids.Length];
			for (var i = 0; i < _scenesPath.Length; ++i)
			{
				_scenesPath[i] = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);
			}

			var activeScene = SceneManager.GetActiveScene();
			var names = new List<string>();
			var folders = new List<string>();
			for (var i = 0; i < _scenesPath.Length; ++i)
			{
				var name = GetSceneName(_scenesPath[i]);
				if (showSceneFolder)
				{
					var folderName = Path.GetFileName(Path.GetDirectoryName(_scenesPath[i]));
					folders.Add(folderName);
				}

				names.Add(name);
				if (_selectedSceneIndex == -1 && name == activeScene.name)
				{
					_selectedSceneIndex = i;
				}
			}

			if (showSceneFolder)
			{
				var dicCount = new Dictionary<string, int>();
				for (var i = 0; i < folders.Count; ++i)
				{
					dicCount[folders[i]] = dicCount.ContainsKey(folders[i]) ? dicCount[folders[i]] + 1 : 1;
				}

				for (var i = 0; i < folders.Count; ++i)
				{
					if (dicCount[folders[i]] > 1)
					{
						names[i] = $"{folders[i]}/{names[i]}";
					}
				}
			}

			for (var i = 0; i < names.Count; ++i)
			{
				toDisplay.Add(new SceneData()
				{
					path = _scenesPath[i],
					popupDisplay = new GUIContent(names[i],
						EditorGUIUtility.Load("d_BuildSettings.SelectedIcon") as Texture,
						"打开场景"),
				});
			}

			_scenesPopupDisplay = toDisplay.ToArray();
		}

		void HandleSceneOpened(Scene scene, OpenSceneMode mode)
		{
			RefreshScenesList();
		}

		string GetSceneName(string path)
		{
			path = path.Replace(".unity", "");
			return Path.GetFileName(path);
		}

		class SceneData
		{
			public string path;
			public GUIContent popupDisplay;
		}
	}
}
#endif
