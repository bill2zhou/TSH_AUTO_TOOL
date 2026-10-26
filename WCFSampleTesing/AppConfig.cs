using System.Configuration;

namespace WCFSampleTesing;

/// <summary>
/// 配置项读写：全部存放在 TSH_AUTO_TOOL2.exe.config 的 appSettings 节点中。
/// </summary>
internal static class AppConfig
{
	private const string SectionName = "appSettings";

	public static string Get(string key)
	{
		return Get(key, string.Empty);
	}

	public static string Get(string key, string defaultValue)
	{
		string text = ConfigurationManager.AppSettings[key];
		if (string.IsNullOrEmpty(text))
		{
			return defaultValue;
		}
		return text;
	}

	public static void Set(string key, string value)
	{
		Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
		KeyValueConfigurationCollection settings = configuration.AppSettings.Settings;
		if (settings[key] == null)
		{
			settings.Add(key, value ?? string.Empty);
		}
		else
		{
			settings[key].Value = value ?? string.Empty;
		}
		configuration.Save(ConfigurationSaveMode.Modified);
		ConfigurationManager.RefreshSection(SectionName);
	}
}
