#if UNITY_EDITOR && TBAR
using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace TBar.Editor
{
	[Serializable]
	[ToolbarElementDisplay("[按钮]设置", "打开toolbar设置窗口")]
	internal class ToolbarSetting : BaseToolbarElement
	{
		private GUIContent content = new();

		internal override GUIStyle Style
		{
			get => new("AppCommand")
			{
				stretchWidth = false,
				stretchHeight = false,
				fontSize = 9,
				fixedWidth = 70,
				fixedHeight = 22,
				fontStyle = FontStyle.Bold,
				alignment = TextAnchor.MiddleCenter,
				imagePosition = ImagePosition.ImageLeft,
			};
			set { }
		}

		public override void Init()
		{
		}

		protected override void OnDrawInSettings(VisualElement container)
		{
		}

		protected override void OnDrawInToolbar(VisualElement container)
		{
			//var sdkType = BundleTable.BuildBundleInfo.BundleMemo;
			//var dataType = ContentDataImporter.GetCurrentContentDataEdition().Equals("CN")?"国内":"海外";
			//content.text = $"sdk ->{sdkType}\r\ndata->{dataType}";
			content.text = $"hhahah";
			var bk = GUI.color;
			EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
			// if (!sdkType.Contains(dataType))
			// {
			// 	GUI.color = Color.red;
			// 	content.tooltip = "SDK版本和数据版本不匹配，点击切换数据版本。\r\n如要切换SDK版本，点击菜单Tools->XDSDK版本切换";
			// }
			// else
			// {
			// 	GUI.color = Color.green;
			// 	content.tooltip = "点击打开工具栏设置";
			// }

			if (GUILayout.Button(content, Style))
			{
				//if (!sdkType.Equals(dataType))
				{
					//ContentDataImporter.ImportContentData();	
				}
				//else
				{
					SettingsService.OpenProjectSettings("Project/自定义工具栏");
				}
				Counting();
			}

			EditorGUI.EndDisabledGroup();
			GUI.color = bk;
		}

		public void ForceDrawInToolbar()
		{
			//OnDrawInToolbar();
		}
	}
}
#endif
