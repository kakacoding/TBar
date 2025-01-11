#if UNITY_EDITOR
using System;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;
using System.Linq;

namespace TBar.Editor
{
	[Serializable]
	[ToolbarElementDisplay("Button@4x", "打开场景", "在工具栏上增加一个按钮以打开指定的场景")]
	internal class ToolbarSceneOpen : BaseToolbarElement
	{
		[JsonProperty]
		internal TBarUtility.ETextTextureDisplay SettingDisplayType;
		[JsonProperty]
		internal string BtnText;
		[JsonProperty]
		internal string TexturePath;
		[JsonProperty]
		internal string ScenePath;
		
		private const string StrScenePath = "场景路径";
		private const string StrSceneNull = "未设置场景名";
		private const string StrButton = "选择场景";
		
		private string Tooltip => string.IsNullOrEmpty(ScenePath) ? StrSceneNull : $"打开场景 {ScenePath}";

		public override string CountingSubKey => ScenePath;

		protected override void OnDrawInSettings(VisualElement container)
		{
			base.OnDrawInSettings(container);
			
			container.Add(TextureTextCtrl.Create(
				()=>SettingDisplayType,
				v=>SettingDisplayType=v,
				()=>BtnText,
				v=>BtnText=v,
				()=>TexturePath,
				v=>TexturePath=v
			));
			
			container.Add(PathCtrl.Create(
				()=>StrScenePath,
				()=>ScenePath,
				v=>ScenePath=v,
				() => StrButton,
				async () => await SelectWindow.ShowWindowAsync("场景列表", () =>
				{
					var sceneGuids = AssetDatabase.FindAssets("t:Scene");
					var scenePaths = sceneGuids.Select(AssetDatabase.GUIDToAssetPath).ToList();
					scenePaths.Sort();
					return scenePaths;
				})
			));
		}

		protected override void OnDrawInToolbar(VisualElement container)
		{
			container.Add(ImgToolbarBtnCtrl.Create(
				()=>SettingDisplayType,
				()=>BtnText,
				()=>TexturePath,
				()=>Tooltip,
				() =>
				{
					if (string.IsNullOrEmpty(ScenePath))
					{
						TBarUtility.LogError(StrSceneNull);
					}
					else
					{
						EditorSceneManager.OpenScene(ScenePath);
						Counting();
					}
				}));
		}
	}
}
#endif
