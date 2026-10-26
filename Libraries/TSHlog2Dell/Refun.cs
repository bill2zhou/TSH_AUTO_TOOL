using System.Text.RegularExpressions;

namespace TSHlog2Dell;

public class Refun
{
	public static string getbomstr(string patten, string matstr)
	{
		string text = "";
		foreach (Match item in new Regex(patten, RegexOptions.IgnoreCase).Matches(matstr))
		{
			GroupCollection groups = item.Groups;
			text = groups[1].Value;
			if (groups[2].Value.Length > 0)
			{
				text = text + "," + groups[2].Value;
			}
		}
		return text;
	}
}
