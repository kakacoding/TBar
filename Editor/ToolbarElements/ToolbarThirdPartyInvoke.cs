#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine.UIElements;

namespace TBar.Editor
{
	[Serializable]
	[ToolbarElementDisplay("Button@4x", "调用第三方程序", "在工具栏上增加一个按钮以打开第三方程序，路径支持以下方式：\r\nApplication.dataPath的相对路径\r\n绝对路径\r\n类似%UserProfile%的环境变量")]
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
		
		private const string StrExecutePath = "程序路径";
		private const string StrParams = "调用参数";
		private const string StrButton = "选择文件";
		private string Tooltip => $"调用程序 {ExecutePath}";

		private static readonly List<string> EnvironmentVariables = new()
		{
			"%APPDATA%",
			"%LOCALAPPDATA%",
			"%TEMP%",
			"%USERPROFILE%",
		};

		private static string ReplaceEnvironmentPath(string path)
		{
			if (string.IsNullOrEmpty(path)) return path;
			path = Path.GetFullPath(path);
			foreach (var environmentVariable in EnvironmentVariables)
			{
				var environmentPath = Environment.ExpandEnvironmentVariables(environmentVariable);
				if (path.StartsWith(environmentPath))
				{
					return path.Replace(environmentPath, $"{environmentVariable}");
				}
			}

			return path;
		}
		
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
				() =>
				{
					var path = EditorUtility.OpenFilePanel(StrButton, "", "");
					return Task.FromResult(ReplaceEnvironmentPath(path));
				}));
			
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
