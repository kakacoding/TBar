#if UNITY_EDITOR && TBAR
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace TBar.Editor
{
	internal class TBarConfig
	{
		private const string ConfigPath = "ProjectSettings/TBar/TBarConfig.json";
		[JsonProperty]
		internal List<BaseToolbarElement> Elements { get; set; } = new();
		private static readonly JsonSerializerSettings JsonSetting = new()
		{
			TypeNameHandling = TypeNameHandling.Auto,
			NullValueHandling = NullValueHandling.Ignore,
			DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
		};

		internal static TBarConfig Instance
		{
			get
			{
				if (_instance != null) return _instance;
				
				if (File.Exists(ConfigPath))
				{
					var json = File.ReadAllText(ConfigPath);
					_instance = JsonConvert.DeserializeObject<TBarConfig>(json, JsonSetting);
					if (_instance != null) return _instance;
				}
				
				_instance = new TBarConfig();
				_instance.Save();
				return _instance;
			}
		}

		private static TBarConfig _instance;

		internal void Save()
		{
			var json = JsonConvert.SerializeObject(this, Formatting.Indented, JsonSetting);
			File.WriteAllText(ConfigPath, json);
			ToolbarExtender.Reload();
		}
	}
}
#endif
