#if UNITY_EDITOR && TBAR
using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine.UIElements;

namespace TBar.Editor
{

	[Serializable]
	[ToolbarElementDisplay("[按钮]P4Workspace", "显示对应的Perforce的Workspace，没有设置时点击可以打开设置界面")]
	internal class ToolbarWorkspace : BaseToolbarElement
	{
		internal override void OnUpdate()
		{
			//instance.
			//instance?.tooltip
		}

		private string _textGetter()
		{
			var workspace = EditorUserSettings.GetConfigValue("vcPerforceWorkspace");
			return string.IsNullOrEmpty(workspace) ? "未配置P4" : workspace+ (Provider.onlineState != OnlineState.Offline ? "-on" : "-off");
		}

		private string _tooltipGetter()
		{
			var workspace = EditorUserSettings.GetConfigValue("vcPerforceWorkspace");
			return string.IsNullOrEmpty(workspace) ? "点击开启配置窗口" : $@"点击本按钮 强制重连P4
红色表示P4 Disconnect
绿色表示P4 Connect
workspace路径:{Application.dataPath}";
		}

		private void _onClick()
		{
			var workspace = EditorUserSettings.GetConfigValue("vcPerforceWorkspace");
			if (string.IsNullOrEmpty(workspace))
			{
				var assembly = Assembly.GetAssembly(typeof(EditorWindow));
				var t = assembly.GetType("UnityEditor.SettingsWindow");
				var obj = ScriptableObject.CreateInstance(t);
				t.GetMethod("Show", BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(obj, new object[] { SettingsScope.Project, "Project/Version Control" });
			}
			else
			{
				Provider.UpdateSettings();
			}
			Counting();	
			Refresh();
		}

		private VisualElement SubContainer => _subContainer ??= new VisualElement();

		private VisualElement _subContainer;

		private void Refresh()
		{
			SubContainer?.Clear();
			SubContainer?.Add(ImageButtonCtrl.Create(
				()=>TBarUtility.ETextTextureDisplay.TextureText,
				_textGetter,
				()=>null,
				_tooltipGetter,
				_onClick));
		}
		
		protected override void OnDrawInToolbar(VisualElement container)
		{
			Refresh();
			if (SubContainer != null)
			{
				container.Add(SubContainer);
			}
			
// 			
// 			var workspace = EditorUserSettings.GetConfigValue("vcPerforceWorkspace");
// 			if (!string.IsNullOrEmpty(workspace))
// 			{
// 				container.Add(instance = ImageButtonCtrl.Create(
// 					()=>CustomToolbarUtility.ETextTextureDisplay.TextureText,
// 					()=>workspace,
// 					()=>null,
// 					()=>$@"点击本按钮 强制重连P4
// 红色表示P4 Disconnect
// 绿色表示P4 Connect
// workspace路径:{Application.dataPath}",
// 					() =>
// 					{
// 						Provider.UpdateSettings();
// 						Counting();
// 					}));
// 				//GUI.color = Provider.onlineState != OnlineState.Offline ? Color.green : Color.red;
// 			}
// 			else
// 			{
// 				container.Add(instance = ImageButtonCtrl.Create(
// 					()=>CustomToolbarUtility.ETextTextureDisplay.TextureText,
// 					()=>"未配置P4",
// 					()=>null,
// 					()=>"点击开启配置窗口",
// 					() =>
// 					{
// 						var assembly = Assembly.GetAssembly(typeof(EditorWindow));
// 						var t = assembly.GetType("UnityEditor.SettingsWindow");
// 						var obj = ScriptableObject.CreateInstance(t);
// 						t.GetMethod("Show", BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(obj, new object[] { SettingsScope.Project, "Project/Version Control" });
// 						Counting();
// 					}));
// 			}
		}
	}
}
#endif