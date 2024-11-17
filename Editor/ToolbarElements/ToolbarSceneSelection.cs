#if UNITY_EDITOR && TBAR
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace TBar.Editor
{
	[Serializable]
	[ToolbarElementDisplay("Dropdown@4x", "场景选择", "在工具栏上增加一个场景下拉框")]
	internal class ToolbarSceneSelection : BaseToolbarElement
	{
		[JsonProperty]
		internal bool ShowSceneFolder = false;
		
		private List<SceneData> sceneDatas;
		private ToolbarMenu sceneToolbarMenu;
		private const string StrShowFolder = "按目录层级显示";
		//unity\Modules\UIServiceEditor\EditorToolbar\ToolbarElements\AccountDropdown.cs
		
		public override string CountingSubKey => sceneToolbarMenu?.text;
		
		class SceneData
		{
			public string guid;
			public string name;
			public string fullPath;
			public string simplePath;
		}
		
		public override void Init()
		{
			RefreshScenesList();
			EditorSceneManager.sceneOpened -= HandleSceneOpened;
			EditorSceneManager.sceneOpened += HandleSceneOpened;
		}

		protected override void OnDrawInSettings(VisualElement container)
		{
			base.OnDrawInSettings(container);
			container.Add(ToggleCtrl.Create(
				() => StrShowFolder,
				() => ShowSceneFolder,
				v=> ShowSceneFolder = v
				));
		}
		
		protected override void OnDrawInToolbar(VisualElement container)
		{
			sceneToolbarMenu = new ToolbarMenu
			{
				name = "SceneMenu",
				text = SceneManager.GetActiveScene().name
			};
			foreach (var sceneData in sceneDatas)
			{
				sceneToolbarMenu.menu.AppendAction(ShowSceneFolder ? sceneData.simplePath : sceneData.name,
					_ => OnMenuItemClick(sceneData),
					_=>OnStatusCallback(sceneData),
					sceneData
					);
			}
			container.Add(sceneToolbarMenu);
		}

		void OnMenuItemClick(SceneData sceneData)
		{
			if (sceneToolbarMenu != null)
			{
				sceneToolbarMenu.text = sceneData.name;	
			}

			if (File.Exists(sceneData.fullPath))
			{
				EditorSceneManager.OpenScene(sceneData.fullPath);
				Counting();
			}
		}

		DropdownMenuAction.Status OnStatusCallback(SceneData sceneData) => SceneManager.GetActiveScene().path.Equals(sceneData.fullPath) ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal;

		void RefreshScenesList()
		{
			sceneDatas = AssetDatabase.FindAssets("t:scene", new[] { "Assets" }).Select(guid=>
			{
				var data = new SceneData
				{
					guid = guid,
					fullPath = AssetDatabase.GUIDToAssetPath(guid)
				};
				data.name = Path.GetFileNameWithoutExtension(data.fullPath);
				data.simplePath = Path.GetFileName(Path.GetDirectoryName(data.fullPath));
				data.simplePath += $"/{data.name}";
				return data;
			}).ToList();
			if (ShowSceneFolder)
			{
				sceneDatas.Sort((x,y)=>string.Compare(x.simplePath, y.simplePath, StringComparison.InvariantCultureIgnoreCase));	
			}
			else
			{
				sceneDatas.Sort((x,y)=>string.Compare(x.name, y.name, StringComparison.InvariantCultureIgnoreCase));
			}
		}

		private void HandleSceneOpened(Scene scene, OpenSceneMode mode) => RefreshScenesList();
	}
}
#endif
