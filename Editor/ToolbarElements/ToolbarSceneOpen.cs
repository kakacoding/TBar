#if UNITY_EDITOR && TBAR
using System;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;

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
		internal string SceneName;
		
		private const string StrSceneName = "场景名字";
		private const string StrSceneNull = "未设置场景名";
		
		private string Tooltip => string.IsNullOrEmpty(SceneName) ? StrSceneNull : $"打开场景 {SceneName}";

		public override string CountingSubKey => SceneName;

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
			
			container.Add(TextFieldCtrl.Create(
				()=>StrSceneName,
				()=>SceneName,
				v=>SceneName=v
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
					if (string.IsNullOrEmpty(SceneName))
					{
						TBarUtility.LogError(StrSceneNull);
					}
					else
					{
						var sceneGuids = AssetDatabase.FindAssets($"t:scene {SceneName}", new[] { "Assets" });
						if (sceneGuids.Length > 0)
						{
							var path = AssetDatabase.GUIDToAssetPath(sceneGuids[0]);
							EditorSceneManager.OpenScene(path);
							Counting();
						}
						else
						{
							TBarUtility.LogError($"找不到需要打开的名为[{SceneName}]的场景");
						}
					}
				}));
		}
	}
}
#endif
