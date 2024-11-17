#if UNITY_EDITOR && TBAR
using System;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine.UIElements;

namespace TBar.Editor
{
	[Serializable]
	[ToolbarElementDisplay("Button@4x", "调用菜单", "在工具栏上增加一个按钮以打开指定的菜单，格式为【Help/About Unity】")]
	internal class ToolbarMenuInvoke : BaseToolbarElement
	{
		[JsonProperty]
		internal TBarUtility.ETextTextureDisplay SettingDisplayType;
		[JsonProperty]
		internal string BtnText;
		[JsonProperty]
		internal string TexturePath;
		[JsonProperty]
		internal string MenuInvokePath;
		
		private const string StrMenuPath = "菜单路径";
		private const string StrButton = "选择菜单";
		private string Tooltip => $"调用菜单 {MenuInvokePath}";

		public override string CountingSubKey => MenuInvokePath;
		
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
				() => StrMenuPath,
				() => MenuInvokePath,
				v => MenuInvokePath=v,
				() => StrButton,
				async () => await MenuItemSelectWindow.ShowWindowAsync()
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
					if (!string.IsNullOrEmpty(MenuInvokePath))
					{
						EditorApplication.ExecuteMenuItem(MenuInvokePath);
						Counting();
					}
				}));
		}
	}
}
#endif
