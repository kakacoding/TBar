#if UNITY_EDITOR && TBAR
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine.UIElements;

namespace TBar.Editor
{
	[Serializable]
	[ToolbarElementDisplay("Button@4x", "第三方程序调用", "在工具栏上增加一个按钮以打开第三方程序，格式为Application.dataPath的相对路径，或使用绝对路径")]
	internal class ToolbarThirdPartyInvoke : BaseToolbarElement
	{
		[JsonProperty]
		internal TBarUtility.ETextTextureDisplay SettingDisplayType;
		[JsonProperty]
		internal string BtnText;
		[JsonProperty]
		internal string TexturePath;
		[JsonProperty]
		internal string ExecutePath;
		[JsonProperty]
		internal string Params;
		
		private const string StrExecutePath = "调用的程序路径";
		private const string StrParams = "调用参数";
		private const string StrButton = "选择文件";
		private string Tooltip => $"调用程序 {ExecutePath}";
		
		public override string CountingSubKey => Path.GetFileName(ExecutePath);

		protected override void OnDrawInSettings(VisualElement container)
		{
			base.OnDrawInSettings(container);

			container.Add(TextureTextCtrl.Create(
				() => SettingDisplayType,
				v=> SettingDisplayType=v,
				() => BtnText,
				v=> BtnText = v,
				() => TexturePath,
				v=> TexturePath = v
			));

			container.Add(PathCtrl.Create(
				() => StrExecutePath,
				() => ExecutePath,
				v => ExecutePath = v,
				() => StrButton,
				() => Task.FromResult(EditorUtility.OpenFilePanel(StrButton, "", ""))));//EditorUtility.OpenFilePanel(StrButton, "", "")));
			
			container.Add(TextFieldCtrl.Create(
				() => StrParams,
				() => Params,
				v=> Params=v
			));
		}

		protected override void OnDrawInToolbar(VisualElement container)
		{
			container.Add(ImgToolbarBtnCtrl.Create(
				() => SettingDisplayType,
				() => BtnText,
				() => TexturePath,
				() => Tooltip,
				() =>
				{
					if (!string.IsNullOrEmpty(ExecutePath))
					{
						var startInfo = new ProcessStartInfo
						{
							FileName = Environment.ExpandEnvironmentVariables(ExecutePath),
							Arguments = string.IsNullOrEmpty(Params) ? string.Empty : $"{Environment.ExpandEnvironmentVariables(Params)}"
						};

						var process = new Process();
						process.StartInfo = startInfo;
						process.Start();
						Counting();
					}
				}));
		}
	}
}
#endif
