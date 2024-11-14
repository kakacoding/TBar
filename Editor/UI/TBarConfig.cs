#if UNITY_EDITOR && TBAR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;

namespace TBar.Editor
{
	internal abstract class TBarConfigBase
	{
		protected static readonly JsonSerializerSettings JsonSetting = new()
		{
			TypeNameHandling = TypeNameHandling.Auto,
			NullValueHandling = NullValueHandling.Ignore,
			DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
		};
		protected static void EnsureDir(string path)
		{
			var dir = Path.GetDirectoryName(path);
			if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}
		}
		public abstract void Save();
	}
	internal class TBarStatus : TBarConfigBase
	{
		[JsonProperty] 
		internal string CurConfigPath = string.Empty;

		public string CurConfigName
		{
			get
			{
				if (CurConfigPath.StartsWith(TBarConfig.ConfigDir))
				{
					return Path.GetFileNameWithoutExtension(CurConfigPath);
				}

				return string.Empty;
			}
		}
		private const string DefaultConfigPath = "Library/TBar/DefaultStatus.json";
		internal static TBarStatus Instance
		{
			get
			{
				if (_instance != null) return _instance;
				
				if (File.Exists(DefaultConfigPath))
				{
					var json = File.ReadAllText(DefaultConfigPath);
					_instance = JsonConvert.DeserializeObject<TBarStatus>(json, JsonSetting);
					if (_instance != null) return _instance;
				}

				_instance = new TBarStatus();
				_instance.Save();
				return _instance;
			}
		}
		private static TBarStatus _instance;
		private void Save(string path)
		{
			var json = JsonConvert.SerializeObject(this, Formatting.Indented, JsonSetting);
			EnsureDir(path);
			File.WriteAllText(path, json);
		}

		public override void Save()
		{
			Save(DefaultConfigPath);
		}
	}
	internal class TBarConfig : TBarConfigBase
	{
		public const string NewDefaultName = "NewConfig";
		public const string ConfigDir = "ProjectSettings/TBar/";
		private static readonly string DefaultConfigPath = $"{ConfigDir}Default.json";
		[JsonProperty]
		internal List<BaseToolbarElement> Elements { get; set; } = new();
		
		private static TBarStatus CurStatus;

		internal static TBarConfig Instance
		{
			get
			{
				if (_instance != null) return _instance;
				
				if (!string.IsNullOrEmpty(TBarStatus.Instance.CurConfigPath) && File.Exists(TBarStatus.Instance.CurConfigPath))
				{
					var json = File.ReadAllText(TBarStatus.Instance.CurConfigPath);
					_instance = JsonConvert.DeserializeObject<TBarConfig>(json, JsonSetting);
					if (_instance != null) return _instance;
				}
				
				_instance = new TBarConfig
				{
					Elements = new ()
					{
						new ToolbarMenuInvoke
						{
							SettingDisplayType = TBarUtility.ETextTextureDisplay.TextTexture,
							BtnText = "Test",
							TexturePath = "Packages/com.kakacoding.tbar/Editor/Resources/Command.png",
							MenuInvokePath = "Help/About Unity"
						},
						new ToolbarSides(),
						new ToolbarMenuInvoke
						{
							SettingDisplayType = TBarUtility.ETextTextureDisplay.TextTexture,
							BtnText = "Test",
							TexturePath = "Packages/com.kakacoding.tbar/Editor/Resources/Command.png",
							MenuInvokePath = "Help/About Unity"
						},
					}
				};
				_instance.Save();
				return _instance;
			}
		}
		internal static readonly string ConfigTitle = "================ Present by kakacoding ================\r\n";
		public string ExportConfigStr()
		{
			var jsonStr = JsonConvert.SerializeObject(this, Formatting.None, JsonSetting);
			return $"{ConfigTitle}{Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonStr))}";
		}
		
		public void ImportConfigStr(string configStr)
		{
			var base64Bytes = Convert.FromBase64String(configStr.Substring(ConfigTitle.Length)); // 将 Base64 字符串转换回字节数组
			var jsonStr = Encoding.UTF8.GetString(base64Bytes); 
			_instance = JsonConvert.DeserializeObject<TBarConfig>(jsonStr, JsonSetting);
			DuplicateCurConfig(NewDefaultName);
			ToolbarExtender.Reload();
			SettingsService.NotifySettingsProviderChanged();
		}

		private static TBarConfig _instance;

		public static SortedDictionary<string, string> ConfigMap 
		{
			get
			{
				EnsureDir(ConfigDir);
				var files = Directory.GetFiles(ConfigDir);
				var map = new SortedDictionary<string, string>();
				foreach (var file in files)
				{
					var name = Path.GetFileNameWithoutExtension(file);
					map[name] = file;
				}
				return map;
			}
		}
		internal void LoadInDefaultDirByName(string configName)
		{
			var configPath = $"{ConfigDir}{configName}.json";
			Load(configPath);
		}

		private void Load(string configPath)
		{
			if (File.Exists(configPath))
			{
				TBarStatus.Instance.CurConfigPath = configPath;
				TBarStatus.Instance.Save();
				
				var json = File.ReadAllText(configPath);
				_instance = JsonConvert.DeserializeObject<TBarConfig>(json, JsonSetting);
				ToolbarExtender.Reload();
				SettingsService.NotifySettingsProviderChanged();
			}
		}

		public override void Save()
		{
			var json = JsonConvert.SerializeObject(this, Formatting.Indented, JsonSetting);
			if (string.IsNullOrEmpty(TBarStatus.Instance.CurConfigPath))
			{
				TBarStatus.Instance.CurConfigPath = DefaultConfigPath;
				TBarStatus.Instance.Save();
			}
			EnsureDir(TBarStatus.Instance.CurConfigPath);
			File.WriteAllText(TBarStatus.Instance.CurConfigPath, json);
			ToolbarExtender.Reload();
		}

		private void Save(string path)
		{
			var json = JsonConvert.SerializeObject(this, Formatting.Indented, JsonSetting);
			EnsureDir(path);
			File.WriteAllText(path, json);
			if (!path.Equals(TBarStatus.Instance.CurConfigPath))
			{
				TBarStatus.Instance.CurConfigPath = path;
				TBarStatus.Instance.Save();
			}
			ToolbarExtender.Reload();
			SettingsService.NotifySettingsProviderChanged();
		}

		internal void DeleteCurAndLoadOther(string configName)
		{
			File.Delete(TBarStatus.Instance.CurConfigPath);
			LoadInDefaultDirByName(configName);
		}

		internal void Rename(string configName)
		{
			File.Delete(TBarStatus.Instance.CurConfigPath);
			DuplicateCurConfig(configName);
		}
		
		internal void CreateNewConfig(string name)
		{
			var path = Path.Combine(ConfigDir, $"{name}.json");
			Elements.Clear();	
			Save(path);
		}
		internal void DuplicateCurConfig(string newName)
		{
			var defaultNewName = newName;
			var names = Directory.GetFiles(ConfigDir).Select(Path.GetFileNameWithoutExtension).ToList();
			if (names.Contains(defaultNewName))
			{
				for (var i = 0; i <= names.Count;++i)
				{
					var name = $"{defaultNewName}{i + 1}";
					if (names.Contains(name)) continue;
					Save($"{ConfigDir}{name}.json");
					return;
				}
			}
			Save($"{ConfigDir}{defaultNewName}.json");
		}
	}
}
#endif
