using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Management;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Security;
using System.Xml;

namespace KCString;

public class KCSTRING
{
	public string path;

	private string _XMLPath;

	private string SqlConnectionString;

	private SqlConnection SQLConnect;

	private DataSet Dataset;

	private SqlCommand cmd;

	private SqlDataAdapter data;

	private static uint iCodeStart = 0u;

	private static uint iCodeEnd = 0u;

	private static uint iCodeChecksum = 0u;

	private static string sCodeEnd = "00000000";

	private static string sCodeChecksum = "00000000";

	private static string sCodeCRC = "00000000";

	private static string sUserBootCRC = "00000000";

	private static string sDataFlashCRC = "00000000";

	private static uint[] crc_32_tab = new uint[256]
	{
		0u, 1996959894u, 3993919788u, 2567524794u, 124634137u, 1886057615u, 3915621685u, 2657392035u, 249268274u, 2044508324u,
		3772115230u, 2547177864u, 162941995u, 2125561021u, 3887607047u, 2428444049u, 498536548u, 1789927666u, 4089016648u, 2227061214u,
		450548861u, 1843258603u, 4107580753u, 2211677639u, 325883990u, 1684777152u, 4251122042u, 2321926636u, 335633487u, 1661365465u,
		4195302755u, 2366115317u, 997073096u, 1281953886u, 3579855332u, 2724688242u, 1006888145u, 1258607687u, 3524101629u, 2768942443u,
		901097722u, 1119000684u, 3686517206u, 2898065728u, 853044451u, 1172266101u, 3705015759u, 2882616665u, 651767980u, 1373503546u,
		3369554304u, 3218104598u, 565507253u, 1454621731u, 3485111705u, 3099436303u, 671266974u, 1594198024u, 3322730930u, 2970347812u,
		795835527u, 1483230225u, 3244367275u, 3060149565u, 1994146192u, 31158534u, 2563907772u, 4023717930u, 1907459465u, 112637215u,
		2680153253u, 3904427059u, 2013776290u, 251722036u, 2517215374u, 3775830040u, 2137656763u, 141376813u, 2439277719u, 3865271297u,
		1802195444u, 476864866u, 2238001368u, 4066508878u, 1812370925u, 453092731u, 2181625025u, 4111451223u, 1706088902u, 314042704u,
		2344532202u, 4240017532u, 1658658271u, 366619977u, 2362670323u, 4224994405u, 1303535960u, 984961486u, 2747007092u, 3569037538u,
		1256170817u, 1037604311u, 2765210733u, 3554079995u, 1131014506u, 879679996u, 2909243462u, 3663771856u, 1141124467u, 855842277u,
		2852801631u, 3708648649u, 1342533948u, 654459306u, 3188396048u, 3373015174u, 1466479909u, 544179635u, 3110523913u, 3462522015u,
		1591671054u, 702138776u, 2966460450u, 3352799412u, 1504918807u, 783551873u, 3082640443u, 3233442989u, 3988292384u, 2596254646u,
		62317068u, 1957810842u, 3939845945u, 2647816111u, 81470997u, 1943803523u, 3814918930u, 2489596804u, 225274430u, 2053790376u,
		3826175755u, 2466906013u, 167816743u, 2097651377u, 4027552580u, 2265490386u, 503444072u, 1762050814u, 4150417245u, 2154129355u,
		426522225u, 1852507879u, 4275313526u, 2312317920u, 282753626u, 1742555852u, 4189708143u, 2394877945u, 397917763u, 1622183637u,
		3604390888u, 2714866558u, 953729732u, 1340076626u, 3518719985u, 2797360999u, 1068828381u, 1219638859u, 3624741850u, 2936675148u,
		906185462u, 1090812512u, 3747672003u, 2825379669u, 829329135u, 1181335161u, 3412177804u, 3160834842u, 628085408u, 1382605366u,
		3423369109u, 3138078467u, 570562233u, 1426400815u, 3317316542u, 2998733608u, 733239954u, 1555261956u, 3268935591u, 3050360625u,
		752459403u, 1541320221u, 2607071920u, 3965973030u, 1969922972u, 40735498u, 2617837225u, 3943577151u, 1913087877u, 83908371u,
		2512341634u, 3803740692u, 2075208622u, 213261112u, 2463272603u, 3855990285u, 2094854071u, 198958881u, 2262029012u, 4057260610u,
		1759359992u, 534414190u, 2176718541u, 4139329115u, 1873836001u, 414664567u, 2282248934u, 4279200368u, 1711684554u, 285281116u,
		2405801727u, 4167216745u, 1634467795u, 376229701u, 2685067896u, 3608007406u, 1308918612u, 956543938u, 2808555105u, 3495958263u,
		1231636301u, 1047427035u, 2932959818u, 3654703836u, 1088359270u, 936918000u, 2847714899u, 3736837829u, 1202900863u, 817233897u,
		3183342108u, 3401237130u, 1404277552u, 615818150u, 3134207493u, 3453421203u, 1423857449u, 601450431u, 3009837614u, 3294710456u,
		1567103746u, 711928724u, 3020668471u, 3272380065u, 1510334235u, 755167117u
	};

	public string XMLPath => _XMLPath;

	public string GetLocalHostName => Dns.GetHostName();

	public string GetLocalIP
	{
		get
		{
			IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
			if (addressList.Length < 1)
			{
				return "";
			}
			return addressList[0].ToString();
		}
	}

	public string GetWANIP
	{
		get
		{
			IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
			if (addressList.Length < 2)
			{
				return "";
			}
			return addressList[1].ToString();
		}
	}

	[DllImport("kernel32")]
	private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

	[DllImport("kernel32")]
	private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

	[DllImport("kernel32")]
	private static extern int GetPrivateProfileString(string section, string key, string defVal, byte[] retVal, int size, string filePath);

	public void INIFile(string INIPath)
	{
		path = INIPath;
	}

	public void IniWriteValue(string Section, string Key, string Value)
	{
		WritePrivateProfileString(Section, Key, Value, path);
	}

	public string IniReadValue(string Section, string Key)
	{
		StringBuilder stringBuilder = new StringBuilder(255);
		GetPrivateProfileString(Section, Key, "", stringBuilder, 255, path);
		return stringBuilder.ToString();
	}

	public byte[] IniReadValues(string section, string key)
	{
		byte[] array = new byte[255];
		GetPrivateProfileString(section, key, "", array, 255, path);
		return array;
	}

	public void ClearAllSection()
	{
		IniWriteValue(null, null, null);
	}

	public void ClearSection(string Section)
	{
		IniWriteValue(Section, null, null);
	}

	public string GetFormatDate(DateTime dt, char Separator)
	{
		if (!dt.Equals(DBNull.Value))
		{
			string text = $"yyyy{Separator}MM{Separator}dd";
			return dt.ToString(text);
		}
		return GetFormatDate(DateTime.Now, Separator);
	}

	public string GetFormatTime(DateTime dt, char Separator)
	{
		if (!dt.Equals(DBNull.Value))
		{
			string text = $"hh{Separator}mm{Separator}ss";
			return dt.ToString(text);
		}
		return GetFormatTime(DateTime.Now, Separator);
	}

	public int SecondToMinute(int Second)
	{
		return Convert.ToInt32(Math.Ceiling((decimal)Second / 60m));
	}

	public string DateDiff(DateTime DateTime1, DateTime DateTime2)
	{
		string result = null;
		try
		{
			TimeSpan timeSpan = DateTime2 - DateTime1;
			if (timeSpan.Days >= 1)
			{
				result = DateTime1.Month + "Month" + DateTime1.Day + "Day";
				return result;
			}
			if (timeSpan.Hours > 1)
			{
				result = timeSpan.Hours + "hour";
				return result;
			}
			result = timeSpan.Minutes + "minute";
			return result;
		}
		catch
		{
			return result;
		}
	}

	public TimeSpan TimeDiff(DateTime DateTime1, DateTime DateTime2)
	{
		TimeSpan timeSpan = new TimeSpan(DateTime1.Ticks);
		TimeSpan ts = new TimeSpan(DateTime2.Ticks);
		return timeSpan.Subtract(ts).Duration();
	}

	public string Soft_Version()
	{
		string result = "";
		string file = "C:\\Teradyne\\TSDevPro\\TestStationfixedsprs.txt";
		string pattern = "Patch Level Version\\s\\d.*";
		if (!File.Exists(file))
		{
			return result;
		}
		using StreamReader streamReader = File.OpenText(file);
		string text;
		while ((text = streamReader.ReadLine()) != null)
		{
			if (Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase))
			{
				string[] list = text.Split(' ');
				if (list.Length > 3)
				{
					result = list[3];
				}
				break;
			}
		}
		return result;
	}

	public int GetMonthLastDate(int year, int month)
	{
		return new DateTime(year, month, new GregorianCalendar().GetDaysInMonth(year, month)).Day;
	}

	public string FormatDate(DateTime dateTime1, string dateMode)
	{
		return dateMode switch
		{
			"0" => dateTime1.ToString("yyyy-MM-dd"), 
			"1" => dateTime1.ToString("yyyy-MM-dd HH:mm:ss"), 
			"2" => dateTime1.ToString("yyyy/MM/dd"), 
			"3" => dateTime1.ToString("yyyy年MM月dd日"), 
			"4" => dateTime1.ToString("MM-dd"), 
			"5" => dateTime1.ToString("MM/dd"), 
			"6" => dateTime1.ToString("MM月dd日"), 
			"7" => dateTime1.ToString("yyyy-MM"), 
			"8" => dateTime1.ToString("yyyy/MM"), 
			"9" => dateTime1.ToString("yyyy年MM月"), 
			_ => dateTime1.ToString(), 
		};
	}

	public DateTime GetRandomTime(DateTime time1, DateTime time2)
	{
		Random random = new Random();
		DateTime dateTime = default(DateTime);
		double totalSeconds = new TimeSpan(time1.Ticks - time2.Ticks).TotalSeconds;
		int num = 0;
		num = ((totalSeconds > 2147483647.0) ? int.MaxValue : ((!(totalSeconds < -2147483648.0)) ? ((int)totalSeconds) : int.MinValue));
		if (num > 0)
		{
			dateTime = time2;
		}
		else
		{
			if (num >= 0)
			{
				return time1;
			}
			dateTime = time1;
		}
		int value = num;
		if (num <= int.MinValue)
		{
			value = -2147483647;
		}
		int num2 = random.Next(Math.Abs(value));
		return dateTime.AddSeconds(num2);
	}

	public string TimeStamp()
	{
		return DateTime.Now.ToString("yyyyMMddhhmmss");
	}

	public string YearStamp()
	{
		return DateTime.Now.ToString("yyyy");
	}

	public string MonthStamp()
	{
		return DateTime.Now.ToString("MM");
	}

	public string DayStamp()
	{
		return DateTime.Now.ToString("dd");
	}

	public void Write_Txt(string FileName, string Content)
	{
		Encoding encoding = Encoding.GetEncoding("gb2312");
		string text = AppDomain.CurrentDomain.BaseDirectory + FileName + ".txt";
		StreamWriter streamWriter = null;
		try
		{
			streamWriter = new StreamWriter(text, append: false, encoding);
			streamWriter.Write(Content);
			streamWriter.Flush();
		}
		catch
		{
		}
		streamWriter.Close();
		streamWriter.Dispose();
	}

	public string Read_Txt(string FileName)
	{
		Encoding encoding = Encoding.GetEncoding("gb2312");
		string text = AppDomain.CurrentDomain.BaseDirectory + FileName + ".txt";
		string result = "";
		if (File.Exists(text))
		{
			StreamReader streamReader = null;
			try
			{
				streamReader = new StreamReader(text, encoding);
				result = streamReader.ReadToEnd();
			}
			catch
			{
			}
			streamReader.Close();
			streamReader.Dispose();
		}
		else
		{
			result = "";
		}
		return result;
	}

	public void WriteFile(string Path, string Strings)
	{
		if (!File.Exists(Path))
		{
			FileStream fileStream = File.Create(Path);
			fileStream.Close();
			fileStream.Dispose();
		}
		StreamWriter streamWriter = new StreamWriter(Path, append: true, Encoding.UTF8);
		streamWriter.WriteLine(Strings);
		streamWriter.Close();
		streamWriter.Dispose();
	}

	public void WriteFileDefault(string Path, string Strings)
	{
		if (!File.Exists(Path))
		{
			FileStream fileStream = File.Create(Path);
			fileStream.Close();
			fileStream.Dispose();
		}
		StreamWriter streamWriter = new StreamWriter(Path, append: true, Encoding.Default);
		streamWriter.WriteLine(Strings);
		streamWriter.Close();
		streamWriter.Dispose();
	}

	public string ReadFile(string Path)
	{
		string text = "";
		if (!File.Exists(Path))
		{
			text = "FILE NOT EXIST!!!";
		}
		else
		{
			StreamReader streamReader = new StreamReader(Path, Encoding.GetEncoding("gb2312"));
			text = streamReader.ReadToEnd();
			streamReader.Close();
			streamReader.Dispose();
		}
		return text;
	}

	public string GetPostStr(string filename)
	{
		int num = filename.LastIndexOf(".");
		int length = filename.Length;
		return filename.Substring(num, length - num);
	}

	public void FileAdd(string Path, string strings)
	{
		StreamWriter streamWriter = File.AppendText(Path);
		streamWriter.Write(strings);
		streamWriter.Flush();
		streamWriter.Close();
		streamWriter.Dispose();
	}

	public void FileCreate(string Path)
	{
		FileInfo fileInfo = new FileInfo(Path);
		if (!fileInfo.Exists)
		{
			fileInfo.Create().Close();
		}
	}

	public void FileCopy(string OrignFile, string NewFile)
	{
		File.Copy(OrignFile, NewFile, overwrite: true);
	}

	public void FileMove(string OrignFile, string NewFile)
	{
		File.Move(OrignFile, NewFile);
	}

	public void FileDelete(string Path)
	{
		File.Delete(Path);
	}

	public void FolderCreate(string OrignFolder, string NewFloder)
	{
		Directory.SetCurrentDirectory(OrignFolder);
		Directory.CreateDirectory(NewFloder);
	}

	public void FolderCreate2(string Path)
	{
		if (!Directory.Exists(Path))
		{
			Directory.CreateDirectory(Path);
		}
	}

	public void FolderDelete(string dir)
	{
		if (!Directory.Exists(dir))
		{
			return;
		}
		string[] fileSystemEntries = Directory.GetFileSystemEntries(dir);
		foreach (string dir2 in fileSystemEntries)
		{
			if (File.Exists(dir2))
			{
				File.Delete(dir2);
			}
			else
			{
				FolderDelete(dir2);
			}
		}
		Directory.Delete(dir, recursive: true);
	}

	public void CopyDir(string srcPath, string aimPath)
	{
		try
		{
			if (aimPath[aimPath.Length - 1] != Path.DirectorySeparatorChar)
			{
				aimPath += Path.DirectorySeparatorChar;
			}
			if (!Directory.Exists(aimPath))
			{
				Directory.CreateDirectory(aimPath);
			}
			string[] fileSystemEntries = Directory.GetFileSystemEntries(srcPath);
			foreach (string text in fileSystemEntries)
			{
				if (Directory.Exists(text))
				{
					CopyDir(text, aimPath + Path.GetFileName(text));
				}
				else
				{
					File.Copy(text, aimPath + Path.GetFileName(text), overwrite: true);
				}
			}
		}
		catch (Exception ex)
		{
			throw new Exception(ex.ToString());
		}
	}

	public long GetDirectoryLength(string dirPath)
	{
		if (!Directory.Exists(dirPath))
		{
			return 0L;
		}
		long num = 0L;
		DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
		FileInfo[] files = directoryInfo.GetFiles();
		foreach (FileInfo fileInfo in files)
		{
			num += fileInfo.Length;
		}
		DirectoryInfo[] directories = directoryInfo.GetDirectories();
		if (directories.Length != 0)
		{
			for (int j = 0; j < directories.Length; j++)
			{
				num += GetDirectoryLength(directories[j].FullName);
			}
		}
		return num;
	}

	public string GetFileAtt(string filePath)
	{
		string text = "";
		FileInfo fileInfo = new FileInfo(filePath);
		return text + "Path:" + fileInfo.FullName + "\r\nFile Name:" + fileInfo.Name + "\r\nFile Length:" + fileInfo.Length + "\r\nBuild Time" + fileInfo.CreationTime.ToString() + "\r\nLast Visit Time:" + fileInfo.LastAccessTime.ToString() + "\r\nModify Time:" + fileInfo.LastWriteTime.ToString() + "\r\nFolder:" + fileInfo.DirectoryName + "\r\nExtension:" + fileInfo.Extension;
	}

	public string GetFoldAll(string Path, string DropName, string tplPath)
	{
		string text = "<select name=\"" + DropName + "\" id=\"" + DropName + "\"><option value=\"\">--请选择详细模板--</option>";
		string rn = "";
		DirectoryInfo theDir = new DirectoryInfo(Path);
		rn = ListTreeShow(theDir, 0, rn, tplPath);
		return text + rn + "</select>";
	}

	public string ListTreeShow(DirectoryInfo theDir, int nLevel, string Rn, string tplPath)
	{
		DirectoryInfo[] directories = theDir.GetDirectories();
		foreach (DirectoryInfo directoryInfo in directories)
		{
			Rn = Rn + "<option value=\"" + directoryInfo.Name.ToString() + "\"";
			if (tplPath.ToLower() == directoryInfo.Name.ToString().ToLower())
			{
				Rn += " selected ";
			}
			Rn += ">";
			if (nLevel == 0)
			{
				Rn += "┣";
			}
			else
			{
				string text = "";
				for (int j = 1; j <= nLevel; j++)
				{
					text += "│&nbsp;";
				}
				Rn = Rn + text + "┣";
			}
			Rn = Rn + directoryInfo.Name.ToString() + "</option>";
			FileInfo[] files = directoryInfo.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				Rn = Rn + "<option value=\"" + directoryInfo.Name.ToString() + "/" + fileInfo.Name.ToString() + "\"";
				if (tplPath.ToLower() == fileInfo.Name.ToString().ToLower())
				{
					Rn += " selected ";
				}
				Rn += ">";
				if (nLevel == 0)
				{
					Rn += "│&nbsp;├";
				}
				else
				{
					string text2 = "";
					for (int l = 1; l <= nLevel; l++)
					{
						text2 += "│&nbsp;";
					}
					Rn = Rn + text2 + "│&nbsp;├";
				}
				Rn = Rn + fileInfo.Name.ToString() + "</option>";
			}
			Rn = ListTreeShow(directoryInfo, nLevel + 1, Rn, tplPath);
		}
		return Rn;
	}

	public int StrLength(string Input)
	{
		ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
		int num = 0;
		byte[] bytes = aSCIIEncoding.GetBytes(Input);
		for (int i = 0; i < bytes.Length; i++)
		{
			num = ((bytes[i] != 63) ? (num + 1) : (num + 2));
		}
		return num;
	}

	public string[] StrToArray(string Input, char Seperater)
	{
		return Input.Split(Seperater);
	}

	public string[] StrToArray2(string Input, string Separater)
	{
		string[] result = null;
		if (Input != null && Input != "")
		{
			result = new Regex(Separater).Split(Input);
		}
		return result;
	}

	public string StrClean(string Input, string Separater)
	{
		if (Input == null)
		{
			return "";
		}
		return Input.Replace(Separater, "");
	}

	public string DelPostChar(string Input, string Separater)
	{
		return Input.Substring(0, Input.LastIndexOf(Separater));
	}

	public string DelPreChar(string Input, string Separater)
	{
		return Input.Substring(Input.LastIndexOf(Separater) + 1);
	}

	public int StrToInt(string Input)
	{
		if (IsNumberId(Input))
		{
			return int.Parse(Input);
		}
		return 0;
	}

	public List<string> StrToList(string Input, char Separater, bool ToLower)
	{
		List<string> list = new List<string>();
		string[] array = Input.Split(Separater);
		foreach (string text in array)
		{
			if (!string.IsNullOrEmpty(text) && text != Separater.ToString())
			{
				string item = text;
				if (ToLower)
				{
					item = text.ToLower();
				}
				list.Add(item);
			}
		}
		return list;
	}

	public string ListToStr(List<string> List, string Separater)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < List.Count; i++)
		{
			if (i == List.Count - 1)
			{
				stringBuilder.Append(List[i]);
				continue;
			}
			stringBuilder.Append(List[i]);
			stringBuilder.Append(Separater);
		}
		return stringBuilder.ToString();
	}

	public string IntListToStr(List<int> List)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < List.Count; i++)
		{
			if (i == List.Count - 1)
			{
				stringBuilder.Append(List[i].ToString());
				continue;
			}
			stringBuilder.Append(List[i]);
			stringBuilder.Append(",");
		}
		return stringBuilder.ToString();
	}

	public string DicListToStr(Dictionary<int, int> List)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<int, int> item in List)
		{
			stringBuilder.Append(item.Value + ",");
		}
		if (List.Count > 0)
		{
			return DelPostChar(stringBuilder.ToString(), "");
		}
		return "";
	}

	public string StrToSBC(string Input)
	{
		char[] array = Input.ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == ' ')
			{
				array[i] = '\u3000';
			}
			else if (array[i] < '\u007f')
			{
				array[i] = (char)(array[i] + 65248);
			}
		}
		return new string(array);
	}

	public string StrToDBC(string Input)
	{
		char[] array = Input.ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == '\u3000')
			{
				array[i] = ' ';
			}
			else if (array[i] > '\uff00' && array[i] < '｟')
			{
				array[i] = (char)(array[i] - 65248);
			}
		}
		return new string(array);
	}

	public string StrToSafeStr(string Input, bool IsDel)
	{
		if (IsDel)
		{
			Input = Input.Replace("'", "");
			Input = Input.Replace("\"", "");
			return Input;
		}
		Input = Input.Replace("'", "&#39;");
		Input = Input.Replace("\"", "&#34;");
		return Input;
	}

	public bool IsNumberId(string Input)
	{
		return RegularVer(Input, "^[1-9]*[0-9]*$");
	}

	public static bool RegularVer(string Input, string Express)
	{
		if (Input == null)
		{
			return false;
		}
		Regex regex = new Regex(Express);
		if (Input.Length == 0)
		{
			return false;
		}
		return regex.IsMatch(Input);
	}

	public static string HtmlToTxt(string strHtml)
	{
		string[] array = new string[15]
		{
			"<script[^>]*?>.*?</script>", "<(\\/\\s*)?!?((\\w+:)?\\w+)(\\w+(\\s*=?\\s*(([\"'])(\\\\[\"'tbnr]|[^\\7])*?\\7|\\w+)|.{0})|\\s)*?(\\/\\s*)?>", "([\\r\\n])[\\s]+", "&(quot|#34);", "&(amp|#38);", "&(lt|#60);", "&(gt|#62);", "&(nbsp|#160);", "&(iexcl|#161);", "&(cent|#162);",
			"&(pound|#163);", "&(copy|#169);", "&#(\\d+);", "-->", "<!--.*\\n"
		};
		_ = array[0];
		string text = strHtml;
		for (int i = 0; i < array.Length; i++)
		{
			text = new Regex(array[i], RegexOptions.IgnoreCase).Replace(text, string.Empty);
		}
		text.Replace("<", "");
		text.Replace(">", "");
		text.Replace("\r\n", "");
		return text;
	}

	public bool IsNullOrEmpty(object data)
	{
		if (data == null)
		{
			return true;
		}
		if (data.GetType() == typeof(string) && string.IsNullOrEmpty(data.ToString().Trim()))
		{
			return true;
		}
		if (data.GetType() == typeof(DBNull))
		{
			return true;
		}
		return false;
	}

	public byte[] StringToBytes(string Input, Encoding Default)
	{
		return Default.GetBytes(Input);
	}

	public string BytesToString(byte[] Input, Encoding Default)
	{
		return Default.GetString(Input);
	}

	public byte[] HexStringToByteArray(string Input)
	{
		Input = Input.Replace(" ", "");
		byte[] array = new byte[Input.Length / 2];
		for (int i = 0; i < Input.Length; i += 2)
		{
			array[i / 2] = Convert.ToByte(Input.Substring(i, 2), 16);
		}
		return array;
	}

	public string ByteArrayToHexString(byte[] data)
	{
		StringBuilder stringBuilder = new StringBuilder(data.Length * 3);
		foreach (byte b in data)
		{
			stringBuilder.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
		}
		return stringBuilder.ToString().ToUpper().Replace(" ", "");
	}

	public string GetCRC16ByPoly(byte[] Cmd, ushort Poly, string Model)
	{
		_ = new byte[2];
		ushort num = 0;
		string text = "";
		if (Model == "Modbus" || Model == "CCITT_0xFFFF")
		{
			num = ushort.MaxValue;
		}
		if (Model == "IBM" || Model == "CCITT_Kermit")
		{
			num = 0;
		}
		if (Model != "CCITT_0xFFFF")
		{
			for (int i = 0; i < Cmd.Length; i++)
			{
				num ^= Cmd[i];
				for (int j = 0; j < 8; j++)
				{
					num = (((num & 1) == 0) ? ((ushort)(num >> 1)) : ((ushort)((num >> 1) ^ Poly)));
				}
			}
			text = ByteArrayToHexString(BitConverter.GetBytes(num));
			if (Model == "Modbus" || Model == "IBM")
			{
				text = text.Substring(2, 2) + text.Substring(0, 2);
			}
		}
		else
		{
			for (int k = 0; k < Cmd.Length; k++)
			{
				for (int l = 0; l < 8; l++)
				{
					int num2 = ((Cmd[k] << l) & 0x80) ^ ((num & 0x8000) >> 8);
					num <<= 1;
					if (num2 != 0)
					{
						num ^= 0x1021;
					}
				}
			}
			text = DecimalConvert(num.ToString(), 16).ToUpper();
		}
		return text;
	}

	public string BinaryConvert(string Input, int ToType)
	{
		string result = string.Empty;
		switch (ToType)
		{
		case 8:
			result = Convert.ToString(Convert.ToInt32(Input, 2), 8);
			break;
		case 10:
			result = Convert.ToInt32(Input, 2).ToString();
			break;
		case 16:
			result = Convert.ToString(Convert.ToInt32(Input, 2), 16);
			break;
		}
		return result;
	}

	public string DecimalConvert(string Input, int ToType)
	{
		int num = Convert.ToInt32(Input);
		return ToType switch
		{
			2 => Convert.ToString(num, 2), 
			8 => Convert.ToString(num, 8), 
			16 => Convert.ToString(num, 16), 
			_ => Input, 
		};
	}

	public string HexConvert(string Input, int ToType)
	{
		string result = string.Empty;
		switch (ToType)
		{
		case 2:
			result = Convert.ToString(Convert.ToInt32(Input, 16), 2).PadLeft(4, '0');
			break;
		case 8:
			result = Convert.ToString(Convert.ToInt32(Input, 16), 8);
			break;
		case 10:
			result = Convert.ToInt32(Input, 16).ToString();
			break;
		}
		return result;
	}

	public string BinaryPlusBinary(string Input_x, string Input_Y)
	{
		return Convert.ToString(Convert.ToInt32(Input_x, 2) + Convert.ToInt32(Input_Y, 2), 2);
	}

	public string StringReverse(string Input)
	{
		char[] array = Input.ToCharArray();
		Array.Reverse(array);
		return new string(array).Trim();
	}

	public string String2ASCII(string Input)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(Input);
		string text = null;
		byte[] array = bytes;
		foreach (int num in array)
		{
			text += Convert.ToString(num);
		}
		return text;
	}

	public string String2ASCII16(string Input)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(Input);
		string text = null;
		byte[] array = bytes;
		foreach (int num in array)
		{
			text += num.ToString("X2");
		}
		return text;
	}

	public string StringAdd(string Input, char Seperater)
	{
		string text = null;
		for (int i = 0; i < Input.Length; i++)
		{
			text = text + Input[i] + Seperater;
		}
		return DelPostChar(text, ":");
	}

	public string StringInsert(string input, int interval, string value)
	{
		for (int i = interval; i < input.Length; i += interval + 1)
		{
			input = input.Insert(i, value);
		}
		return input;
	}

	public float FloatStrToFloat(string Input)
	{
		float result = 0f;
		float.TryParse(Input, out result);
		return result;
	}

	public string SUBSTR(string Input, int m, int n)
	{
		return Input.Substring(m - 1, n);
	}

	public string TRIM(string Input)
	{
		return Input.Trim();
	}

	public int INDEX(string Input, string Parameter)
	{
		return Input.IndexOf(Parameter);
	}

	public string TOSTRING(int Input)
	{
		return Input.ToString();
	}

	public string TOSTRING(float Input)
	{
		return Input.ToString();
	}

	public string TOSTRING(double Input)
	{
		return Input.ToString();
	}

	public string TOUPPER(string Input)
	{
		return Input.ToUpper();
	}

	public string TOLOWER(string Input)
	{
		return Input.ToLower();
	}

	public string REPLACE(string Input, string Old, string New)
	{
		return Input.Replace(Old, New);
	}

	public string CurrentPath()
	{
		return AppDomain.CurrentDomain.BaseDirectory.ToString();
	}

	public void SetXMLPath(string strPath)
	{
		_XMLPath = strPath;
	}

	private XmlDocument XMLLoad()
	{
		string xMLPath = XMLPath;
		XmlDocument xmlDocument = new XmlDocument();
		try
		{
			string filename = xMLPath;
			if (File.Exists(filename))
			{
				xmlDocument.Load(filename);
				return xmlDocument;
			}
			return xmlDocument;
		}
		catch (Exception)
		{
			return xmlDocument;
		}
	}

	public string XMLReadNode(string node)
	{
		string result = "";
		try
		{
			result = XMLLoad().SelectSingleNode(node).InnerText;
			return result;
		}
		catch
		{
			return result;
		}
	}

	public void XMLModifyNode(string node, string value)
	{
		try
		{
			XmlDocument xmlDocument = XMLLoad();
			xmlDocument.SelectSingleNode(node).InnerText = value;
			xmlDocument.Save(XMLPath);
		}
		catch
		{
		}
	}

	public void NodeAttributeModify(string node, string attribute, string value)
	{
		try
		{
			XmlDocument xmlDocument = XMLLoad();
			XmlElement xmlElement = (XmlElement)xmlDocument.SelectSingleNode(node);
			if (attribute.Equals(""))
			{
				xmlElement.InnerText = value;
			}
			else
			{
				xmlElement.SetAttribute(attribute, value);
			}
			xmlDocument.Save(XMLPath);
		}
		catch
		{
		}
	}

	public void XMLDeleteNode(string node)
	{
		try
		{
			XmlDocument xmlDocument = XMLLoad();
			XmlNode xmlNode = xmlDocument.SelectSingleNode(node);
			xmlNode.ParentNode.RemoveChild(xmlNode);
			xmlDocument.Save(XMLPath);
		}
		catch
		{
		}
	}

	public void XMLInsertNode(string node, string element, string attribute, string value)
	{
		try
		{
			XmlDocument xmlDocument = XMLLoad();
			XmlNode xmlNode = xmlDocument.SelectSingleNode(node);
			if (element.Equals(""))
			{
				if (!attribute.Equals(""))
				{
					((XmlElement)xmlNode).SetAttribute(attribute, value);
				}
			}
			else
			{
				XmlElement xmlElement = xmlDocument.CreateElement(element);
				if (attribute.Equals(""))
				{
					xmlElement.InnerText = value;
				}
				else
				{
					xmlElement.SetAttribute(attribute, value);
				}
				xmlNode.AppendChild(xmlElement);
			}
			xmlDocument.Save(XMLPath);
		}
		catch
		{
		}
	}

	public void CreateXmlSample()
	{
		XmlDocument xmlDocument = new XmlDocument();
		XmlNode newChild = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", "");
		xmlDocument.AppendChild(newChild);
		XmlNode xmlNode = xmlDocument.CreateElement("Teradyne");
		xmlDocument.AppendChild(xmlNode);
		XmlNode xmlNode2 = xmlDocument.CreateNode(XmlNodeType.Element, "machineID1", null);
		ChildNode(xmlDocument, xmlNode2, "model", "TSLH");
		ChildNode(xmlDocument, xmlNode2, "type", "Offline");
		ChildNode(xmlDocument, xmlNode2, "price", "20k");
		xmlNode.AppendChild(xmlNode2);
		XmlNode xmlNode3 = xmlDocument.CreateNode(XmlNodeType.Element, "machineID2", null);
		ChildNode(xmlDocument, xmlNode3, "mode", "TSH");
		ChildNode(xmlDocument, xmlNode3, "type", "Inline");
		ChildNode(xmlDocument, xmlNode3, "price", "40k");
		xmlNode.AppendChild(xmlNode3);
		xmlDocument.Save(CurrentPath() + "KCSample.xml");
	}

	public void ChildNode(XmlDocument xmlDoc, XmlNode parentNode, string name, string value)
	{
		XmlNode xmlNode = xmlDoc.CreateNode(XmlNodeType.Element, name, null);
		xmlNode.InnerText = value;
		parentNode.AppendChild(xmlNode);
	}

	public int GetValidPort(string port)
	{
		int result = -1;
		try
		{
			if (port == "")
			{
				throw new Exception("Empty Port !!!");
			}
			if (Convert.ToInt32(port) < 0 || Convert.ToInt32(port) > 65535)
			{
				throw new Exception("Invalid Port !!!");
			}
			result = Convert.ToInt32(port);
			return result;
		}
		catch (Exception ex)
		{
			_ = ex.Message;
			return result;
		}
	}

	public IPAddress StringToIPAddress(string ip)
	{
		return IPAddress.Parse(ip);
	}

	public string GetRemoteClientIP(Socket clientSocket)
	{
		return ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString();
	}

	public IPEndPoint CreateIPEndPoint(string ip, int port)
	{
		return new IPEndPoint(StringToIPAddress(ip), port);
	}

	public TcpListener CreateTcpListener(string ip, int port)
	{
		return new TcpListener(new IPEndPoint(StringToIPAddress(ip), port));
	}

	public Socket CreateTcpSocket()
	{
		return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
	}

	public Socket CreateUdpSocket()
	{
		return new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
	}

	public IPEndPoint GetLocalPoint(TcpListener tcpListener)
	{
		return (IPEndPoint)tcpListener.LocalEndpoint;
	}

	public string GetLocalPoint_IP(TcpListener tcpListener)
	{
		return ((IPEndPoint)tcpListener.LocalEndpoint).Address.ToString();
	}

	public static int GetLocalPoint_Port(TcpListener tcpListener)
	{
		return ((IPEndPoint)tcpListener.LocalEndpoint).Port;
	}

	public void SelectEndPoint(Socket socket, IPEndPoint endPoint)
	{
		if (!socket.IsBound)
		{
			socket.Bind(endPoint);
		}
	}

	public void SelectEndPoint(Socket socket, string ip, int port)
	{
		IPEndPoint localEP = CreateIPEndPoint(ip, port);
		if (!socket.IsBound)
		{
			socket.Bind(localEP);
		}
	}

	public void SocketStartListen(Socket socket, int port, int maxConnection)
	{
		IPEndPoint endPoint = CreateIPEndPoint(GetLocalHostName, port);
		SelectEndPoint(socket, endPoint);
		socket.Listen(maxConnection);
	}

	public void StartListen(Socket socket, string ip, int port, int maxConnection)
	{
		SelectEndPoint(socket, ip, port);
		socket.Listen(maxConnection);
	}

	public bool Connect2Server(Socket socket, string ip, int port)
	{
		try
		{
			socket.Connect(ip, port);
			return socket.Poll(-1, SelectMode.SelectWrite);
		}
		catch (SocketException ex)
		{
			throw new Exception(ex.Message);
		}
	}

	public void SendMsg(Socket socket, byte[] msg)
	{
		socket.Send(msg, msg.Length, SocketFlags.None);
	}

	public void SendMsg(Socket socket, string msg)
	{
		byte[] array = StringToBytes(msg, Encoding.Default);
		socket.Send(array, array.Length, SocketFlags.None);
	}

	public void ReceiveMsg(Socket socket, byte[] buffer)
	{
		socket.Receive(buffer);
	}

	public string ReceiveMsg(Socket socket)
	{
		byte[] array = new byte[5000];
		int num = socket.Receive(array);
		byte[] array2 = new byte[num];
		Buffer.BlockCopy(array, 0, array2, 0, num);
		return BytesToString(array2, Encoding.Default);
	}

	public void Disconnect(Socket socket)
	{
		try
		{
			socket.Shutdown(SocketShutdown.Both);
		}
		catch (SocketException ex)
		{
			throw ex;
		}
		finally
		{
			socket.Close();
		}
	}

	public bool SendEmail(string receiveEmail, string msgSubject, string msgBody, bool IsEnableSSL)
	{
		MailMessage mailMessage = new MailMessage();
		mailMessage.To.Add(receiveEmail);
		mailMessage.Subject = msgSubject;
		mailMessage.Body = msgBody;
		mailMessage.IsBodyHtml = true;
		SmtpClient smtpClient = new SmtpClient();
		smtpClient.EnableSsl = IsEnableSSL;
		try
		{
			smtpClient.Send(mailMessage);
			return true;
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public void SQLDBInit(string Input)
	{
		SqlConnectionString = Input;
		SQLConnect = new SqlConnection(Input);
		Dataset = new DataSet();
	}

	public void SQLDBConnect()
	{
		if (SQLConnect.State == ConnectionState.Closed)
		{
			SQLConnect.Open();
		}
	}

	public void SQLDBDisConnect()
	{
		if (SQLConnect != null)
		{
			if (SQLConnect.State == ConnectionState.Open)
			{
				SQLConnect.Close();
			}
			SQLConnect.Dispose();
		}
	}

	public void SQLDBCMD(string Input)
	{
		cmd = new SqlCommand(Input, SQLConnect);
		data = new SqlDataAdapter(cmd);
		data.Fill(Dataset);
	}

	public string SQLDBQuery(string Input, string Parameter)
	{
		string text = string.Empty;
		cmd = new SqlCommand(Input, SQLConnect);
		SqlDataReader sqlDataReader = cmd.ExecuteReader();
		while (sqlDataReader.Read())
		{
			text = text + sqlDataReader[Parameter].ToString() + ",";
		}
		sqlDataReader.Close();
		return text;
	}

	public void ProessStart(string Input, string Parameter)
	{
		if (Parameter == "")
		{
			Process.Start(Input);
		}
		else
		{
			Process.Start(Input, Parameter);
		}
	}

	public void ProessKill(string Input)
	{
		Process[] processes = Process.GetProcesses();
		for (int i = 0; i < processes.GetLength(0); i++)
		{
			if (processes[i].ProcessName.Equals(Input))
			{
				processes[i].Kill();
			}
		}
	}

	public string ProessGetTitle(string Input)
	{
		Process[] processes = Process.GetProcesses();
		string result = string.Empty;
		for (int i = 0; i < processes.GetLength(0); i++)
		{
			if (processes[i].ProcessName.Equals(Input))
			{
				result = processes[i].MainWindowTitle;
			}
		}
		return result;
	}

	public string[] ProessGetAll()
	{
		Process[] processes = Process.GetProcesses();
		string[] array = new string[1024];
		for (int i = 0; i < processes.GetLength(0); i++)
		{
			array[i] = "Id = " + processes[i].Id + "   Name = " + processes[i].ProcessName;
		}
		return array;
	}

	public string ProessGetNameByID(int Input)
	{
		_ = string.Empty;
		try
		{
			return Process.GetProcessById(Input).ProcessName;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public int ProessGetIDByName(string Input)
	{
		return Process.GetProcessesByName(Input)[0].Id;
	}

	public string[] ProessGetModuleAll()
	{
		string[] array = new string[1024];
		ProcessModuleCollection modules = Process.GetCurrentProcess().Modules;
		int num = 0;
		foreach (ProcessModule item in modules)
		{
			array[num] = "Module Name=" + item.ModuleName + "   File Name=" + item.FileName + "   Version=" + item.FileVersionInfo.FileVersion;
			num++;
		}
		return array;
	}

	public string ProessCMD(string CMDStr)
	{
		Process process = new Process();
		process.StartInfo.FileName = "cmd.exe";
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.RedirectStandardInput = true;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.CreateNoWindow = true;
		process.Start();
		process.StandardInput.WriteLine(CMDStr + "&exit");
		process.StandardInput.AutoFlush = true;
		return process.StandardOutput.ReadToEnd();
	}

	private static void MakeVarByteSingle(StreamWriter writerVar, uint d)
	{
		uint num = 1u;
		for (int i = 0; i < 32; i++)
		{
			if ((d & num) != 0)
			{
				writerVar.Write("1");
			}
			else
			{
				writerVar.Write("0");
			}
			writerVar.Write(" ");
			num <<= 1;
		}
	}

	private static void MakeVarBytePanel(StreamWriter writerVar, uint[] d, int width)
	{
		uint num = 1u;
		for (int i = 0; i < 32; i++)
		{
			for (int j = 0; j < width; j++)
			{
				if ((d[j] & num) != 0)
				{
					writerVar.Write("1");
				}
				else
				{
					writerVar.Write("0");
				}
			}
			writerVar.Write(" ");
			num <<= 1;
		}
	}

	private static void MakeDataUnit(StreamWriter writerVar, int unit, uint Address, byte[,] image)
	{
		uint num = Address + 32768;
		for (uint num2 = Address; num2 < num; num2 += 4)
		{
			uint num3 = 0u;
			num3 = Convert.ToUInt32(image[unit, num2 - Address] & 0xFF);
			num3 |= Convert.ToUInt32((image[unit, num2 - Address + 1] << 8) & 0xFF00);
			num3 |= Convert.ToUInt32((image[unit, num2 - Address + 2] << 16) & 0xFF0000);
			num3 |= Convert.ToUInt32((image[unit, num2 - Address + 3] << 24) & 0xFF000000u);
			MakeVarByteSingle(writerVar, num3);
			if (num2 % 16 == 0)
			{
				writerVar.WriteLine(" /* " + $"{num2:X8}" + " */");
			}
			else
			{
				writerVar.WriteLine();
			}
		}
	}

	private static void MakeDataPanel(StreamWriter writerVar, int width, uint Address, byte[,] image)
	{
		uint num = Address + 32768;
		for (uint num2 = Address; num2 < num; num2 += 4)
		{
			uint[] array = new uint[width];
			for (int i = 0; i < width; i++)
			{
				array[i] = Convert.ToUInt32(image[i, num2 - Address] & 0xFF);
			}
			for (int j = 0; j < width; j++)
			{
				array[j] |= Convert.ToUInt32((image[j, num2 - Address + 1] << 8) & 0xFF00);
			}
			for (int k = 0; k < width; k++)
			{
				array[k] |= Convert.ToUInt32((image[k, num2 - Address + 2] << 16) & 0xFF0000);
			}
			for (int l = 0; l < width; l++)
			{
				array[l] |= Convert.ToUInt32((image[l, num2 - Address + 3] << 24) & 0xFF000000u);
			}
			MakeVarBytePanel(writerVar, array, width);
			if (num2 % 16 == 0)
			{
				writerVar.WriteLine(" /* " + $"{num2:X8}" + " */");
			}
			else
			{
				writerVar.WriteLine();
			}
		}
	}

	public int ContiMQBBin2DDS(string DUTNAME, string ADDRESS, string PATH, int SiteID)
	{
		if (!Regex.IsMatch(DUTNAME, "^[0-9A-Z_]+$"))
		{
			return -1;
		}
		if (!Regex.IsMatch(ADDRESS, "^[0-9A-Fa-f]+$"))
		{
			return -2;
		}
		uint address = Convert.ToUInt32(ADDRESS, 16);
		int num = 4;
		byte[,] array = new byte[num, 32768];
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < 32768; j++)
			{
				array[i, j] = byte.MaxValue;
			}
		}
		string text = "";
		for (int k = 0; k < num; k++)
		{
			if (SiteID == 1)
			{
				text = PATH + "UNIT" + (k + 1) + ".BIN";
			}
			if (SiteID == 2)
			{
				text = PATH + "UNIT" + (k + 5) + ".BIN";
			}
			if (File.Exists(text))
			{
				BinaryReader binaryReader = new BinaryReader(File.Open(text, FileMode.Open));
				int num2 = (int)binaryReader.BaseStream.Length;
				if (num2 > 32768)
				{
					return -3;
				}
				for (int l = 0; l < num2; l++)
				{
					array[k, l] = binaryReader.ReadByte();
				}
				binaryReader.Close();
				continue;
			}
			return -4;
		}
		string text2 = PATH + DUTNAME + "_VAR_DATA.DDS";
		string text3 = PATH + DUTNAME + "_VAR_DATA.DDS";
		if (File.Exists(text3))
		{
			File.Delete(text3);
		}
		string text4 = PATH + DUTNAME + "_VAR_DATA.DDB";
		if (File.Exists(text4))
		{
			File.Delete(text4);
		}
		StreamWriter streamWriter = new StreamWriter(text2);
		streamWriter.WriteLine("/*************************************************************************");
		streamWriter.WriteLine("**");
		streamWriter.WriteLine("** This DDS file was created by MPC560_OTF " + DateTime.Now.ToString());
		streamWriter.WriteLine("** TERADYNE SHANGHAI CORP -KenCai");
		streamWriter.WriteLine("** Representing variable data programmed to Data Flash of MPC560");
		streamWriter.WriteLine("** Device Name " + DUTNAME);
		streamWriter.WriteLine("** Number of Units " + num);
		streamWriter.WriteLine("**");
		streamWriter.WriteLine("*************************************************************************/");
		if (num > 1)
		{
			streamWriter.WriteLine("BEGIN '" + DUTNAME + "_PANEL_VAR_DATA'");
			streamWriter.WriteLine("WIDTH = " + num);
			streamWriter.WriteLine("DSMDOUBLE");
			streamWriter.WriteLine("USERINFO = ");
			streamWriter.WriteLine(string.Concat("'", DateTime.Now, "'\n"));
			MakeDataPanel(streamWriter, num, address, array);
			streamWriter.WriteLine("END '" + DUTNAME + "_PANEL_VAR_DATA'");
			for (int m = 0; m < num; m++)
			{
				streamWriter.WriteLine("BEGIN '" + DUTNAME + "_UNIT" + Convert.ToString(m + 1) + "_VAR_DATA'");
				streamWriter.WriteLine("WIDTH = 1");
				streamWriter.WriteLine("DSMDOUBLE");
				streamWriter.WriteLine("USERINFO = ");
				streamWriter.WriteLine(string.Concat("'", DateTime.Now, "'\n"));
				MakeDataUnit(streamWriter, m, address, array);
				streamWriter.WriteLine("END '" + DUTNAME + "_UNIT" + Convert.ToString(m + 1) + "_VAR_DATA'");
			}
		}
		streamWriter.Close();
		return 0;
	}

	public int ContiMQB512KBin2DDS(string DUTNAME, string FILENAME, string SIZE, string UUT_COUNT, string PATH)
	{
		int num = Convert.ToInt32(SIZE) * 1024;
		int uUT_count = Convert.ToInt32(UUT_COUNT);
		byte[] array = (array = new byte[num]);
		if (File.Exists(FILENAME))
		{
			BinaryReader binaryReader = new BinaryReader(File.OpenRead(FILENAME));
			if ((int)binaryReader.BaseStream.Length > num)
			{
				return -1;
			}
			for (int i = 0; i < num; i++)
			{
				array[i] = binaryReader.ReadByte();
			}
			binaryReader.Close();
			string text = DUTNAME + ".DDS";
			string text2 = PATH + DUTNAME + ".DDS";
			if (File.Exists(text2))
			{
				File.Delete(text2);
			}
			string text3 = PATH + DUTNAME + ".DDB";
			if (File.Exists(text3))
			{
				File.Delete(text3);
			}
			StreamWriter streamWriter = new StreamWriter(text + ".TMP");
			streamWriter.WriteLine("/******************************************************");
			streamWriter.WriteLine("** This DDS file was updated by MPC560BIN2DDS");
			streamWriter.WriteLine("** TERADYNE SHANGHAI CORP -KenCai");
			streamWriter.WriteLine("** Main flash size = " + num);
			streamWriter.WriteLine("** Number of DUTs: " + uUT_count);
			streamWriter.WriteLine("** " + DateTime.Now.ToString());
			streamWriter.WriteLine("******************************************************/");
			if (File.Exists(text))
			{
				StreamReader streamReader = new StreamReader(text);
				bool flag = false;
				while (!streamReader.EndOfStream)
				{
					string text4 = streamReader.ReadLine();
					if (text4.Contains("/*******************************"))
					{
						flag = true;
					}
					if (text4.Contains("DATASET") && text4.Contains("BEGIN"))
					{
						flag = true;
					}
					if (text4.Contains(DUTNAME + "_PANEL_CHECKSUM") && text4.Contains("BEGIN"))
					{
						flag = true;
					}
					if (text4.Contains(DUTNAME + "_CHECKSUM") && text4.Contains("BEGIN"))
					{
						flag = true;
					}
					if (!flag)
					{
						streamWriter.WriteLine(text4);
					}
					if (text4.Contains("DATASET") && text4.Contains("END"))
					{
						flag = false;
					}
					if (text4.Contains(DUTNAME + "_PANEL_CHECKSUM") && text4.Contains("END"))
					{
						flag = false;
					}
					if (text4.Contains(DUTNAME + "_CHECKSUM") && text4.Contains("END"))
					{
						flag = false;
					}
					if (text4.Contains("*******************************/"))
					{
						flag = false;
					}
				}
				streamReader.Close();
				iCodeEnd = Convert.ToUInt32(iCodeStart + num - 1);
				sCodeEnd = $"{iCodeEnd:X8}";
				UpdateChecksum(array);
				CompleteDDS(streamWriter, array, num, uUT_count, DUTNAME);
				streamWriter.Close();
				File.Copy(DUTNAME + ".DDS.TMP", PATH + DUTNAME + ".DDS");
				File.Delete(DUTNAME + ".DDS.TMP");
				return 0;
			}
			return -3;
		}
		return -2;
	}

	private static void CompleteDDS(StreamWriter writerVar, byte[] image, int size, int UUT_count, string DUT_name)
	{
		writerVar.WriteLine("/******************************************************");
		writerVar.WriteLine("** This DDS file was updated by MPC560BIN2DDS");
		writerVar.WriteLine("** TERADYNE SHANGHAI CORP -KenCai");
		writerVar.WriteLine("** Main flash size = " + size);
		writerVar.WriteLine("** Number of DUTs: " + UUT_count);
		writerVar.WriteLine("** " + DateTime.Now.ToString());
		writerVar.WriteLine("******************************************************/");
		if (UUT_count > 1)
		{
			MakeCode(writerVar, UUT_count, image, DUT_name);
			MakeChecksum(writerVar, UUT_count, DUT_name);
		}
		MakeCode(writerVar, 1, image, DUT_name);
		MakeChecksum(writerVar, 1, DUT_name);
	}

	private static void MakeCode(StreamWriter writerVar, int count, byte[] image, string DUT_name)
	{
		string text = "";
		if (count > 1)
		{
			text = "_PANEL";
		}
		uint num = 1024u;
		writerVar.WriteLine("BEGIN '" + DUT_name + text + "_DATASET'");
		writerVar.WriteLine("WIDTH = " + count);
		writerVar.WriteLine("DSMDOUBLE");
		writerVar.WriteLine("/*********************** Program Flash ***********************/");
		string text2 = "";
		string text3 = "";
		for (int i = 0; i < count; i++)
		{
			text2 += "0";
			text3 += "1";
		}
		text2 += " ";
		text3 += " ";
		for (uint num2 = 0u; num2 < iCodeEnd - iCodeStart; num2 += num)
		{
			bool flag = true;
			for (uint num3 = num2; num3 < num2 + num; num3++)
			{
				if (image[num3] != byte.MaxValue)
				{
					flag = false;
				}
			}
			if (flag)
			{
				writerVar.WriteLine(text3 + " /* Auto-skip " + num + " bytes at " + $"{num2 | iCodeStart:X8}" + " */");
			}
			else
			{
				writerVar.WriteLine(text2 + " /* Auto-skip bit */");
				for (uint num4 = num2; num4 < num2 + num; num4 += 4)
				{
					WriteMainWord(writerVar, num4, count, image);
				}
			}
		}
		writerVar.WriteLine("END '" + DUT_name + text + "_DATASET'");
		writerVar.WriteLine();
	}

	private static void WriteMainWord(StreamWriter writerVar, uint iaddr, int count, byte[] image)
	{
		string text = "";
		string text2 = "";
		for (int i = 0; i < count; i++)
		{
			text += "0";
			text2 += "1";
		}
		text += " ";
		text2 += " ";
		if ((image[iaddr] & 1) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr] & 2) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr] & 4) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr] & 8) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr] & 0x10) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr] & 0x20) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr] & 0x40) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr] & 0x80) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr + 1] & 1) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr + 1] & 2) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr + 1] & 4) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr + 1] & 8) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr + 1] & 0x10) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr + 1] & 0x20) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr + 1] & 0x40) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr + 1] & 0x80) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr + 2] & 1) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr + 2] & 2) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr + 2] & 4) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr + 2] & 8) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr + 2] & 0x10) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr + 2] & 0x20) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr + 2] & 0x40) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr + 2] & 0x80) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr + 3] & 1) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr + 3] & 2) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr + 3] & 4) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr + 3] & 8) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr + 3] & 0x10) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr + 3] & 0x20) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr + 3] & 0x40) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if ((image[iaddr + 3] & 0x80) != 0)
		{
			writerVar.Write(text2);
		}
		else
		{
			writerVar.Write(text);
		}
		if (iaddr % 16 == 0)
		{
			writerVar.WriteLine("\t/* " + $"{iaddr | iCodeStart:X8}" + " */");
		}
		else
		{
			writerVar.WriteLine("");
		}
	}

	private static void MakeChecksum(StreamWriter writerVar, int count, string DUT_name)
	{
		string text = "";
		string text2 = "00000000";
		if (count > 1)
		{
			text = "_PANEL";
		}
		writerVar.WriteLine("BEGIN '" + DUT_name + text + "_CHECKSUM'");
		writerVar.WriteLine("WIDTH = " + count);
		writerVar.WriteLine("DSMDOUBLE");
		writerVar.WriteLine("/********************** Main Checksum ************************/");
		writerVar.WriteLine("/* Checksum of " + text2 + " to " + sCodeEnd + " = " + sCodeChecksum + " */");
		WriteChecksumWord(writerVar, iCodeChecksum, count);
		writerVar.WriteLine("END '" + DUT_name + text + "_CHECKSUM'");
		writerVar.WriteLine();
	}

	private static void WriteChecksumWord(StreamWriter writerVar, uint csword, int count)
	{
		string text = "";
		string text2 = "";
		for (int i = 0; i < count; i++)
		{
			text += "0";
			text2 += "1";
		}
		text += " ";
		text2 += " ";
		uint num = 1u;
		for (int j = 0; j < 32; j++)
		{
			if ((csword & num) != 0)
			{
				writerVar.Write(text2);
			}
			else
			{
				writerVar.Write(text);
			}
			num <<= 1;
		}
		writerVar.WriteLine("");
	}

	private static void UpdateChecksum(byte[] image)
	{
		uint num = 0u;
		num = 0u;
		for (uint num2 = iCodeStart; num2 < iCodeEnd; num2 += 4)
		{
			uint num3 = Convert.ToUInt32(image[num2 - iCodeStart]) << 24;
			num3 += Convert.ToUInt32(image[num2 - iCodeStart + 1]) << 16;
			num3 += Convert.ToUInt32(image[num2 - iCodeStart + 2]) << 8;
			num3 += Convert.ToUInt32(image[num2 - iCodeStart + 3]);
			num += num3;
		}
		iCodeChecksum = swapEndianness(num);
		sCodeChecksum = $"{iCodeChecksum:X8}";
	}

	private static uint swapEndianness(uint x)
	{
		return ((x & 0xFF) << 24) + ((x & 0xFF00) << 8) + ((x & 0xFF0000) >> 8) + ((x & 0xFF000000u) >> 24);
	}

	public int ContiAudiBin2DDS(string DUTNAME, string ADDRESS, string PATH, int SiteID)
	{
		if (!Regex.IsMatch(DUTNAME, "^[0-9A-Z_]+$"))
		{
			return -1;
		}
		if (!Regex.IsMatch(ADDRESS, "^[0-9A-Fa-f]+$"))
		{
			return -2;
		}
		uint address = Convert.ToUInt32(ADDRESS, 16);
		int num = 4;
		byte[,] array = new byte[num, 2048];
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < 2048; j++)
			{
				array[i, j] = byte.MaxValue;
			}
		}
		string text = "";
		for (int k = 0; k < num; k++)
		{
			if (SiteID == 1)
			{
				text = PATH + "UNIT" + (k + 1) + ".BIN";
			}
			if (SiteID == 2)
			{
				text = PATH + "UNIT" + (k + 5) + ".BIN";
			}
			if (File.Exists(text))
			{
				BinaryReader binaryReader = new BinaryReader(File.Open(text, FileMode.Open));
				int num2 = (int)binaryReader.BaseStream.Length;
				if (num2 > 2048)
				{
					return -3;
				}
				for (int l = 0; l < num2; l++)
				{
					array[k, l] = binaryReader.ReadByte();
				}
				binaryReader.Close();
				continue;
			}
			return -4;
		}
		string text2 = PATH + DUTNAME + "_VAR_DATA.DDS";
		string text3 = PATH + DUTNAME + "_VAR_DATA.DDS";
		if (File.Exists(text3))
		{
			File.Delete(text3);
		}
		string text4 = PATH + DUTNAME + "_VAR_DATA.DDB";
		if (File.Exists(text4))
		{
			File.Delete(text4);
		}
		StreamWriter streamWriter = new StreamWriter(text2);
		streamWriter.WriteLine("/*************************************************************************");
		streamWriter.WriteLine("**");
		streamWriter.WriteLine("** This DDS file was created In " + DateTime.Now.ToString());
		streamWriter.WriteLine("** TERADYNE SHANGHAI CORP -KenCai");
		streamWriter.WriteLine("** Representing variable data programmed to Data Flash of RH850");
		streamWriter.WriteLine("** Device Name " + DUTNAME);
		streamWriter.WriteLine("** Number of Units " + num);
		streamWriter.WriteLine("**");
		streamWriter.WriteLine("*************************************************************************/");
		if (num > 1)
		{
			streamWriter.WriteLine("BEGIN '" + DUTNAME + "_PANEL_VAR_DATA'");
			streamWriter.WriteLine("WIDTH = " + num);
			streamWriter.WriteLine("DSMDOUBLE");
			streamWriter.WriteLine("USERINFO = ");
			streamWriter.WriteLine(string.Concat("'", DateTime.Now, "'\n"));
			MakeDataPanel(streamWriter, num, "1.25K", address, array);
			streamWriter.WriteLine("END '" + DUTNAME + "_PANEL_VAR_DATA'");
			for (int m = 0; m < num; m++)
			{
				streamWriter.WriteLine("BEGIN '" + DUTNAME + "_UNIT" + Convert.ToString(m + 1) + "_VAR_DATA'");
				streamWriter.WriteLine("WIDTH = 1");
				streamWriter.WriteLine("DSMDOUBLE");
				streamWriter.WriteLine("USERINFO = ");
				streamWriter.WriteLine(string.Concat("'", DateTime.Now, "'\n"));
				MakeDataUnit(streamWriter, m, "1.25K", address, array);
				streamWriter.WriteLine("END '" + DUTNAME + "_UNIT" + Convert.ToString(m + 1) + "_VAR_DATA'");
			}
		}
		streamWriter.Close();
		return 0;
	}

	private static void MakeVarByteSingle(StreamWriter writerVar, byte d)
	{
		byte b = 128;
		for (int i = 0; i < 8; i++)
		{
			if ((d & b) != 0)
			{
				writerVar.Write("1");
			}
			else
			{
				writerVar.Write("0");
			}
			writerVar.Write(" ");
			b >>= 1;
		}
	}

	private static void MakeVarBytePanel(StreamWriter writerVar, byte[] bytes, int width)
	{
		byte b = 128;
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < width; j++)
			{
				if ((bytes[j] & b) != 0)
				{
					writerVar.Write("1");
				}
				else
				{
					writerVar.Write("0");
				}
			}
			writerVar.Write(" ");
			b >>= 1;
		}
	}

	private static void MakeDataUnit(StreamWriter writerVar, int unit, string Judge, uint Address, byte[,] image)
	{
		uint num = Address;
		if (Judge == "2K")
		{
			num += 2048;
		}
		if (Judge == "1.25K")
		{
			num += 1280;
		}
		int num2 = 0;
		for (uint num3 = Address; num3 < num; num3 += 1024)
		{
			writerVar.WriteLine("/* Data Flash, Data frame " + num2 + " */");
			byte b = 0;
			b = ((num2 != 0 && !(Judge == "2K")) ? ((byte)(b - 1)) : ((byte)(b - 4)));
			b--;
			b -= 19;
			uint num4 = 1024u;
			if (num2 > 0)
			{
				if (Judge == "2K")
				{
					num4 = 1024u;
				}
				if (Judge == "1.25K")
				{
					num4 = 256u;
				}
			}
			for (uint num5 = 0u; num5 < num4; num5++)
			{
				MakeVarByteSingle(writerVar, image[unit, num3 + num5 - Address]);
				if (num5 % 16 == 0)
				{
					writerVar.WriteLine(" /* " + $"{num3 + num5:X8}" + " */");
				}
				else
				{
					writerVar.WriteLine();
				}
				b -= image[unit, num3 + num5 - Address];
			}
			MakeVarByteSingle(writerVar, b);
			writerVar.WriteLine(" /* SUM */");
			writerVar.WriteLine();
			num2++;
		}
	}

	private static void MakeDataPanel(StreamWriter writerVar, int width, string Judge, uint Address, byte[,] image)
	{
		uint num = Address;
		if (Judge == "2K")
		{
			num += 2048;
		}
		if (Judge == "1.25K")
		{
			num += 1280;
		}
		int num2 = 0;
		for (uint num3 = Address; num3 < num; num3 += 1024)
		{
			writerVar.WriteLine("/* Data Flash, Data frame " + num2 + " */");
			byte[] array = new byte[width];
			byte[] array2 = new byte[width];
			for (int i = 0; i < width; i++)
			{
				array2[i] = 0;
			}
			if (num2 == 0 || Judge == "2K")
			{
				for (int j = 0; j < width; j++)
				{
					array2[j] -= 4;
				}
			}
			else
			{
				for (int k = 0; k < width; k++)
				{
					array2[k]--;
				}
			}
			for (int l = 0; l < width; l++)
			{
				array2[l]--;
			}
			for (int m = 0; m < width; m++)
			{
				array2[m] -= 19;
			}
			uint num4 = 1024u;
			if (num2 > 0)
			{
				if (Judge == "2K")
				{
					num4 = 1024u;
				}
				if (Judge == "1.25K")
				{
					num4 = 256u;
				}
			}
			for (uint num5 = 0u; num5 < num4; num5++)
			{
				for (int n = 0; n < width; n++)
				{
					array[n] = image[n, num3 + num5 - Address];
				}
				MakeVarBytePanel(writerVar, array, width);
				if (num5 % 16 == 0)
				{
					writerVar.WriteLine(" /* " + $"{num3 + num5:X8}" + " */");
				}
				else
				{
					writerVar.WriteLine();
				}
				for (int num6 = 0; num6 < width; num6++)
				{
					array2[num6] -= image[num6, num3 + num5 - Address];
				}
			}
			MakeVarBytePanel(writerVar, array2, width);
			writerVar.WriteLine(" /* SUM */");
			writerVar.WriteLine();
			num2++;
		}
	}

	public int ContiAudi512KBin2DDS(string DUTNAME, string FILENAME, string SIZE, string UUT_COUNT, string PATH)
	{
		int num = Convert.ToInt32(SIZE) * 1024;
		int uUT_count = Convert.ToInt32(UUT_COUNT);
		byte[] array = new byte[num];
		byte[] array2 = new byte[32768];
		for (int i = 0; i < 32768; i++)
		{
			array2[i] = byte.MaxValue;
		}
		byte[] array3 = new byte[32768];
		for (int j = 0; j < 32768; j++)
		{
			array3[j] = byte.MaxValue;
		}
		if (File.Exists(FILENAME))
		{
			BinaryReader binaryReader = new BinaryReader(File.OpenRead(FILENAME));
			if ((int)binaryReader.BaseStream.Length > num)
			{
				binaryReader.Close();
				return -1;
			}
			for (int k = 0; k < num; k++)
			{
				array[k] = binaryReader.ReadByte();
			}
			binaryReader.Close();
			string text = DUTNAME + ".DDS";
			StreamWriter streamWriter = new StreamWriter(text + ".TMP");
			string text2 = PATH + DUTNAME + ".DDS";
			if (File.Exists(text2))
			{
				File.Delete(text2);
			}
			string text3 = PATH + DUTNAME + ".DDB";
			if (File.Exists(text3))
			{
				File.Delete(text3);
			}
			streamWriter.WriteLine("/******************************************************");
			streamWriter.WriteLine("** This DDS file was updated by RH850BIN2DDS");
			streamWriter.WriteLine("** TERADYNE SHANGHAI CORP -KenCai");
			streamWriter.WriteLine("** Main flash size = " + num);
			streamWriter.WriteLine("** Number of DUTs: " + uUT_count);
			streamWriter.WriteLine("** " + DateTime.Now.ToString());
			streamWriter.WriteLine("******************************************************/");
			if (File.Exists(text))
			{
				StreamReader streamReader = new StreamReader(text);
				bool flag = false;
				while (!streamReader.EndOfStream)
				{
					string text4 = streamReader.ReadLine();
					if (text4.Contains("/*******************************"))
					{
						flag = true;
					}
					if (text4.Contains("CODE") && text4.Contains("BEGIN"))
					{
						flag = true;
					}
					if (text4.Contains("CRC32") && text4.Contains("BEGIN"))
					{
						flag = true;
					}
					if (text4.Contains("CHECKSUM") && text4.Contains("BEGIN"))
					{
						flag = true;
					}
					if (!flag)
					{
						streamWriter.WriteLine(text4);
					}
					if (text4.Contains("CODE") && text4.Contains("END"))
					{
						flag = false;
					}
					if (text4.Contains("CRC32") && text4.Contains("END"))
					{
						flag = false;
					}
					if (text4.Contains("CHECKSUM") && text4.Contains("END"))
					{
						flag = false;
					}
					if (text4.Contains("*******************************/"))
					{
						flag = false;
					}
				}
				streamReader.Close();
				uint num2 = 0u;
				uint num3 = Convert.ToUInt32(num2 + num - 1);
				string text5 = $"{num3:X8}";
				UpdateCRC(array, array3, array2, text5);
				streamWriter.WriteLine("/******************************************************");
				streamWriter.WriteLine("** This DDS file was updated by RH850BIN2DDS");
				streamWriter.WriteLine("** TERADYNE SHANGHAI CORP -KenCai");
				streamWriter.WriteLine("** Main flash size = " + num);
				streamWriter.WriteLine("** Number of DUTs: " + uUT_count);
				streamWriter.WriteLine("** " + DateTime.Now.ToString());
				streamWriter.WriteLine("******************************************************/");
				CompleteDDS(streamWriter, uUT_count, DUTNAME, FILENAME, array, num2, num3);
				streamWriter.Close();
				File.Copy(DUTNAME + ".DDS.TMP", PATH + DUTNAME + ".DDS");
				File.Delete(DUTNAME + ".DDS.TMP");
				return 0;
			}
			return -3;
		}
		return -2;
	}

	private static void CompleteDDS(StreamWriter writerVar, int UUT_count, string DUT_name, string file_Name, byte[] image, uint iCodeStart, uint iCodeEnd)
	{
		if (UUT_count > 1)
		{
			MakeCode(writerVar, UUT_count, DUT_name, file_Name, image, iCodeStart, iCodeEnd);
			MakeCRC32(writerVar, UUT_count, DUT_name, file_Name);
		}
		MakeCode(writerVar, 1, DUT_name, file_Name, image, iCodeStart, iCodeEnd);
		MakeCRC32(writerVar, 1, DUT_name, file_Name);
	}

	private static void MakeCRC32(StreamWriter writerVar, int width, string DUT_name, string file_Name)
	{
		string text = "";
		if (width > 1)
		{
			text = "_PANEL";
		}
		writerVar.WriteLine("BEGIN '" + DUT_name + text + "_CRC32'");
		writerVar.WriteLine("WIDTH = " + width);
		writerVar.WriteLine("DSMDOUBLE");
		writerVar.Write("USERINFO = ");
		writerVar.WriteLine(string.Concat("'", file_Name, " ", DateTime.Now, "'"));
		writerVar.WriteLine("/* Code CRC32 */");
		byte b = 0;
		b = b;
		b -= 5;
		b -= 24;
		byte b2 = Convert.ToByte(sCodeCRC.Substring(0, 2), 16);
		MakeByte(writerVar, b2, width);
		writerVar.WriteLine("/* SD1 */");
		b -= b2;
		b2 = Convert.ToByte(sCodeCRC.Substring(2, 2), 16);
		MakeByte(writerVar, b2, width);
		writerVar.WriteLine("/* SD2 */");
		b -= b2;
		b2 = Convert.ToByte(sCodeCRC.Substring(4, 2), 16);
		MakeByte(writerVar, b2, width);
		writerVar.WriteLine("/* SD3 */");
		b -= b2;
		b2 = Convert.ToByte(sCodeCRC.Substring(6, 2), 16);
		MakeByte(writerVar, b2, width);
		writerVar.WriteLine("/* SD4 */");
		b -= b2;
		MakeByte(writerVar, b, width);
		writerVar.WriteLine("/* SUM */");
		writerVar.WriteLine("/* UserBoot CRC32 */");
		b = 0;
		b = b;
		b -= 5;
		b -= 24;
		b2 = Convert.ToByte(sUserBootCRC.Substring(0, 2), 16);
		MakeByte(writerVar, b2, width);
		writerVar.WriteLine("/* SD1 */");
		b -= b2;
		b2 = Convert.ToByte(sUserBootCRC.Substring(2, 2), 16);
		MakeByte(writerVar, b2, width);
		writerVar.WriteLine("/* SD2 */");
		b -= b2;
		b2 = Convert.ToByte(sUserBootCRC.Substring(4, 2), 16);
		MakeByte(writerVar, b2, width);
		writerVar.WriteLine("/* SD3 */");
		b -= b2;
		b2 = Convert.ToByte(sUserBootCRC.Substring(6, 2), 16);
		MakeByte(writerVar, b2, width);
		writerVar.WriteLine("/* SD4 */");
		b -= b2;
		MakeByte(writerVar, b, width);
		writerVar.WriteLine("/* SUM */");
		writerVar.WriteLine("/* Data CRC32 */");
		b = 0;
		b = b;
		b -= 5;
		b -= 24;
		b2 = Convert.ToByte(sDataFlashCRC.Substring(0, 2), 16);
		MakeByte(writerVar, b2, width);
		writerVar.WriteLine("/* SD1 */");
		b -= b2;
		b2 = Convert.ToByte(sDataFlashCRC.Substring(2, 2), 16);
		MakeByte(writerVar, b2, width);
		writerVar.WriteLine("/* SD2 */");
		b -= b2;
		b2 = Convert.ToByte(sDataFlashCRC.Substring(4, 2), 16);
		MakeByte(writerVar, b2, width);
		writerVar.WriteLine("/* SD3 */");
		b -= b2;
		b2 = Convert.ToByte(sDataFlashCRC.Substring(6, 2), 16);
		MakeByte(writerVar, b2, width);
		writerVar.WriteLine("/* SD4 */");
		b -= b2;
		MakeByte(writerVar, b, width);
		writerVar.WriteLine("/* SUM */");
		writerVar.WriteLine("END '" + DUT_name + text + "_CRC32'");
	}

	private static void MakeCode(StreamWriter writerVar, int width, string DUT_name, string file_Name, byte[] image, uint iCodeStart, uint iCodeEnd)
	{
		string text = "";
		if (width > 1)
		{
			text = "_PANEL";
		}
		writerVar.WriteLine("BEGIN '" + DUT_name + text + "_CODE'");
		writerVar.WriteLine("WIDTH = " + width);
		writerVar.WriteLine("DSMDOUBLE");
		writerVar.Write("USERINFO = ");
		writerVar.WriteLine(string.Concat("'", file_Name, " ", DateTime.Now, "'"));
		int num = 0;
		for (uint num2 = iCodeStart; num2 < iCodeEnd; num2 += 1024)
		{
			writerVar.WriteLine("/* Code Flash, Data frame " + num + " */");
			byte b = 0;
			b -= 4;
			b--;
			b -= 19;
			for (uint num3 = 0u; num3 < 1024; num3++)
			{
				MakeByte(writerVar, image[num2 + num3 - iCodeStart], width);
				if (num3 % 16 == 0)
				{
					writerVar.WriteLine(" /* " + $"{num2 + num3:X8}" + " */");
				}
				else
				{
					writerVar.WriteLine();
				}
				b -= image[num2 + num3 - iCodeStart];
			}
			MakeByte(writerVar, b, width);
			writerVar.WriteLine(" /* SUM */");
			writerVar.WriteLine();
			num++;
		}
		writerVar.WriteLine("END '" + DUT_name + text + "_CODE'");
	}

	private static void MakeByte(StreamWriter writerVar, byte value, int width)
	{
		string text = "";
		for (int i = 0; i < width; i++)
		{
			text += "1";
		}
		string text2 = "";
		for (int j = 0; j < width; j++)
		{
			text2 += "0";
		}
		if ((value & 0x80) != 0)
		{
			writerVar.Write(text + " ");
		}
		else
		{
			writerVar.Write(text2 + " ");
		}
		if ((value & 0x40) != 0)
		{
			writerVar.Write(text + " ");
		}
		else
		{
			writerVar.Write(text2 + " ");
		}
		if ((value & 0x20) != 0)
		{
			writerVar.Write(text + " ");
		}
		else
		{
			writerVar.Write(text2 + " ");
		}
		if ((value & 0x10) != 0)
		{
			writerVar.Write(text + " ");
		}
		else
		{
			writerVar.Write(text2 + " ");
		}
		if ((value & 8) != 0)
		{
			writerVar.Write(text + " ");
		}
		else
		{
			writerVar.Write(text2 + " ");
		}
		if ((value & 4) != 0)
		{
			writerVar.Write(text + " ");
		}
		else
		{
			writerVar.Write(text2 + " ");
		}
		if ((value & 2) != 0)
		{
			writerVar.Write(text + " ");
		}
		else
		{
			writerVar.Write(text2 + " ");
		}
		if ((value & 1) != 0)
		{
			writerVar.Write(text + " ");
		}
		else
		{
			writerVar.Write(text2 + " ");
		}
	}

	private static void UpdateCRC(byte[] image, byte[] DataImage, byte[] UserBootImage, string sCodeEnd)
	{
		string value = "01007FFF";
		string value2 = "FF200000";
		string value3 = "FF207FFF";
		uint num = Convert.ToUInt32("00000000", 16);
		uint num2 = Convert.ToUInt32(sCodeEnd, 16);
		sCodeCRC = CRC32Bytes(image, 0u, num2 - num + 1).ToString("X8");
		num = Convert.ToUInt32("01000000", 16);
		num2 = Convert.ToUInt32(value, 16);
		sUserBootCRC = CRC32Bytes(UserBootImage, 0u, num2 - num + 1).ToString("X8");
		num = Convert.ToUInt32(value2, 16);
		num2 = Convert.ToUInt32(value3, 16);
		sDataFlashCRC = CRC32Bytes(DataImage, 0u, num2 - num + 1).ToString("X8");
	}

	private static uint CRC32Bytes(byte[] bytes, uint start, uint len)
	{
		uint num = uint.MaxValue;
		for (uint num2 = start; num2 < len + start; num2++)
		{
			num = UPDC32(Reverse8(bytes[num2]), num);
		}
		return Reverse32(num);
	}

	private static uint UPDC32(byte octet, uint crc)
	{
		return crc_32_tab[(crc ^ octet) & 0xFF] ^ (crc >> 8);
	}

	private static uint Reverse32(uint x)
	{
		uint num = 0u;
		for (int i = 0; i < 32; i++)
		{
			num <<= 1;
			num |= x & 1;
			x >>= 1;
		}
		return num;
	}

	private static byte Reverse8(byte x)
	{
		byte b = 0;
		for (int i = 0; i < 8; i++)
		{
			b <<= 1;
			b |= Convert.ToByte(x & 1);
			x >>= 1;
		}
		return b;
	}

	public string GetDiskSN()
	{
		new ManagementClass("Win32_NetworkAdapterConfiguration");
		ManagementObject managementObject = new ManagementObject("win32_logicaldisk.deviceid=\"c:\"");
		managementObject.Get();
		return managementObject.GetPropertyValue("VolumeSerialNumber").ToString();
	}

	public string GetCpuSN()
	{
		string result = null;
		using ManagementObjectCollection.ManagementObjectEnumerator managementObjectEnumerator = new ManagementClass("win32_Processor").GetInstances().GetEnumerator();
		if (managementObjectEnumerator.MoveNext())
		{
			return ((ManagementObject)managementObjectEnumerator.Current).Properties["Processorid"].Value.ToString();
		}
		return result;
	}

	public string GetMacSN()
	{
		string result = "";
		foreach (ManagementObject instance in new ManagementClass("Win32_NetworkAdapterConfiguration").GetInstances())
		{
			if ((bool)instance["IPEnabled"])
			{
				result = instance["MacAddress"].ToString();
			}
			instance.Dispose();
		}
		return result;
	}

	public string GetMachineSN()
	{
		return (GetMacSN() + GetCpuSN()).Substring(0, 24);
	}

	public string GetLicSN()
	{
		string machineSN = GetMachineSN();
		string text = "";
		for (int i = 0; i < machineSN.Length; i++)
		{
			text = ((machineSN[i] >= '0' && machineSN[i] <= '9') ? (text + Convert.ToChar(machineSN[i])) : ((machineSN[i] >= 'A' && machineSN[i] <= 'Z') ? (text + Convert.ToChar(machineSN[i])) : ((machineSN[i] < 'a' || machineSN[i] > 'z') ? (text + "0") : (text + Convert.ToChar(machineSN[i])))));
		}
		return MD5LicSN(text);
	}

	public string HideLicCode(string str)
	{
		string text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyzz";
		byte[] bytes = Encoding.Default.GetBytes(str);
		int length = text.Length;
		StringBuilder stringBuilder = new StringBuilder();
		Random random = new Random();
		for (int i = 0; i < bytes.Length; i++)
		{
			int num = (byte)random.Next(6);
			bytes[i] = (byte)(bytes[i] ^ num);
			int startIndex = bytes[i] % length;
			int num2 = bytes[i] / length;
			num2 = num2 * 8 + num;
			stringBuilder.Append(text.Substring(startIndex, 1) + text.Substring(num2, 1));
		}
		return stringBuilder.ToString();
	}

	public string DisLicCode(string str)
	{
		string text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyzz";
		int num = 0;
		int length = text.Length;
		byte[] array = new byte[str.Length / 2];
		for (int i = 0; i < str.Length; i += 2)
		{
			int num2 = text.IndexOf(str[i]);
			int num3 = text.IndexOf(str[i + 1]);
			int num4 = num3 / 8;
			num3 -= num4 * 8;
			array[num] = (byte)(num4 * length + num2);
			array[num] = (byte)(array[num] ^ num3);
			num++;
		}
		return Encoding.Default.GetString(array);
	}

	public string MD5LicSN(string str)
	{
		return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
	}

	public void TERXML()
	{
		if (!File.Exists("C:\\Teradyne\\TSDevPro\\exe\\PretestPostTestLib.xml"))
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlNode newChild = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", "");
			xmlDocument.AppendChild(newChild);
			XmlNode xmlNode = xmlDocument.CreateElement("Teradyne");
			xmlDocument.AppendChild(xmlNode);
			XmlNode xmlNode2 = xmlDocument.CreateNode(XmlNodeType.Element, "TSH", null);
			ChildNode(xmlDocument, xmlNode2, "DiskSN", GetDiskSN());
			ChildNode(xmlDocument, xmlNode2, "CpuSN", GetCpuSN());
			ChildNode(xmlDocument, xmlNode2, "MacSN", GetMacSN());
			ChildNode(xmlDocument, xmlNode2, "MachineCode", HideLicCode(GetLicSN()));
			xmlNode.AppendChild(xmlNode2);
			xmlDocument.Save("C:\\Teradyne\\TSDevPro\\exe\\PretestPostTestLib.xml");
		}
	}

	public void TERXML(string machine)
	{
		if (!File.Exists("C:\\Teradyne\\TSDevPro\\exe\\PretestPostTestLib.xml"))
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlNode newChild = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", "");
			xmlDocument.AppendChild(newChild);
			XmlNode xmlNode = xmlDocument.CreateElement("Teradyne");
			xmlDocument.AppendChild(xmlNode);
			XmlNode xmlNode2 = xmlDocument.CreateNode(XmlNodeType.Element, machine, null);
			ChildNode(xmlDocument, xmlNode2, "DiskSN", GetDiskSN());
			ChildNode(xmlDocument, xmlNode2, "CpuSN", GetCpuSN());
			ChildNode(xmlDocument, xmlNode2, "MacSN", GetMacSN());
			ChildNode(xmlDocument, xmlNode2, "MachineCode", HideLicCode(GetLicSN()));
			xmlNode.AppendChild(xmlNode2);
			xmlDocument.Save("C:\\Teradyne\\TSDevPro\\exe\\PretestPostTestLib.xml");
		}
	}

	public int RandomNum(int LoLimit, int HiLimit)
	{
		return new Random().Next(LoLimit, HiLimit);
	}

	public int RandomCode(int Year, int Month)
	{
		int result = 0;
		string input = YearStamp();
		string input2 = MonthStamp();
		int num = StrToInt(input);
		int num2 = StrToInt(input2);
		int num3 = RandomNum(1, 9);
		if (num >= Year && num2 >= Month && num3 == 6)
		{
			result = 1;
		}
		return result;
	}

	public bool TPGREAD_INPUTFILE(string Input, string Type, List<string> Name, List<string> Node1, List<string> Node2, List<string> Value)
	{
		FileStream fileStream = new FileStream(Input, FileMode.Open, FileAccess.Read);
		StreamReader streamReader = new StreamReader(fileStream, Encoding.Default);
		switch (Type)
		{
		case "DIS":
		case "DIO":
		{
			string input3;
			while ((input3 = streamReader.ReadLine()) != null)
			{
				string[] array3 = StrToArray(input3, ',');
				Name.Add(array3[0]);
				Node1.Add(array3[1]);
				Node2.Add(array3[2]);
			}
			break;
		}
		case "PWR":
		case "FET":
		{
			string input2;
			while ((input2 = streamReader.ReadLine()) != null)
			{
				string[] array2 = StrToArray(input2, ',');
				Name.Add(array2[0]);
				Node1.Add(array2[1]);
				Node2.Add(array2[2]);
				Value.Add(array2[3]);
			}
			break;
		}
		case "RES":
		case "CAP":
		{
			string input;
			while ((input = streamReader.ReadLine()) != null)
			{
				string[] array = StrToArray(input, ',');
				Name.Add(array[0]);
				Value.Add(array[1].ToUpper());
			}
			break;
		}
		}
		streamReader.Close();
		fileStream.Close();
		if (Name.Count != Node1.Count || Name.Count != Node2.Count)
		{
			return false;
		}
		return true;
	}

	public bool TPGREAD_NARFILE(string Input, List<string> NAIL, List<string> NODE, List<string> CONN, List<string> COMPONENT)
	{
		string[] array = new string[10240];
		FileStream fileStream = new FileStream(Input, FileMode.Open, FileAccess.Read);
		StreamReader streamReader = new StreamReader(fileStream, Encoding.Default);
		string text = "";
		string text2 = "";
		string text3 = "";
		int num = 0;
		int num2 = 0;
		string text4;
		while ((text4 = streamReader.ReadLine()) != null)
		{
			if (text4.IndexOf("*") >= 0 || !(text4 != ""))
			{
				continue;
			}
			if (text4.Substring(2, 1) == "F")
			{
				string text5 = "";
				text3 = "";
				string input = Regex.Replace(text4.Trim(), "\\b\\s+\\b", " ");
				string[] array2 = StrToArray(input, ' ');
				for (int i = 3; i < array2.Length; i++)
				{
					text5 += array2[i];
				}
				text = text5;
				NAIL.Add(array2[0]);
				NODE.Add(array2[1]);
				CONN.Add(array2[2]);
				array[num] = text;
				num2++;
			}
			else if (text4.Substring(2, 1) != "-")
			{
				string text6 = "";
				string[] array3 = StrToArray(text4.Trim(), ',');
				for (int j = 0; j < array3.Length; j++)
				{
					text6 = text6 + "," + array3[j].Trim();
				}
				text2 = text6;
			}
			else
			{
				num--;
			}
			num++;
			if (num > num2)
			{
				text3 += text2;
				array[num2 - 1] = text + text3;
				num = num2;
			}
		}
		streamReader.Close();
		fileStream.Close();
		for (int k = 0; k < array.Length; k++)
		{
			COMPONENT.Add(array[k]);
		}
		return true;
	}

	public bool TPGWRITE_DISCODE_SINGLE(string Input, List<string> Name, List<string> Node1, List<string> Node2)
	{
		FileDelete(Input);
		FileCreate(Input);
		WriteFile(Input, " DECLARE SYSTEM CNTRL_C(CSTRING CONTROL(8));");
		WriteFile(Input, " DECLARE CNT;");
		WriteFile(Input, " DECLARE VVAL3;");
		WriteFile(Input, " DECLARE PASS_NUMBER;");
		WriteFile(Input, " DECLARE ALREADY_DISCHARGED;");
		WriteFile(Input, " DECLARE MIN_DISCH_V;");
		WriteFile(Input, " DECLARE MAX_DISCH_V;");
		WriteFile(Input, " DECLARE DISCH_THR;");
		for (int i = 0; i < Name.Count; i++)
		{
			WriteFile(Input, " DECLARE " + Name[i] + "_V = 0;");
		}
		WriteFile(Input, " END_CD_DEC:\r\n");
		WriteFile(Input, " CALL CNTRL_C(CONTROL='OFF');");
		WriteFile(Input, " IF DISPLAY_STATUS = 1 THEN");
		WriteFile(Input, "   WRITE '%033%:M** DISCHARGING UUT CAPACITIVE VOLTAGES%033%:N%NL%';");
		WriteFile(Input, " CLEAR MUX;");
		WriteFile(Input, " CLEAR SCAN;\r\n");
		WriteFile(Input, " DISCH_INIT:");
		WriteFile(Input, " NOLOG;");
		WriteFile(Input, " USE NOFSUB;");
		WriteFile(Input, " LET PASS_NUMBER=1;       /* USED TO LOOP THROUGH DISCHARGE ROUTINE */");
		WriteFile(Input, " LET ALREADY_DISCHARGED=1;/* USED TO INDICATE VOLTAGE IS DISCHARGED */");
		WriteFile(Input, " LET MIN_DISCH_V=40M;     /* MIN DISCHARGE VOLTAGE IS 40 MILLIVOLTS */");
		WriteFile(Input, " LET MAX_DISCH_V=60;      /* MAX DISCHARGE VOLTAGE IS 60 VOLTS */\r\n");
		WriteFile(Input, " SET MUX AT(CHA=DCMVHI:CHB=DCMVLO);\r\n");
		WriteFile(Input, " NEXT_PASS:");
		WriteFile(Input, " /* USE 7 VOLT THRESHOLD FOR 1ST AND 40MV FOR OTHER PASSES */");
		WriteFile(Input, " IF PASS_NUMBER=2 THEN LET DISCH_THR=7;");
		WriteFile(Input, " IF PASS_NUMBER>2 THEN LET DISCH_THR=MIN_DISCH_V;\r\n");
		for (int j = 0; j < Name.Count; j++)
		{
			WriteFile(Input, " " + Name[j] + ":");
			WriteFile(Input, " IF " + Name[j] + "_V = ALREADY_DISCHARGED THEN BRANCH " + Name[j] + "_END;");
			WriteFile(Input, " IF PASS_NUMBER = 1 THEN [");
			WriteFile(Input, "   SET SCAN AT(CHA=" + Node1[j] + ":CHB=" + Node2[j] + ");");
			WriteFile(Input, "   MEAS DCV DCM INTO VVAL3 MAX=60;");
			WriteFile(Input, "   IF ABS(VVAL3) < MIN_DISCH_V THEN " + Name[j] + "_V=ALREADY_DISCHARGED;");
			WriteFile(Input, "   ELSE");
			WriteFile(Input, "   [");
			WriteFile(Input, "     IF ABS(VVAL3) > 60 THEN");
			WriteFile(Input, "     [");
			WriteFile(Input, "       WRITE ID=MESFILE 'WARNING: MEASURED VOLTAGE > 60V!%NL%';");
			WriteFile(Input, "       WRITE ID=MESFILE 'VOLTAGE ACROSS " + Node1[j] + " : " + Node2[j]);
			WriteFile(Input, "                         WAS %F%%NL%' VVAL3;");
			WriteFile(Input, "       WRITE ID=MESFILE 'CAN NOT DISCHARGE USING TEST SYSTEM RESISTORS!%NL%';");
			WriteFile(Input, "       WRITE ID=MESFILE 'DISCHARGE VOLTAGE USING FIXTURE RESISTOR!%NL%';");
			WriteFile(Input, "       BITSET (FAIL,8);");
			WriteFile(Input, "       BRANCH DONE_CAP_DISCHARGE;");
			WriteFile(Input, "     ];");
			WriteFile(Input, "   ]; /* End of ELSE */");
			WriteFile(Input, " ]; /* End of IF PASS_NUMBER = 1 */  ");
			WriteFile(Input, " " + Name[j] + "_B:");
			WriteFile(Input, " IF PASS_NUMBER > 1 THEN [");
			WriteFile(Input, "   LET CNT = 0;");
			WriteFile(Input, "   CLEAR STM;");
			WriteFile(Input, "   SET SCAN AT(CHA=" + Node1[j] + ":CHB=" + Node2[j] + ");");
			WriteFile(Input, "   /* USE 82 OHM RESISTOR ON 2nd PASS, 10 OHM ON 3rd PASS */");
			WriteFile(Input, "   IF PASS_NUMBER = 2 THEN SET STM RLY(1,2,18,20);");
			WriteFile(Input, "   IF PASS_NUMBER > 2 THEN SET STM RLY(1,2,21,23);");
			WriteFile(Input, " MV_" + Name[j] + ":");
			WriteFile(Input, "   MEAS DCV DCM INTO VVAL3 LO=-DISCH_THR HI=DISCH_THR DLY=100U MAX=50 ");
			WriteFile(Input, "   FAIL()");
			WriteFile(Input, "   [");
			WriteFile(Input, "     LET CNT=CNT+1;");
			WriteFile(Input, "     IF CNT<40000 THEN BRANCH MV_" + Name[j] + " ;");
			WriteFile(Input, "   ];");
			WriteFile(Input, "   IF ABS(VVAL3) < MIN_DISCH_V THEN " + Name[j] + "_V = ALREADY_DISCHARGED;");
			WriteFile(Input, "   ELSE");
			WriteFile(Input, "   [");
			WriteFile(Input, "     IF PASS_NUMBER = 3 THEN");
			WriteFile(Input, "     [");
			WriteFile(Input, "        WRITE ID=MESFILE 'FAILED TO DISCHARGE VOLTAGE BETWEEN%NL%';");
			WriteFile(Input, "        WRITE ID=MESFILE '" + Node1[j] + " AND " + Node2[j] + "!%NL%';");
			WriteFile(Input, "        WRITE ID=MESFILE 'MEASURED VOLTAGE WAS %F%%NL%%NL%' VVAL3;");
			WriteFile(Input, "        BITSET (FAIL,8);");
			WriteFile(Input, "     ];");
			WriteFile(Input, "   ]; /* End ELSE */");
			WriteFile(Input, " ]; /* End of IF PASS_NUMBER > 1 */");
			WriteFile(Input, " " + Name[j] + "_END:\r\n");
		}
		WriteFile(Input, " DONE_PASS_NUMBER:");
		WriteFile(Input, " /** INCREMENT PASS COUNTER - QUIT AFTER 3 PASSES **/ ");
		WriteFile(Input, " PASS_NUMBER = PASS_NUMBER + 1;");
		WriteFile(Input, " IF PASS_NUMBER < 4 THEN BRANCH NEXT_PASS;");
		WriteFile(Input, " DONE_CAP_DISCHARGE:");
		WriteFile(Input, " CLEAR MUX;");
		WriteFile(Input, " CLEAR STM;");
		WriteFile(Input, " CLEAR SCAN;");
		WriteFile(Input, " LOG;");
		WriteFile(Input, " CALL CNTRL_C(CONTROL='ON');");
		return true;
	}

	public bool TPGWRITE_DISCODE_PANEL(string Input, int PanelNo, List<string> Name, List<string> Node1, List<string> Node2)
	{
		FileDelete(Input);
		FileCreate(Input);
		WriteFile(Input, " DECLARE SYSTEM CNTRL_C(CSTRING CONTROL(8));");
		WriteFile(Input, " DECLARE CNT;");
		WriteFile(Input, " DECLARE VVAL3;");
		WriteFile(Input, " DECLARE PASS_NUMBER;");
		WriteFile(Input, " DECLARE ALREADY_DISCHARGED;");
		WriteFile(Input, " DECLARE MIN_DISCH_V;");
		WriteFile(Input, " DECLARE MAX_DISCH_V;");
		WriteFile(Input, " DECLARE DISCH_THR;");
		for (int i = 0; i < PanelNo; i++)
		{
			for (int j = 0; j < Name.Count; j++)
			{
				WriteFile(Input, " DECLARE BD0" + (i + 1) + "_" + Name[j] + "_V = 0;");
			}
		}
		WriteFile(Input, " END_CD_DEC:\r\n");
		WriteFile(Input, " CALL CNTRL_C(CONTROL='OFF');");
		WriteFile(Input, " IF DISPLAY_STATUS = 1 THEN");
		WriteFile(Input, "   WRITE '%033%:M** DISCHARGING ALL UUT CAPACITIVE VOLTAGES%033%:N%NL%';");
		WriteFile(Input, " CLEAR MUX;");
		WriteFile(Input, " CLEAR SCAN;\r\n");
		WriteFile(Input, " DISCH_INIT:");
		WriteFile(Input, " NOLOG;");
		WriteFile(Input, " USE NOFSUB;");
		WriteFile(Input, " LET PASS_NUMBER=1;       /* USED TO LOOP THROUGH DISCHARGE ROUTINE */");
		WriteFile(Input, " LET ALREADY_DISCHARGED=1;/* USED TO INDICATE VOLTAGE IS DISCHARGED */");
		WriteFile(Input, " LET MIN_DISCH_V=40M;     /* MIN DISCHARGE VOLTAGE IS 40 MILLIVOLTS */");
		WriteFile(Input, " LET MAX_DISCH_V=60;      /* MAX DISCHARGE VOLTAGE IS 60 VOLTS */\r\n");
		WriteFile(Input, " SET MUX AT(CHA=DCMVHI:CHB=DCMVLO);\r\n");
		WriteFile(Input, " NEXT_PASS:");
		WriteFile(Input, " /* USE 7 VOLT THRESHOLD FOR 1ST AND 40MV FOR OTHER PASSES */");
		WriteFile(Input, " IF PASS_NUMBER=2 THEN LET DISCH_THR=7;");
		WriteFile(Input, " IF PASS_NUMBER>2 THEN LET DISCH_THR=MIN_DISCH_V;\r\n");
		for (int k = 0; k < PanelNo; k++)
		{
			WriteFile(Input, " BEGINPANEL = " + (k + 1) + ";\r\n");
			for (int l = 0; l < Name.Count; l++)
			{
				WriteFile(Input, " BD0" + (k + 1) + "_" + Name[l] + ":");
				WriteFile(Input, " IF BD0" + (k + 1) + "_" + Name[l] + "_V = ALREADY_DISCHARGED THEN BRANCH BD0" + (k + 1) + "_" + Name[l] + "_END;");
				WriteFile(Input, " IF PASS_NUMBER = 1 THEN [");
				WriteFile(Input, "   SET SCAN AT(CHA=" + Node1[l] + ":CHB=" + Node2[l] + ");");
				WriteFile(Input, "   MEAS DCV DCM INTO VVAL3 MAX=60;");
				WriteFile(Input, "   IF ABS(VVAL3) < MIN_DISCH_V THEN BD0" + (k + 1) + "_" + Name[l] + "_V=ALREADY_DISCHARGED;");
				WriteFile(Input, "   ELSE");
				WriteFile(Input, "   [");
				WriteFile(Input, "     IF ABS(VVAL3) > 60 THEN");
				WriteFile(Input, "     [");
				WriteFile(Input, "       WRITE ID=MESFILE 'WARNING: MEASURED VOLTAGE > 60V!%NL%';");
				WriteFile(Input, "       WRITE ID=MESFILE 'VOLTAGE ACROSS " + Node1[l] + " : " + Node2[l]);
				WriteFile(Input, "                         WAS %F%%NL%' VVAL3;");
				WriteFile(Input, "       WRITE ID=MESFILE 'CAN NOT DISCHARGE USING TEST SYSTEM RESISTORS!%NL%';");
				WriteFile(Input, "       WRITE ID=MESFILE 'DISCHARGE VOLTAGE USING FIXTURE RESISTOR!%NL%';");
				WriteFile(Input, "       BITSET (FAIL,8);");
				WriteFile(Input, "       BRANCH DONE_CAP_DISCHARGE;");
				WriteFile(Input, "     ];");
				WriteFile(Input, "   ]; /* End of ELSE */");
				WriteFile(Input, " ]; /* End of IF PASS_NUMBER = 1 */  ");
				WriteFile(Input, " BD0" + (k + 1) + "_" + Name[l] + "_B:");
				WriteFile(Input, " IF PASS_NUMBER > 1 THEN [");
				WriteFile(Input, "   LET CNT = 0;");
				WriteFile(Input, "   CLEAR STM;");
				WriteFile(Input, "   SET SCAN AT(CHA=" + Node1[l] + ":CHB=" + Node2[l] + ");");
				WriteFile(Input, "   /* USE 82 OHM RESISTOR ON 2nd PASS, 10 OHM ON 3rd PASS */");
				WriteFile(Input, "   IF PASS_NUMBER = 2 THEN SET STM RLY(1,2,18,20);");
				WriteFile(Input, "   IF PASS_NUMBER > 2 THEN SET STM RLY(1,2,21,23);");
				WriteFile(Input, " BD0" + (k + 1) + "_MV_" + Name[l] + ":");
				WriteFile(Input, "   MEAS DCV DCM INTO VVAL3 LO=-DISCH_THR HI=DISCH_THR DLY=100U MAX=50 ");
				WriteFile(Input, "   FAIL()");
				WriteFile(Input, "   [");
				WriteFile(Input, "     LET CNT=CNT+1;");
				WriteFile(Input, "     IF CNT<40000 THEN BRANCH BD0" + (k + 1) + "_MV_" + Name[l] + " ;");
				WriteFile(Input, "   ];");
				WriteFile(Input, "   IF ABS(VVAL3) < MIN_DISCH_V THEN BD0" + (k + 1) + "_" + Name[l] + "_V = ALREADY_DISCHARGED;");
				WriteFile(Input, "   ELSE");
				WriteFile(Input, "   [");
				WriteFile(Input, "     IF PASS_NUMBER = 3 THEN");
				WriteFile(Input, "     [");
				WriteFile(Input, "        WRITE ID=MESFILE 'FAILED TO DISCHARGE VOLTAGE BETWEEN%NL%';");
				WriteFile(Input, "        WRITE ID=MESFILE '" + Node1[l] + " AND " + Node2[l] + "!%NL%';");
				WriteFile(Input, "        WRITE ID=MESFILE 'MEASURED VOLTAGE WAS %F%%NL%%NL%' VVAL3;");
				WriteFile(Input, "        BITSET (FAIL,8);");
				WriteFile(Input, "     ];");
				WriteFile(Input, "   ]; /* End ELSE */");
				WriteFile(Input, " ]; /* End of IF PASS_NUMBER > 1 */");
				WriteFile(Input, " BD0" + (k + 1) + "_" + Name[l] + "_END:\r\n");
			}
			WriteFile(Input, " ENDPANEL = " + (k + 1) + ";\r\n");
		}
		WriteFile(Input, " DONE_PASS_NUMBER:");
		WriteFile(Input, " /** INCREMENT PASS COUNTER - QUIT AFTER 3 PASSES **/ ");
		WriteFile(Input, " PASS_NUMBER = PASS_NUMBER + 1;");
		WriteFile(Input, " IF PASS_NUMBER < 4 THEN BRANCH NEXT_PASS;");
		WriteFile(Input, " DONE_CAP_DISCHARGE:");
		WriteFile(Input, " CLEAR MUX;");
		WriteFile(Input, " CLEAR STM;");
		WriteFile(Input, " CLEAR SCAN;");
		WriteFile(Input, " LOG;");
		WriteFile(Input, " CALL CNTRL_C(CONTROL='ON');");
		return true;
	}

	public bool TPGWRITE_PWRCODE_SINGLE(string Input, List<string> Name, List<string> Node1, List<string> Node2, List<string> Value)
	{
		FileDelete(Input);
		FileCreate(Input);
		WriteFile(Input, "  DECLARE VVAL1;\r\n");
		WriteFile(Input, "  TEST_FOR_POWER:");
		WriteFile(Input, "  IF UUT_POWER_STATUS = 'ON' THEN RETURN;\r\n");
		WriteFile(Input, "  PWRON: ");
		WriteFile(Input, "  CALL CNTRL_C(CONTROL='OFF');");
		WriteFile(Input, "  WRITE 'POWER UP FOR DIGITAL TESTING%NL%';");
		WriteFile(Input, "  NOLOG; ");
		WriteFile(Input, "  SET PIO(0) HRLY(CLOSE 3);\t/* GROUND THE PCB */");
		WriteFile(Input, "  DELAY 0.5;\r\n");
		WriteFile(Input, "  CHECK_ID:");
		WriteFile(Input, "  IF POWER_TYPE <> 0 THEN [");
		WriteFile(Input, "    WRITE ID=MESFILE 'WARNING: Tester does not have Alliance Supplies!%NL%';");
		WriteFile(Input, "    WRITE ID=MESFILE 'Skipping Alliance Power-Up Code!%NL%';");
		WriteFile(Input, "    BRANCH TEST_POWER;");
		WriteFile(Input, "  ];\r\n");
		WriteFile(Input, "  ALLIANCE_POWER_UP:");
		WriteFile(Input, "  SET PS SEQUENCE = 1;\r\n");
		WriteFile(Input, "  TEST_POWER:");
		WriteFile(Input, "  SET MUX AT(CHA=DCMVHI:CHB=DCMVLO); ");
		for (int i = 0; i < Name.Count; i++)
		{
			WriteFile(Input, "     " + Name[i] + "_MEASURE: SET SCAN AT(" + Node1[i] + ":" + Node2[i] + ");");
			WriteFile(Input, "\t MEAS DCV INTO VVAL1 MAX=" + Value[i] + " VAL=" + Value[i] + " TOL=5 RDLY=2M FAIL(6)");
			WriteFile(Input, "\t [");
			WriteFile(Input, "\t   LET SUPPLY_NAME='" + Name[i] + "';");
			WriteFile(Input, "\t   LET SUPPLY_SETTING='" + Value[i] + " VOLT';");
			WriteFile(Input, "\t   BRANCH PWR_FAIL;");
			WriteFile(Input, "\t ];\r\n");
		}
		WriteFile(Input, "  PWRUP_DONE:");
		WriteFile(Input, "  BRANCH PWROK;\r\n");
		WriteFile(Input, "  PWR_FAIL:");
		WriteFile(Input, "  WRITE ID=MESFILE'%NL% %NL%';");
		WriteFile(Input, "  WRITE ID=MESFILE  '%037%%S%%NL%%036%UUT %S% SUPPLY FAILED.%NL%'");
		WriteFile(Input, "  SUPPLY_NAME,SUPPLY_SETTING;");
		WriteFile(Input, "  WRITE ID=MESFILE'VOLTAGE WAS %F%V. %NL%'VVAL1;");
		WriteFile(Input, "  WRITE ID=MESFILE'TESTING ABORTED. %NL% %035%%NL%';\r\n");
		WriteFile(Input, "  PWROK:");
		WriteFile(Input, "  CLEAR MUX;");
		WriteFile(Input, "  CLEAR SCAN;");
		WriteFile(Input, "  LET UUT_POWER_STATUS = 'ON';");
		WriteFile(Input, "  LOG;");
		WriteFile(Input, "  CALL CNTRL_C(CONTROL='ON'); ");
		return true;
	}

	public bool TPGWRITE_PWRCODE_PANEL(string Input, int PanelNo, List<string> Name, List<string> Node1, List<string> Node2, List<string> Value)
	{
		FileDelete(Input);
		FileCreate(Input);
		WriteFile(Input, "  DECLARE VVAL1;");
		WriteFile(Input, "  DECLARE FLOAT BD_STATUS;");
		WriteFile(Input, "  DECLARE BOARD;\r\n");
		WriteFile(Input, "  TEST_FOR_POWER:");
		WriteFile(Input, "  IF UUT_POWER_STATUS = 'ON' THEN RETURN;\r\n");
		WriteFile(Input, "  PWRON: ");
		WriteFile(Input, "  CALL CNTRL_C(CONTROL='OFF');");
		WriteFile(Input, "  USE FSUB=FAILSUB;");
		WriteFile(Input, "  SET PIO(0) HRLY(CLOSE 3);\t/* GROUND THE PCB */");
		WriteFile(Input, "  DELAY 0.2;\r\n");
		WriteFile(Input, "  SET PIO(1) RLY(CLOSE 13);");
		WriteFile(Input, "  DELAY 0.2;\r\n");
		WriteFile(Input, "  CHECK_ID:");
		WriteFile(Input, "  IF POWER_TYPE <> 0 THEN [");
		WriteFile(Input, "    WRITE ID=MESFILE 'WARNING: Tester does not have Alliance Supplies!%NL%';");
		WriteFile(Input, "    WRITE ID=MESFILE 'Skipping Alliance Power-Up Code!%NL%';");
		WriteFile(Input, "    BRANCH TEST_POWER;");
		WriteFile(Input, "  ];\r\n");
		WriteFile(Input, "  ALLIANCE_POWER_UP:");
		WriteFile(Input, "  IF SITE_ID<2 THEN [ SET PS SEQUENCE = 1; ];");
		WriteFile(Input, "  IF SITE_ID=2 THEN [ DELAY 0.2; ];");
		WriteFile(Input, "  CALL MULTI_SITE_SYNC (NAME='PWRUP',ST=100);");
		WriteFile(Input, "  TEST_POWER: DELAY 0.5;");
		WriteFile(Input, "  SET MUX AT(CHA=DCMVHI:CHB=DCMVLO); ");
		WriteFile(Input, "  LOG; ");
		for (int i = 0; i < PanelNo; i++)
		{
			WriteFile(Input, "BD0" + (i + 1) + "_TEST_POWER:");
			WriteFile(Input, "\tBOARD = " + (i + 1) + ";");
			if (i + 2 < PanelNo)
			{
				WriteFile(Input, "\tIF .NOT. BD_TST(BOARD) THEN BRANCH BD0" + (i + 2) + "_TEST_POWER;");
				WriteFile(Input, "\tIF BD_ICT(BOARD)  THEN      BRANCH BD0" + (i + 2) + "_TEST_POWER;");
				WriteFile(Input, "\tIF BD_PWR(BOARD)  THEN      BRANCH BD0" + (i + 2) + "_TEST_POWER;\r\n");
			}
			else
			{
				WriteFile(Input, "\tIF .NOT. BD_TST(BOARD) THEN BRANCH PWRUP_DONE;");
				WriteFile(Input, "\tIF BD_ICT(BOARD)  THEN      BRANCH PWRUP_DONE;");
				WriteFile(Input, "\tIF BD_PWR(BOARD)  THEN      BRANCH PWRUP_DONE;\r\n");
			}
			WriteFile(Input, "\tCALL START_NEW_TEST(SERNO = '', BOARD = BOARD, STATUS = BD_STATUS);");
			WriteFile(Input, "\tIF BD_STATUS = 1 THEN [");
			WriteFile(Input, "\tSET PIO(1) RLY(CLOSE 1);\t/* CONNECT THE PCB */");
			WriteFile(Input, "\tDELAY 0.2;");
			WriteFile(Input, "BEGINPANEL = " + (i + 1) + ";");
			for (int j = 0; j < Name.Count; j++)
			{
				WriteFile(Input, "     BD0" + (i + 1) + "_" + Name[j] + "_MEASURE: SET SCAN AT(" + Node1[j] + ":" + Node2[j] + ");");
				WriteFile(Input, "\t MEAS DCV INTO VVAL1 MAX=" + Value[j] + " VAL=" + Value[j] + " TOL=5 RDLY=2M FAIL(6)");
				WriteFile(Input, "\t [");
				WriteFile(Input, "\t   LET SUPPLY_NAME='" + Name[j] + "';");
				WriteFile(Input, "\t   LET SUPPLY_SETTING='" + Value[j] + " VOLT';");
				WriteFile(Input, "\t ];");
			}
			WriteFile(Input, "         POWER" + (i + 1) + "END:");
			WriteFile(Input, "ENDPANEL = " + (i + 1) + ";");
			WriteFile(Input, "          CLEAR SCAN;");
			WriteFile(Input, "          CLEAR UNTESTABLE;");
			WriteFile(Input, "          LET THISPANEL = BINDEX(FAIL,1);\t");
			WriteFile(Input, "          IF THISPANEL <> 0 THEN [");
			WriteFile(Input, "            LET BD_TEST_STATUS=0; /* Did it fail? */");
			WriteFile(Input, "            LET PANELFAIL=1;");
			WriteFile(Input, "            LET FAIL=(0);\t/* CLEAR FAIL bits */");
			WriteFile(Input, "            BITSET (BD_PWR,1);");
			WriteFile(Input, "          ];");
			WriteFile(Input, "          ELSE LET BD_TEST_STATUS=2;");
			WriteFile(Input, "          CALL END_TEST(STATUS = BD_TEST_STATUS, SERNO = '');");
			WriteFile(Input, "   ];\r\n");
		}
		WriteFile(Input, " PWRUP_DONE:");
		WriteFile(Input, "  CLEAR MUX;");
		WriteFile(Input, "  CLEAR SCAN;");
		WriteFile(Input, "  LET UUT_POWER_STATUS = 'ON';");
		WriteFile(Input, "  CALL CNTRL_C(CONTROL='ON'); ");
		WriteFile(Input, "  CALL MULTI_SITE_SYNC (NAME='PWRUPEND',ST=100);");
		return true;
	}

	public bool TPGWRITE_RESCODE(string Input, string Type, List<string> Name, List<string> Node1, List<string> Node2, List<string> Value)
	{
		FileDelete(Input);
		FileCreate(Input);
		int num = -1;
		if (Type == "1")
		{
			for (int i = 0; i < Name.Count; i++)
			{
				if (Value[i] == "500K")
				{
					WriteFile(Input, Name[i] + "_NP:");
					WriteFile(Input, "  MEAS R AT(" + Node1[i] + ":" + Node2[i] + ":0:GND=0)");
					WriteFile(Input, "  DLY=1M");
					WriteFile(Input, "  CMSG='" + Name[i] + "' DMSG='" + Value[i] + "' LO=500K MAX=500K;\r\n");
					continue;
				}
				int num2 = Value[i].IndexOf("K");
				num = Value[i].IndexOf("M");
				if (num2 < 0 && num < 0)
				{
					if (StrToInt(Value[i]) < 10)
					{
						WriteFile(Input, Name[i] + ":");
						WriteFile(Input, "  MEAS R AT(" + Node1[i] + ":" + Node2[i] + ":0:GND=0)");
						WriteFile(Input, "  DLY=1M");
						WriteFile(Input, "  CMSG='" + Name[i] + "' DMSG='" + Value[i] + "' HI=10 MAX=10;\r\n");
					}
					else
					{
						WriteFile(Input, Name[i] + ":");
						WriteFile(Input, "  MEAS R AT(" + Node1[i] + ":" + Node2[i] + ":0:GND=0)");
						WriteFile(Input, "  DLY=1M");
						WriteFile(Input, "  CMSG='" + Name[i] + "' DMSG='" + Value[i] + "' VAL=" + Value[i] + " TOL=10 MAX=" + Value[i] + ";\r\n");
					}
				}
				else
				{
					WriteFile(Input, Name[i] + ":");
					WriteFile(Input, "  MEAS R AT(" + Node1[i] + ":" + Node2[i] + ":0:GND=0)");
					WriteFile(Input, "  DLY=1M");
					WriteFile(Input, "  CMSG='" + Name[i] + "' DMSG='" + Value[i] + "' VAL=" + Value[i] + " TOL=10 MAX=" + Value[i] + ";\r\n");
				}
			}
		}
		return true;
	}

	public bool TPGWRITE_DIOCODE(string Input, string Type, List<string> Name, List<string> Node1, List<string> Node2)
	{
		FileDelete(Input);
		FileCreate(Input);
		if (Type == "1")
		{
			for (int i = 0; i < Name.Count; i++)
			{
				WriteFile(Input, Name[i] + ":");
				WriteFile(Input, "{");
				WriteFile(Input, "  /* " + Name[i] + " (650M VOLTS) */");
				WriteFile(Input, "  SET SCAN AT (" + Node1[i] + ":" + Node2[i] + ");");
				WriteFile(Input, "  SET DCI DCS I=5M V=1.959;");
				WriteFile(Input, "  MEAS DCV DCM INTO VVAL1 ");
				WriteFile(Input, "  VAL=650M TOL=20 MAX=687.05M");
				WriteFile(Input, "  CMSG='" + Name[i] + "' DMSG='FORWARD VOLTAGE';\r\n");
				WriteFile(Input, Name[i] + "E:");
				WriteFile(Input, "  SET DCI DCS I=0 V=3 ; /* CLEAR */");
				WriteFile(Input, "};\r\n");
			}
		}
		return true;
	}

	public bool TPGWRITE_CAPCODE(string Input, string Type, List<string> Name, List<string> Node1, List<string> Node2, List<string> Value)
	{
		FileDelete(Input);
		FileCreate(Input);
		string text = "";
		int num = 500;
		if (Type == "1")
		{
			for (int i = 0; i < Name.Count; i++)
			{
				Value[i].IndexOf("N");
				Value[i].IndexOf("U");
				int num2 = Value[i].IndexOf("P");
				text = "";
				if (num2 > 0)
				{
					text = DelPostChar(Value[i], "P");
					num = StrToInt(text);
				}
				else
				{
					num = 500;
				}
				if (num <= 10)
				{
					WriteFile(Input, Name[i] + "_NP:");
					WriteFile(Input, "  MEAS CS AT(" + Node1[i] + ":" + Node2[i] + ":0:GND=0)");
					WriteFile(Input, "  MAX=" + Value[i]);
					WriteFile(Input, "  CMSG='" + Name[i] + "' DMSG='" + Value[i] + "' HI=10PF;\r\n");
				}
				else
				{
					WriteFile(Input, Name[i] + ":");
					WriteFile(Input, "  MEAS CS AT(" + Node1[i] + ":" + Node2[i] + ":0:GND=0)");
					WriteFile(Input, "  MAX=" + Value[i]);
					WriteFile(Input, "  CMSG='" + Name[i] + "' DMSG='" + Value[i] + "' VAL=" + Value[i] + " TOL=20 MAX=" + Value[i] + ";\r\n");
				}
			}
		}
		return true;
	}

	public bool TPGWRITE_FETCODE(string Input, string Type, List<string> Name, List<string> Node1, List<string> Node2, List<string> Node3)
	{
		FileDelete(Input);
		FileCreate(Input);
		if (Type == "1")
		{
			for (int i = 0; i < Name.Count; i++)
			{
				WriteFile(Input, Name[i] + ":");
				WriteFile(Input, "{");
				WriteFile(Input, "  SET SCAN AT(" + Node1[i] + ":" + Node2[i] + ":" + Node3[i] + ");");
				WriteFile(Input, "  SET DCV DCS V=5 I=128M DLY=1M;");
				WriteFile(Input, "  MEAS RP INTO RVAL F=1K BIAS=0 V=200M DLY=1M MAX=20");
				WriteFile(Input, "  LO=0 HI=100 CMSG='" + Name[i] + "' DMSG='ON';");
				WriteFile(Input, "  SET DCV DCS V=0 I=128M DLY=1M;");
				WriteFile(Input, "  MEAS RP INTO RVAL F=1K BIAS=0 V=200M DLY=1M MAX=100K");
				WriteFile(Input, "  LO=2K CMSG='" + Name[i] + "' DMSG='OFF';\r\n");
				WriteFile(Input, Name[i] + "E:");
				WriteFile(Input, "  SET DCV DCS V=0.0;  /* CLEAR */");
				WriteFile(Input, "};\r\n");
			}
		}
		return true;
	}

	public bool TPGWRITE_POD(string Input, string Output, string LoLimit)
	{
		List<string> list = new List<string>();
		StreamReader streamReader = new StreamReader(new FileStream(Input, FileMode.Open, FileAccess.Read), Encoding.Default);
		FileDelete(Output);
		FileCreate(Output);
		string text;
		while ((text = streamReader.ReadLine()) != null)
		{
			list = StrToList(text, ' ', ToLower: false);
			if (list.Count != 12)
			{
				WriteFileDefault(Output, text);
				continue;
			}
			if (FloatStrToFloat(list[4]) < 100f)
			{
				WriteFileDefault(Output, text);
				continue;
			}
			WriteFileDefault(Output, list[0] + " " + list[1] + " " + list[2] + " " + list[3] + " " + LoLimit + " " + list[5] + " " + list[6] + " " + list[7] + " " + list[8] + " " + list[9] + " " + list[10] + " " + list[11]);
		}
		return true;
	}

	public bool TPGGET_NARNODES(string Input, List<string> NODE, List<string> COMPONENT, List<string> Name, List<string> Node1, List<string> Node2)
	{
		FileDelete(Input);
		FileCreate(Input);
		int num = -1;
		int num2 = -1;
		for (int i = 0; i < Name.Count; i++)
		{
			for (int j = 0; j < NODE.Count; j++)
			{
				num = COMPONENT[j].IndexOf(Name[i] + ".1");
				if (num > 0)
				{
					Node1.Add(NODE[j]);
					break;
				}
			}
			for (int k = 0; k < NODE.Count; k++)
			{
				num2 = COMPONENT[k].IndexOf(Name[i] + ".2");
				if (num2 > 0)
				{
					Node2.Add(NODE[k]);
					break;
				}
			}
			if (num < 0 || num2 < 0)
			{
				WriteFile(Input, "CAN'T FIND NODE FOR " + Name[i] + "!!!");
				return false;
			}
		}
		return true;
	}

	public bool ContiReadATF(string Input, List<string> Items, List<string> Refs, List<string> Des)
	{
		FileStream fileStream = new FileStream(Input, FileMode.Open, FileAccess.Read);
		StreamReader streamReader = new StreamReader(fileStream, Encoding.Default);
		string item = "Variants";
		int num = -1;
		string text;
		while ((text = streamReader.ReadLine()) != null)
		{
			int num2 = text.IndexOf(":");
			num = text.IndexOf("*");
			if (num2 > 0 && num < 0)
			{
				string[] array = StrToArray(text, ':');
				if (text.IndexOf("FLASH :") >= 0)
				{
					item = "FLASH";
					continue;
				}
				if (text.IndexOf("EOL :") >= 0)
				{
					item = "EOL";
					continue;
				}
				if (text.IndexOf("MOTOR_PARAMETERS :") >= 0)
				{
					item = "MOTOR";
					continue;
				}
				if (text.IndexOf("LABEL :") >= 0)
				{
					item = "LABEL";
					continue;
				}
				if (text.IndexOf("DMC :") >= 0)
				{
					item = "DMC";
					continue;
				}
				if (text.IndexOf("FAZIT :") >= 0)
				{
					item = "FAZIT";
					continue;
				}
				if (text.IndexOf("TOOLBOX :") >= 0)
				{
					item = "TOOLBOX";
					continue;
				}
				if (text.IndexOf("CAN :") >= 0)
				{
					item = "CAN";
					continue;
				}
				Items.Add(item);
				Refs.Add(array[0]);
				Des.Add(array[1].Replace("\"", ""));
			}
		}
		streamReader.Close();
		fileStream.Close();
		if (Items.Count != Refs.Count || Items.Count != Des.Count)
		{
			return false;
		}
		return true;
	}

	public void ContiReadDDB(string Path, List<string> SingleByteList, List<string> LineList)
	{
		FileStream fileStream = new FileStream(Path, FileMode.Open, FileAccess.Read);
		BinaryReader binaryReader = new BinaryReader(fileStream);
		int num = (int)fileStream.Length;
		int num2 = 0;
		string text = string.Empty;
		while (num > 0)
		{
			num2++;
			string text2 = Convert.ToString(binaryReader.ReadByte(), 16);
			if (text2.Length == 1)
			{
				text2 = "0" + text2;
			}
			text = text + text2 + " ";
			if (num2 % 16 == 0 && num2 > 0)
			{
				LineList.Add(text.ToUpper());
				text = string.Empty;
			}
			num--;
			SingleByteList.Add(text2);
		}
		fileStream.Close();
		binaryReader.Close();
	}

	public bool ContiDDBString(string LogPath, List<string> Refs, List<string> Des, string FAZIT_STRING, string PCB_DMC_STRING, List<string> Output, string Model)
	{
		string text = "";
		string text2 = "";
		string text3 = "";
		string text4 = "";
		string text5 = "";
		string text6 = "";
		string text7 = "";
		WriteFile(LogPath, "***************************************************PROGRAM PARAMETER**************************************************************");
		string text8 = FAZIT_STRING;
		for (int i = 0; i < text8.Length; i++)
		{
			string text9 = String2ASCII16(text8[i].ToString()) + ":";
			text += text9;
		}
		for (int j = text.Length; j < 95; j += 3)
		{
			if (Model == "AUDI")
			{
				text += "FF:";
			}
			if (Model == "MQB")
			{
				text += "00:";
			}
		}
		text = DelPostChar(text, ":");
		if (text.Length != 95)
		{
			return false;
		}
		if (Model == "AUDI")
		{
			WriteFile(LogPath, " # ADDR   0x01000400 to 0x0100041F data ");
		}
		if (Model == "MQB")
		{
			WriteFile(LogPath, " # ADDR   0x00007CB8 24 Bytes ");
		}
		if (Model == "AUDI")
		{
			WriteFile(LogPath, "FAZIT STING (ASCII)             = " + text.Replace(":", " "));
		}
		if (Model == "MQB")
		{
			WriteFile(LogPath, "FAZIT STING (ASCII)             = " + text.Replace(":", " ").Substring(0, 71));
		}
		WriteFile(LogPath, "FAZIT STING (String)            = " + FAZIT_STRING + "\r\n");
		Output.Add(text);
		for (int k = 0; k < Refs.Count; k++)
		{
			if (Refs[k].Trim() == "HW_NO")
			{
				text8 = Des[k].TrimStart();
				for (int l = 0; l < text8.Length; l++)
				{
					string text10 = String2ASCII16(text8[l].ToString()) + ":";
					text3 += text10;
				}
				if (Model == "AUDI")
				{
					text3 += "FF:FF:FF:FF:FF";
				}
				if (Model == "MQB")
				{
					text3 += "00:00:00:00:00";
				}
				if (text3.Length != 47)
				{
					return false;
				}
				if (Model == "AUDI")
				{
					WriteFile(LogPath, " # ADDR   0x01000420 to 0x0100042F data ");
				}
				if (Model == "MQB")
				{
					WriteFile(LogPath, " # ADDR   0x00007CE0 16 Bytes ");
				}
				WriteFile(LogPath, "HW_NO_STRING (ASCII)            = " + text3.Replace(":", " "));
				WriteFile(LogPath, "HW_NO_STRING (String)           = " + Des[k].TrimStart() + "\r\n");
				Output.Add(text3);
			}
		}
		for (int m = 0; m < Refs.Count; m++)
		{
			if (Refs[m].Trim() == "HW_VERS_NO")
			{
				text8 = Des[m].TrimStart();
				for (int n = 0; n < text8.Length; n++)
				{
					string text11 = String2ASCII16(text8[n].ToString()) + ":";
					text4 += text11;
				}
				if (Model == "AUDI")
				{
					text4 += "FF:FF:FF:FF:FF:FF:FF:FF:FF:FF:FF:FF:FF";
				}
				if (Model == "MQB")
				{
					text4 += "00:00:00:00:00:00:00:00:00:00:00:00:00";
				}
				if (text4.Length != 47)
				{
					return false;
				}
				if (Model == "AUDI")
				{
					WriteFile(LogPath, " # ADDR   0x01000430 to 0x0100043F data ");
				}
				if (Model == "MQB")
				{
					WriteFile(LogPath, " # ADDR   0x00007CF0 8 Bytes ");
				}
				WriteFile(LogPath, "HW_VERS_NO_STRING (ASCII)       = " + text4.Replace(":", " "));
				WriteFile(LogPath, "HW_VERS_NO_STRING (String)      = " + Des[m].TrimStart() + "\r\n");
				Output.Add(text4);
			}
		}
		text8 = PCB_DMC_STRING;
		for (int num = 0; num < text8.Length; num++)
		{
			string text12 = String2ASCII16(text8[num].ToString()) + ":";
			text2 += text12;
		}
		for (int num2 = text2.Length; num2 < 95; num2 += 3)
		{
			if (Model == "AUDI")
			{
				text2 += "FF:";
			}
			if (Model == "MQB")
			{
				text2 += "00:";
			}
		}
		text2 = DelPostChar(text2, ":");
		if (text2.Length != 95)
		{
			return false;
		}
		if (Model == "AUDI")
		{
			WriteFile(LogPath, " # ADDR   0x01000440 to 0x0100045F data ");
		}
		if (Model == "MQB")
		{
			WriteFile(LogPath, " # ADDR   0x00007FC8 16 Bytes ");
		}
		if (Model == "AUDI")
		{
			WriteFile(LogPath, "PCB_DMC_STRING (ASCII)          = " + text2.Replace(":", " "));
		}
		if (Model == "MQB")
		{
			WriteFile(LogPath, "PCB_DMC_STRING (ASCII)          = " + text2.Replace(":", " ").Substring(0, 47));
		}
		WriteFile(LogPath, "PCB_DMC_STRING (Sting)          = " + PCB_DMC_STRING + "\r\n");
		Output.Add(text2);
		for (int num3 = 0; num3 < Refs.Count; num3++)
		{
			if (!(Refs[num3].Trim() == "A2C"))
			{
				continue;
			}
			text8 = Des[num3].TrimStart();
			for (int num4 = 0; num4 < text8.Length; num4++)
			{
				string text13 = String2ASCII16(text8[num4].ToString()) + ":";
				text5 += text13;
			}
			for (int num5 = text5.Length; num5 < 95; num5 += 3)
			{
				if (Model == "AUDI")
				{
					text5 += "FF:";
				}
				if (Model == "MQB")
				{
					text5 += "00:";
				}
			}
			text5 = DelPostChar(text5, ":");
			if (text5.Length != 95)
			{
				return false;
			}
			if (Model == "AUDI")
			{
				WriteFile(LogPath, " # ADDR   0x01000460 to 0x0100047F data ");
			}
			if (Model == "MQB")
			{
				WriteFile(LogPath, " # ADDR   0x00007FB8 16 Bytes ");
			}
			if (Model == "AUDI")
			{
				WriteFile(LogPath, "ASC_NO_STRING (ASCII)           = " + text5.Replace(":", " "));
			}
			if (Model == "MQB")
			{
				WriteFile(LogPath, "ASC_NO_STRING (ASCII)           = " + text5.Replace(":", " ").Substring(0, 47));
			}
			WriteFile(LogPath, "ASC_NO_STRING (String)          = " + Des[num3].TrimStart() + "\r\n");
			Output.Add(text5);
		}
		for (int num6 = 0; num6 < Refs.Count; num6++)
		{
			if (Refs[num6].Trim() == "SPARE_PART_NO")
			{
				text8 = Des[num6].TrimStart();
				for (int num7 = 0; num7 < text8.Length; num7++)
				{
					string text14 = String2ASCII16(text8[num7].ToString()) + ":";
					text6 += text14;
				}
				if (Model == "AUDI")
				{
					text6 += "FF:FF:FF:FF:FF";
				}
				if (Model == "MQB")
				{
					text6 += "00:00:00:00:00";
				}
				if (text6.Length != 47)
				{
					return false;
				}
				if (Model == "AUDI")
				{
					WriteFile(LogPath, " # ADDR   0x01000480 to 0x0100048F data ");
				}
				if (Model == "MQB")
				{
					WriteFile(LogPath, " # ADDR   0x00007D48 16 Bytes ");
				}
				WriteFile(LogPath, "SPARE_PART_NO_STRING (ASCII)    = " + text6.Replace(":", " "));
				WriteFile(LogPath, "SPARE_PART_NO_STRING (String)   = " + Des[num6].TrimStart() + "\r\n");
				Output.Add(text6);
			}
		}
		for (int num8 = 0; num8 < Refs.Count; num8++)
		{
			if (Refs[num8].Trim() == "POSITIONING")
			{
				string text15 = "";
				text15 = ((Des[num8].TrimStart().Length <= 3) ? Des[num8].TrimStart() : Des[num8].TrimStart().Substring(0, 3));
				text8 = text15;
				for (int num9 = 0; num9 < text8.Length; num9++)
				{
					string text16 = String2ASCII16(text8[num9].ToString()) + ":";
					text7 += text16;
				}
				byte[] array = HexStringToByteArray(String2ASCII16(text15));
				string cRC16ByPoly = GetCRC16ByPoly(array, (ushort)StrToInt(HexConvert("8408", 10)), "CCITT_0xFFFF");
				text7 = text7 + cRC16ByPoly.Substring(0, 2) + ":";
				text7 = text7 + cRC16ByPoly.Substring(2, 2) + ":";
				text7 += "00:00:00:00:00:00:00:00:00:00:00";
				if (text7.Length != 47)
				{
					return false;
				}
				if (Model == "AUDI")
				{
					WriteFile(LogPath, " # ADDR   0x01000490 to 0x01000495 data ");
					WriteFile(LogPath, "POSITIONING_CRC_STRING (ASCII)  = " + text7.Replace(":", " "));
					WriteFile(LogPath, "POSITIONING_CRC_STRING (String) = " + text15 + "\r\n");
				}
				Output.Add(text7);
			}
		}
		return true;
	}

	public bool ContiDDBStringEX(string LogPath, List<string> Refs, List<string> Des, string FAZIT_STRING, string PCB_DMC_STRING, List<string> Output, string Model)
	{
		string text = "";
		string text2 = "";
		string text3 = "";
		string text4 = "";
		string text5 = "";
		string text6 = "";
		string text7 = "";
		string text8 = "";
		WriteFile(LogPath, "***************************************************PROGRAM PARAMETER**************************************************************");
		string text9 = FAZIT_STRING;
		for (int i = 0; i < text9.Length; i++)
		{
			string text10 = String2ASCII16(text9[i].ToString()) + ":";
			text += text10;
		}
		for (int j = text.Length; j < 95; j += 3)
		{
			if (Model == "AUDI")
			{
				text += "FF:";
			}
			if (Model == "MQB")
			{
				text += "00:";
			}
		}
		text = DelPostChar(text, ":");
		if (text.Length != 95)
		{
			return false;
		}
		if (Model == "AUDI")
		{
			WriteFile(LogPath, " # ADDR   0x01000400 to 0x0100041F data ");
		}
		if (Model == "MQB")
		{
			WriteFile(LogPath, " # ADDR   0x00007CB8 24 Bytes ");
		}
		if (Model == "AUDI")
		{
			WriteFile(LogPath, "FAZIT STING (ASCII)             = " + text.Replace(":", " "));
		}
		if (Model == "MQB")
		{
			WriteFile(LogPath, "FAZIT STING (ASCII)             = " + text.Replace(":", " ").Substring(0, 71));
		}
		WriteFile(LogPath, "FAZIT STING (String)            = " + FAZIT_STRING + "\r\n");
		Output.Add(text);
		for (int k = 0; k < Refs.Count; k++)
		{
			if (Refs[k].Trim() == "HW_NO")
			{
				text9 = Des[k].TrimStart();
				for (int l = 0; l < text9.Length; l++)
				{
					string text11 = String2ASCII16(text9[l].ToString()) + ":";
					text3 += text11;
				}
				if (Model == "AUDI")
				{
					text3 += "FF:FF:FF:FF:FF";
				}
				if (Model == "MQB")
				{
					text3 += "00:00:00:00:00";
				}
				if (text3.Length != 47)
				{
					return false;
				}
				if (Model == "AUDI")
				{
					WriteFile(LogPath, " # ADDR   0x01000420 to 0x0100042F data ");
				}
				if (Model == "MQB")
				{
					WriteFile(LogPath, " # ADDR   0x00007CE0 16 Bytes ");
				}
				WriteFile(LogPath, "HW_NO_STRING (ASCII)            = " + text3.Replace(":", " "));
				WriteFile(LogPath, "HW_NO_STRING (String)           = " + Des[k].TrimStart() + "\r\n");
				Output.Add(text3);
			}
		}
		for (int m = 0; m < Refs.Count; m++)
		{
			if (Refs[m].Trim() == "HW_VERS_NO")
			{
				text9 = Des[m].TrimStart();
				for (int n = 0; n < text9.Length; n++)
				{
					string text12 = String2ASCII16(text9[n].ToString()) + ":";
					text4 += text12;
				}
				if (Model == "AUDI")
				{
					text4 += "FF:FF:FF:FF:FF:FF:FF:FF:FF:FF:FF:FF:FF";
				}
				if (Model == "MQB")
				{
					text4 += "00:00:00:00:00:00:00:00:00:00:00:00:00";
				}
				if (text4.Length != 47)
				{
					return false;
				}
				if (Model == "AUDI")
				{
					WriteFile(LogPath, " # ADDR   0x01000430 to 0x0100043F data ");
				}
				if (Model == "MQB")
				{
					WriteFile(LogPath, " # ADDR   0x00007CF0 8 Bytes ");
				}
				WriteFile(LogPath, "HW_VERS_NO_STRING (ASCII)       = " + text4.Replace(":", " "));
				WriteFile(LogPath, "HW_VERS_NO_STRING (String)      = " + Des[m].TrimStart() + "\r\n");
				Output.Add(text4);
			}
		}
		text9 = PCB_DMC_STRING;
		for (int num = 0; num < text9.Length; num++)
		{
			string text13 = String2ASCII16(text9[num].ToString()) + ":";
			text2 += text13;
		}
		for (int num2 = text2.Length; num2 < 95; num2 += 3)
		{
			if (Model == "AUDI")
			{
				text2 += "FF:";
			}
			if (Model == "MQB")
			{
				text2 += "00:";
			}
		}
		text2 = DelPostChar(text2, ":");
		if (text2.Length != 95)
		{
			return false;
		}
		if (Model == "AUDI")
		{
			WriteFile(LogPath, " # ADDR   0x01000440 to 0x0100045F data ");
		}
		if (Model == "MQB")
		{
			WriteFile(LogPath, " # ADDR   0x00007FC8 16 Bytes ");
		}
		if (Model == "AUDI")
		{
			WriteFile(LogPath, "PCB_DMC_STRING (ASCII)          = " + text2.Replace(":", " "));
		}
		if (Model == "MQB")
		{
			WriteFile(LogPath, "PCB_DMC_STRING (ASCII)          = " + text2.Replace(":", " ").Substring(0, 47));
		}
		WriteFile(LogPath, "PCB_DMC_STRING (Sting)          = " + PCB_DMC_STRING + "\r\n");
		Output.Add(text2);
		for (int num3 = 0; num3 < Refs.Count; num3++)
		{
			if (!(Refs[num3].Trim() == "A2C"))
			{
				continue;
			}
			text9 = Des[num3].TrimStart();
			for (int num4 = 0; num4 < text9.Length; num4++)
			{
				string text14 = String2ASCII16(text9[num4].ToString()) + ":";
				text5 += text14;
			}
			for (int num5 = text5.Length; num5 < 95; num5 += 3)
			{
				if (Model == "AUDI")
				{
					text5 += "FF:";
				}
				if (Model == "MQB")
				{
					text5 += "00:";
				}
			}
			text5 = DelPostChar(text5, ":");
			if (text5.Length != 95)
			{
				return false;
			}
			if (Model == "AUDI")
			{
				WriteFile(LogPath, " # ADDR   0x01000460 to 0x0100047F data ");
			}
			if (Model == "MQB")
			{
				WriteFile(LogPath, " # ADDR   0x00007FB8 16 Bytes ");
			}
			if (Model == "AUDI")
			{
				WriteFile(LogPath, "ASC_NO_STRING (ASCII)           = " + text5.Replace(":", " "));
			}
			if (Model == "MQB")
			{
				WriteFile(LogPath, "ASC_NO_STRING (ASCII)           = " + text5.Replace(":", " ").Substring(0, 47));
			}
			WriteFile(LogPath, "ASC_NO_STRING (String)          = " + Des[num3].TrimStart() + "\r\n");
			Output.Add(text5);
		}
		for (int num6 = 0; num6 < Refs.Count; num6++)
		{
			if (Refs[num6].Trim() == "SPARE_PART_NO")
			{
				text9 = Des[num6].TrimStart();
				for (int num7 = 0; num7 < text9.Length; num7++)
				{
					string text15 = String2ASCII16(text9[num7].ToString()) + ":";
					text6 += text15;
				}
				if (Model == "AUDI")
				{
					text6 += "FF:FF:FF:FF:FF";
				}
				if (Model == "MQB")
				{
					text6 += "00:00:00:00:00";
				}
				if (text6.Length != 47)
				{
					return false;
				}
				if (Model == "AUDI")
				{
					WriteFile(LogPath, " # ADDR   0x01000480 to 0x0100048F data ");
				}
				if (Model == "MQB")
				{
					WriteFile(LogPath, " # ADDR   0x00007D48 16 Bytes ");
				}
				WriteFile(LogPath, "SPARE_PART_NO_STRING (ASCII)    = " + text6.Replace(":", " "));
				WriteFile(LogPath, "SPARE_PART_NO_STRING (String)   = " + Des[num6].TrimStart() + "\r\n");
				Output.Add(text6);
			}
		}
		for (int num8 = 0; num8 < Refs.Count; num8++)
		{
			if (Refs[num8].Trim() == "POSITIONING")
			{
				string text16 = "";
				text16 = ((Des[num8].TrimStart().Length <= 3) ? Des[num8].TrimStart() : Des[num8].TrimStart().Substring(0, 3));
				text9 = text16;
				for (int num9 = 0; num9 < text9.Length; num9++)
				{
					string text17 = String2ASCII16(text9[num9].ToString()) + ":";
					text7 += text17;
				}
				byte[] array = HexStringToByteArray(String2ASCII16(text16));
				string cRC16ByPoly = GetCRC16ByPoly(array, (ushort)StrToInt(HexConvert("8408", 10)), "CCITT_0xFFFF");
				text7 = text7 + cRC16ByPoly.Substring(0, 2) + ":";
				text7 = text7 + cRC16ByPoly.Substring(2, 2) + ":";
				text7 += "00:00:00:00:00:00:00:00:00:00:00";
				if (text7.Length != 47)
				{
					return false;
				}
				if (Model == "AUDI")
				{
					WriteFile(LogPath, " # ADDR   0x01000490 to 0x01000495 data ");
					WriteFile(LogPath, "POSITIONING_CRC_STRING (ASCII)  = " + text7.Replace(":", " "));
					WriteFile(LogPath, "POSITIONING_CRC_STRING (String) = " + text16 + "\r\n");
				}
				Output.Add(text7);
			}
			if (Refs[num8].Trim() == "CURRENT_SENSE_HW_VER")
			{
				string text18 = "";
				text18 = ((Des[num8].TrimStart().Length <= 4) ? Des[num8].TrimStart() : Des[num8].TrimStart().Substring(0, 4));
				text9 = text18;
				for (int num10 = 0; num10 < text9.Length; num10++)
				{
					string text19 = String2ASCII16(text9[num10].ToString()) + ":";
					text8 += text19;
				}
				byte[] array2 = HexStringToByteArray(String2ASCII16(text18));
				string cRC16ByPoly2 = GetCRC16ByPoly(array2, (ushort)StrToInt(HexConvert("8408", 10)), "CCITT_0xFFFF");
				text8 = text8 + cRC16ByPoly2.Substring(0, 2) + ":";
				text8 = text8 + cRC16ByPoly2.Substring(2, 2) + ":";
				text8 += "00:00:00:00:00:00:00:00:00:00";
				if (text7.Length != 47)
				{
					return false;
				}
				if (Model == "AUDI")
				{
					WriteFile(LogPath, " # ADDR   0x010004A0 to 0x010004A5 data ");
					WriteFile(LogPath, " CURRENT_SENSE_HW_VER_CRC (ASCII)  = " + text8.Replace(":", " "));
					WriteFile(LogPath, " CURRENT_SENSE_HW_VER_CRC (String) = " + text18 + "\r\n");
				}
				Output.Add(text8);
			}
		}
		return true;
	}

	public void ContiWriteDDBAUDI(string Path, List<string> SingleByteList, string FAZIT_STRING, string HW_NO_STRING, string HW_VERS_NO_STRING, string PCB_DMC_STRING, string ASC_NO_STRING, string SPARE_PART_NO_STRING, string POSITIONING_CRC_STRING)
	{
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		List<string> list3 = new List<string>();
		List<string> list4 = new List<string>();
		List<string> list5 = new List<string>();
		List<string> list6 = new List<string>();
		List<string> list7 = new List<string>();
		list = StrToList(FAZIT_STRING, ':', ToLower: false);
		list2 = StrToList(HW_NO_STRING, ':', ToLower: false);
		list3 = StrToList(HW_VERS_NO_STRING, ':', ToLower: false);
		list4 = StrToList(PCB_DMC_STRING, ':', ToLower: false);
		list5 = StrToList(ASC_NO_STRING, ':', ToLower: false);
		list6 = StrToList(SPARE_PART_NO_STRING, ':', ToLower: false);
		list7 = StrToList(POSITIONING_CRC_STRING, ':', ToLower: false);
		FileDelete(Path);
		FileCreate(Path);
		FileStream fileStream = new FileStream(Path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream, Encoding.Default);
		int num = SingleByteList.Count / 2;
		for (int i = 0; i < num; i++)
		{
			byte[] buffer = HexStringToByteArray(SingleByteList[i]);
			binaryWriter.Write(buffer);
		}
		num = SingleByteList.Count / 2;
		for (int j = num; j < num + 32; j++)
		{
			byte[] buffer2 = HexStringToByteArray(list[j - num]);
			binaryWriter.Write(buffer2);
		}
		num = SingleByteList.Count / 2 + 32;
		for (int k = num; k < num + 16; k++)
		{
			byte[] buffer3 = HexStringToByteArray(list2[k - num]);
			binaryWriter.Write(buffer3);
		}
		num = SingleByteList.Count / 2 + 48;
		for (int l = num; l < num + 16; l++)
		{
			byte[] buffer4 = HexStringToByteArray(list3[l - num]);
			binaryWriter.Write(buffer4);
		}
		num = SingleByteList.Count / 2 + 80;
		for (int m = num; m < num + 32; m++)
		{
			byte[] buffer5 = HexStringToByteArray(list4[m - num]);
			binaryWriter.Write(buffer5);
		}
		num = SingleByteList.Count / 2 + 112;
		for (int n = num; n < num + 32; n++)
		{
			byte[] buffer6 = HexStringToByteArray(list5[n - num]);
			binaryWriter.Write(buffer6);
		}
		num = SingleByteList.Count / 2 + 128;
		for (int num2 = num; num2 < num + 16; num2++)
		{
			byte[] buffer7 = HexStringToByteArray(list6[num2 - num]);
			binaryWriter.Write(buffer7);
		}
		num = SingleByteList.Count / 2 + 144;
		for (int num3 = num; num3 < num + 16; num3++)
		{
			byte[] buffer8 = HexStringToByteArray(list7[num3 - num]);
			binaryWriter.Write(buffer8);
		}
		num = SingleByteList.Count / 2 + 160;
		for (int num4 = num; num4 < num + 96; num4++)
		{
			byte[] buffer9 = HexStringToByteArray("FF");
			binaryWriter.Write(buffer9);
		}
		num = SingleByteList.Count / 2 + 256;
		for (int num5 = num; num5 < SingleByteList.Count; num5++)
		{
			byte[] buffer10 = HexStringToByteArray("FF");
			binaryWriter.Write(buffer10);
		}
		fileStream.Close();
	}

	public void ContiWriteDDBAUDIEX(string Path, List<string> SingleByteList, string FAZIT_STRING, string HW_NO_STRING, string HW_VERS_NO_STRING, string PCB_DMC_STRING, string ASC_NO_STRING, string SPARE_PART_NO_STRING, string POSITIONING_CRC_STRING, string CURRENT_SENSE_HW_VER_CRC_STRING)
	{
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		List<string> list3 = new List<string>();
		List<string> list4 = new List<string>();
		List<string> list5 = new List<string>();
		List<string> list6 = new List<string>();
		List<string> list7 = new List<string>();
		List<string> list8 = new List<string>();
		list = StrToList(FAZIT_STRING, ':', ToLower: false);
		list2 = StrToList(HW_NO_STRING, ':', ToLower: false);
		list3 = StrToList(HW_VERS_NO_STRING, ':', ToLower: false);
		list4 = StrToList(PCB_DMC_STRING, ':', ToLower: false);
		list5 = StrToList(ASC_NO_STRING, ':', ToLower: false);
		list6 = StrToList(SPARE_PART_NO_STRING, ':', ToLower: false);
		list7 = StrToList(POSITIONING_CRC_STRING, ':', ToLower: false);
		list8 = StrToList(CURRENT_SENSE_HW_VER_CRC_STRING, ':', ToLower: false);
		FileDelete(Path);
		FileCreate(Path);
		FileStream fileStream = new FileStream(Path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream, Encoding.Default);
		int num = SingleByteList.Count / 2;
		for (int i = 0; i < num; i++)
		{
			byte[] buffer = HexStringToByteArray(SingleByteList[i]);
			binaryWriter.Write(buffer);
		}
		num = SingleByteList.Count / 2;
		for (int j = num; j < num + 32; j++)
		{
			byte[] buffer2 = HexStringToByteArray(list[j - num]);
			binaryWriter.Write(buffer2);
		}
		num = SingleByteList.Count / 2 + 32;
		for (int k = num; k < num + 16; k++)
		{
			byte[] buffer3 = HexStringToByteArray(list2[k - num]);
			binaryWriter.Write(buffer3);
		}
		num = SingleByteList.Count / 2 + 48;
		for (int l = num; l < num + 16; l++)
		{
			byte[] buffer4 = HexStringToByteArray(list3[l - num]);
			binaryWriter.Write(buffer4);
		}
		num = SingleByteList.Count / 2 + 80;
		for (int m = num; m < num + 32; m++)
		{
			byte[] buffer5 = HexStringToByteArray(list4[m - num]);
			binaryWriter.Write(buffer5);
		}
		num = SingleByteList.Count / 2 + 112;
		for (int n = num; n < num + 32; n++)
		{
			byte[] buffer6 = HexStringToByteArray(list5[n - num]);
			binaryWriter.Write(buffer6);
		}
		num = SingleByteList.Count / 2 + 128;
		for (int num2 = num; num2 < num + 16; num2++)
		{
			byte[] buffer7 = HexStringToByteArray(list6[num2 - num]);
			binaryWriter.Write(buffer7);
		}
		num = SingleByteList.Count / 2 + 144;
		for (int num3 = num; num3 < num + 16; num3++)
		{
			byte[] buffer8 = HexStringToByteArray(list7[num3 - num]);
			binaryWriter.Write(buffer8);
		}
		num = SingleByteList.Count / 2 + 160;
		for (int num4 = num; num4 < num + 16; num4++)
		{
			byte[] buffer9 = HexStringToByteArray(list8[num4 - num]);
			binaryWriter.Write(buffer9);
		}
		num = SingleByteList.Count / 2 + 176;
		for (int num5 = num; num5 < num + 80; num5++)
		{
			byte[] buffer10 = HexStringToByteArray("FF");
			binaryWriter.Write(buffer10);
		}
		num = SingleByteList.Count / 2 + 256;
		for (int num6 = num; num6 < SingleByteList.Count; num6++)
		{
			byte[] buffer11 = HexStringToByteArray("FF");
			binaryWriter.Write(buffer11);
		}
		fileStream.Close();
	}

	public void ContiWriteDDBMQB(string Path, List<string> SingleByteList, string FAZIT_STRING, string HW_NO_STRING, string HW_VERS_NO_STRING, string PCB_DMC_STRING, string ASC_NO_STRING, string SPARE_PART_NO_STRING)
	{
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		List<string> list3 = new List<string>();
		List<string> list4 = new List<string>();
		List<string> list5 = new List<string>();
		List<string> list6 = new List<string>();
		new List<string>();
		list = StrToList(FAZIT_STRING, ':', ToLower: false);
		list2 = StrToList(HW_NO_STRING, ':', ToLower: false);
		list3 = StrToList(HW_VERS_NO_STRING, ':', ToLower: false);
		list4 = StrToList(PCB_DMC_STRING, ':', ToLower: false);
		list5 = StrToList(ASC_NO_STRING, ':', ToLower: false);
		list6 = StrToList(SPARE_PART_NO_STRING, ':', ToLower: false);
		FileDelete(Path);
		FileCreate(Path);
		FileStream fileStream = new FileStream(Path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream, Encoding.Default);
		int num = 31928;
		for (int i = 0; i < num; i++)
		{
			byte[] buffer = HexStringToByteArray(SingleByteList[i]);
			binaryWriter.Write(buffer);
		}
		num = 31929;
		for (int j = num; j < num + 24; j++)
		{
			byte[] buffer2 = HexStringToByteArray(list[j - num]);
			binaryWriter.Write(buffer2);
		}
		num = 31953;
		for (int k = num; k < num + 16; k++)
		{
			byte[] buffer3 = HexStringToByteArray(SingleByteList[k]);
			binaryWriter.Write(buffer3);
		}
		num = 31969;
		for (int l = num; l < num + 16; l++)
		{
			byte[] buffer4 = HexStringToByteArray(list2[l - num]);
			binaryWriter.Write(buffer4);
		}
		num = 31985;
		for (int m = num; m < num + 8; m++)
		{
			byte[] buffer5 = HexStringToByteArray(list3[m - num]);
			binaryWriter.Write(buffer5);
		}
		num = 31992;
		for (int n = num; n < num + 80; n++)
		{
			byte[] buffer6 = HexStringToByteArray(SingleByteList[n]);
			binaryWriter.Write(buffer6);
		}
		num = 32072;
		for (int num2 = num; num2 < num + 16; num2++)
		{
			byte[] buffer7 = HexStringToByteArray(list6[num2 - num]);
			binaryWriter.Write(buffer7);
		}
		num = 32088;
		for (int num3 = num; num3 < num + 608; num3++)
		{
			byte[] buffer8 = HexStringToByteArray(SingleByteList[num3]);
			binaryWriter.Write(buffer8);
		}
		num = 32696;
		for (int num4 = num; num4 < num + 16; num4++)
		{
			byte[] buffer9 = HexStringToByteArray(list5[num4 - num]);
			binaryWriter.Write(buffer9);
		}
		num = 32712;
		for (int num5 = num; num5 < num + 16; num5++)
		{
			byte[] buffer10 = HexStringToByteArray(list4[num5 - num]);
			binaryWriter.Write(buffer10);
		}
		num = 32728;
		for (int num6 = num; num6 < 32768; num6++)
		{
			byte[] buffer11 = HexStringToByteArray(SingleByteList[num6]);
			binaryWriter.Write(buffer11);
		}
		fileStream.Close();
	}

	public string ContiSNGenerate(string PanelSN, int PlusNumber)
	{
		string text = PanelSN.Substring(0, PanelSN.Length - 4);
		string input = PanelSN.Substring(PanelSN.Length - 4, 4);
		string text2 = (StrToInt(input) + PlusNumber).ToString();
		if (StrLength(text2) == 3)
		{
			text2 = "0" + text2;
		}
		if (StrLength(text2) == 2)
		{
			text2 = "00" + text2;
		}
		if (StrLength(text2) == 1)
		{
			text2 = "000" + text2;
		}
		return text + text2;
	}

	public string ContiGetFlashFile(string Path, string A2C)
	{
		string text = "";
		string text2 = "";
		text2 = Path;
		text2 = ((StrToInt(A2C.Substring(A2C.Length - 1, 1)) != 0) ? (text2 + A2C.Substring(0, A2C.Length - 1) + "0\\" + A2C) : (text2 + A2C + "\\" + A2C));
		text = text2 + "\\STATE\\FLASH";
		if (!File.Exists(text))
		{
			text = text2 + "\\STATE\\PROGRAM";
			if (!File.Exists(text))
			{
				text = "Error";
			}
		}
		if (text == "Error")
		{
			text = "";
			text2 = Path;
			text2 = text2 + A2C.Substring(0, A2C.Length - 2) + "00\\" + A2C;
			text = text2 + "\\STATE\\FLASH";
			if (!File.Exists(text))
			{
				text = text2 + "\\STATE\\PROGRAM";
				if (!File.Exists(text))
				{
					text = "Error";
				}
			}
		}
		return text;
	}

	public string ContiReadFlashFile(string FlashFile, int Parameter)
	{
		FileStream fileStream = new FileStream(FlashFile, FileMode.Open, FileAccess.Read);
		StreamReader streamReader = new StreamReader(fileStream, Encoding.Default);
		List<string> list = new List<string>();
		string input;
		while ((input = streamReader.ReadLine()) != null)
		{
			list = StrToList(input, '|', ToLower: false);
		}
		streamReader.Close();
		fileStream.Close();
		if (Parameter >= list.Count)
		{
			return "Error";
		}
		return list[Parameter];
	}

	public string ContiGetATFFile(string FlashFile, string Parameter)
	{
		string result = "Error";
		string input = DelPostChar(FlashFile, "\\");
		input = DelPostChar(input, "\\");
		input = input + "\\" + Parameter + "\\ATF";
		string[] files = Directory.GetFiles(input);
		foreach (string text in files)
		{
			text.Substring(text.LastIndexOf(".") + 1);
			if (".ATF".IndexOf(text.Substring(text.LastIndexOf(".") + 1)) > -1 && !text.Contains("_ICT") && !text.Contains("_EOL"))
			{
				result = text;
			}
		}
		return result;
	}

	public string ContiGetATFParameter(string ATFFile, string Parameter)
	{
		string text = "";
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		List<string> list3 = new List<string>();
		if (!ContiReadATF(ATFFile, list, list2, list3))
		{
			return "Error";
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (list2[i].Trim() == Parameter)
			{
				text = list3[i];
			}
		}
		return DelPostChar(ATFFile, "\\") + "\\" + text.Trim();
	}

	public string ContiGetATFParameterEX(string ATFFile, string Parameter)
	{
		string text = "";
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		List<string> list3 = new List<string>();
		if (!ContiReadATF(ATFFile, list, list2, list3))
		{
			return "Error";
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (list2[i].Trim() == Parameter)
			{
				text = list3[i];
			}
		}
		return text.Trim();
	}

	public string ContiBinToLocal(string OldBinFile, string LocalPath)
	{
		string text = OldBinFile.Substring(OldBinFile.LastIndexOf("\\") + 1);
		string text2 = LocalPath + text;
		if (File.Exists(text2))
		{
			if (File.GetAttributes(text2).ToString().IndexOf("ReadOnly") != -1)
			{
				File.SetAttributes(text2, FileAttributes.Normal);
			}
			FileDelete(text2);
		}
		FileCopy(OldBinFile, text2);
		if (File.GetAttributes(text2).ToString().IndexOf("ReadOnly") != -1)
		{
			File.SetAttributes(text2, FileAttributes.Normal);
		}
		return text2;
	}

	public int ContiBINTool(string Model, string SN, string FAZID, string ATFFile, string OLDBinFile, string NewBinFile, string DebugPath)
	{
		new List<string>();
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		List<string> list3 = new List<string>();
		List<string> singleByteList = new List<string>();
		List<string> singleByteList2 = new List<string>();
		int result = 1;
		string text = DebugPath + "\\" + YearStamp() + MonthStamp() + DayStamp();
		FolderCreate2(text);
		string logPath = text + "\\" + SN + ".LOG";
		string text2 = DebugPath + "TAP2RTS.TXT";
		FileCreate(text2);
		FileDelete(logPath);
		FileCreate(logPath);
		WriteFile(logPath, "****************************************************INPUT PARAMETER***************************************************************");
		WriteFile(logPath, "Model      = " + Model);
		WriteFile(logPath, "SN         = " + SN);
		WriteFile(logPath, "FAZID      = " + FAZID);
		WriteFile(logPath, "ATFFile    = " + ATFFile);
		WriteFile(logPath, "OLDBinFile = " + OLDBinFile);
		WriteFile(logPath, "NewBinFile = " + NewBinFile);
		WriteFile(logPath, "DebugPath  = " + DebugPath);
		if (SUBSTR(SN, 1, 1) == "S")
		{
			SN = SUBSTR(SN, 2, StrLength(SN) - 1);
		}
		WriteFile(logPath, "FINAL SN   = " + SN);
		WriteFile(logPath, "");
		if (!ContiReadATF(ATFFile, list, list2, list3))
		{
			WriteFile(logPath, "Read ATF File Failed!!!");
			return result;
		}
		WriteFile(logPath, "****************************************************ATF PARAMETER*****************************************************************");
		for (int i = 0; i < list.Count; i++)
		{
			WriteFile(logPath, list[i] + " " + list2[i] + " " + list3[i]);
		}
		WriteFile(logPath, "");
		if (Model == "AUDI")
		{
			List<string> lineList = new List<string>();
			ContiReadDDB(OLDBinFile, singleByteList, lineList);
			List<string> list4 = new List<string>();
			ContiDDBStringEX(logPath, list2, list3, FAZID, SN, list4, "AUDI");
			if (list4.Count != 7 && list4.Count != 8)
			{
				WriteFile(logPath, "Output count should be 7 Or 8 but actual is " + list4.Count + "!!!");
				return result;
			}
			for (int j = 0; j < list4.Count; j++)
			{
				string strings = list4[j].Replace(":", "");
				WriteFile(text2, strings);
			}
			if (list4.Count == 7)
			{
				ContiWriteDDBAUDI(NewBinFile, singleByteList, list4[0], list4[1], list4[2], list4[3], list4[4], list4[5], list4[6]);
			}
			if (list4.Count == 8)
			{
				ContiWriteDDBAUDIEX(NewBinFile, singleByteList, list4[0], list4[1], list4[2], list4[3], list4[4], list4[5], list4[6], list4[7]);
			}
			result = 0;
		}
		else if (Model == "MQB")
		{
			List<string> lineList2 = new List<string>();
			ContiReadDDB(OLDBinFile, singleByteList2, lineList2);
			List<string> list5 = new List<string>();
			ContiDDBString(logPath, list2, list3, FAZID, SN, list5, "MQB");
			if (list5.Count != 6)
			{
				WriteFile(logPath, "Output count should be 6 but actual is " + list5.Count + "!!!");
			}
			for (int k = 0; k < list5.Count; k++)
			{
			}
			ContiWriteDDBMQB(NewBinFile, singleByteList2, list5[0], list5[1], list5[2], list5[3], list5[4], list5[5]);
			result = 0;
		}
		return result;
	}

	public int ContiToolBox(string Input_ImageFile, string Output_ToolBox, string Debug_Path)
	{
		List<string> list = new List<string>();
		List<string> lineList = new List<string>();
		string text = Debug_Path + DateTime.Now.ToString("yyyyMMdd//");
		FolderCreate2(text);
		text += "TOOLBOX.DEB";
		FileCreate(text);
		WriteFile(text, DateTime.Now.ToString("yyyyMMddhhmmss: ") + "Input ImageFile = " + Input_ImageFile);
		WriteFile(text, DateTime.Now.ToString("yyyyMMddhhmmss: ") + "Output_ToolBox  = " + Output_ToolBox);
		ContiReadDDB(Input_ImageFile, list, lineList);
		string text2 = list[1].ToString().ToUpper() + list[0].ToString().ToUpper();
		int num = StrToInt(HexConvert(text2, 10));
		WriteFile(text, DateTime.Now.ToString("yyyyMMddhhmmss: ") + "Tool Box Length = " + text2 + "(Hex) or " + num + "(Dec)");
		FileDelete(Output_ToolBox);
		FileCreate(Output_ToolBox);
		List<string> list2 = new List<string>();
		for (int i = 0; i < num + 2; i++)
		{
			list2.Add(list[i]);
		}
		int num2 = list2.Count / 8;
		WriteFile(text, DateTime.Now.ToString("yyyyMMddhhmmss: ") + "LineNo = " + (num2 + 1));
		int num3 = list2.Count % 8;
		string empty = string.Empty;
		for (int j = 0; j < num2 * 8; j += 8)
		{
			empty = list2[j] + list2[j + 1] + list2[j + 2] + list2[j + 3] + list2[j + 4] + list2[j + 5] + list2[j + 6] + list2[j + 7];
			WriteFileDefault(Output_ToolBox, empty.ToUpper());
		}
		empty = string.Empty;
		for (int k = 0; k < num3; k++)
		{
			empty += list2[num2 * 8 + k];
		}
		WriteFileDefault(Output_ToolBox, empty.ToUpper());
		WriteFile(text, DateTime.Now.ToString("yyyyMMddhhmmss: ") + num3 + " Bytes In Last Line =" + empty.ToUpper());
		WriteFile(text, "");
		return num2 + 1;
	}

	public string ICH_ContiSNGenerate(string PanelSN, int PlusNumber)
	{
		string text = PanelSN.Substring(0, PanelSN.Length - 4);
		string input = PanelSN.Substring(PanelSN.Length - 4, 4);
		int num = StrToInt(input);
		if (num % 2 == 0 && PlusNumber == 1)
		{
			PlusNumber = -1;
		}
		string text2 = (num + PlusNumber).ToString();
		if (StrLength(text2) == 3)
		{
			text2 = "0" + text2;
		}
		if (StrLength(text2) == 2)
		{
			text2 = "00" + text2;
		}
		if (StrLength(text2) == 1)
		{
			text2 = "000" + text2;
		}
		return text + text2;
	}

	public string ICH_ContiGetFlashFile(string Path, string HW_MFLB, string SW_MFLB)
	{
		string text = "";
		text = string.Concat(Path + HW_MFLB + "\\" + SW_MFLB, "\\STATE\\EP");
		if (!File.Exists(text))
		{
			text = "Error";
		}
		return text;
	}

	public string ICH_ContiReadFlashFile(string FlashFile, int Parameter)
	{
		FileStream fileStream = new FileStream(FlashFile, FileMode.Open, FileAccess.Read);
		StreamReader streamReader = new StreamReader(fileStream, Encoding.Default);
		List<string> list = new List<string>();
		string input;
		while ((input = streamReader.ReadLine()) != null)
		{
			list = StrToList(input, '|', ToLower: false);
		}
		streamReader.Close();
		fileStream.Close();
		if (Parameter >= list.Count)
		{
			return "Error";
		}
		return list[Parameter];
	}

	public string ICH_ContiGetATFFile(string FlashFile, string Parameter)
	{
		string result = "Error";
		string input = DelPostChar(FlashFile, "\\");
		input = DelPostChar(input, "\\");
		input = input + "\\" + Parameter + "\\ATF";
		string[] files = Directory.GetFiles(input);
		foreach (string text in files)
		{
			text.Substring(text.LastIndexOf(".") + 1);
			if (".ATF".IndexOf(text.Substring(text.LastIndexOf(".") + 1)) > -1 && !text.Contains("_ICT") && !text.Contains("_EOL"))
			{
				result = text;
			}
		}
		return result;
	}

	public bool ICH_ContiReadATF(string Input, List<string> Items, List<string> Refs, List<string> Des)
	{
		FileStream fileStream = new FileStream(Input, FileMode.Open, FileAccess.Read);
		StreamReader streamReader = new StreamReader(fileStream, Encoding.Default);
		string item = "Variants";
		int num = 1;
		string text;
		while ((text = streamReader.ReadLine()) != null)
		{
			int num2 = text.IndexOf(":");
			num = 1;
			if (num2 > 0 && text.Substring(0, 2) != "/*")
			{
				num = -1;
			}
			if (num2 <= 0 || num >= 0)
			{
				continue;
			}
			string[] array = StrToArray(text, ':');
			if (text.IndexOf("FLASH :") >= 0)
			{
				item = "FLASH";
				continue;
			}
			if (text.IndexOf("EOL :") >= 0)
			{
				item = "EOL";
				continue;
			}
			if (text.IndexOf("MOTOR_PARAMETERS :") >= 0)
			{
				item = "MOTOR";
				continue;
			}
			if (text.IndexOf("LABEL :") >= 0)
			{
				item = "LABEL";
				continue;
			}
			if (text.IndexOf("DMC :") >= 0)
			{
				item = "DMC";
				continue;
			}
			if (text.IndexOf("FAZIT :") >= 0)
			{
				item = "FAZIT";
				continue;
			}
			if (text.IndexOf("TOOLBOX :") >= 0)
			{
				item = "TOOLBOX";
				continue;
			}
			if (text.IndexOf("CAN :") >= 0)
			{
				item = "CAN";
				continue;
			}
			Items.Add(item);
			Refs.Add(array[0].Trim());
			string text2 = "";
			if (array[1].IndexOf("/") > 0)
			{
				text2 = DelPostChar(array[1], "/");
				text2 = text2.Trim();
			}
			else
			{
				text2 = array[1].Trim();
			}
			text2 = text2.Replace("\"", "");
			Des.Add(text2);
		}
		streamReader.Close();
		fileStream.Close();
		if (Items.Count != Refs.Count || Items.Count != Des.Count)
		{
			return false;
		}
		return true;
	}

	public string ICH_ContiS19ToLocal(string OldS19File, string LocalPath)
	{
		string text = OldS19File.Substring(OldS19File.LastIndexOf("\\") + 1);
		string text2 = LocalPath + text;
		if (File.Exists(text2))
		{
			if (File.GetAttributes(text2).ToString().IndexOf("ReadOnly") != -1)
			{
				File.SetAttributes(text2, FileAttributes.Normal);
			}
			FileDelete(text2);
		}
		FileCopy(OldS19File, text2);
		if (File.GetAttributes(text2).ToString().IndexOf("ReadOnly") != -1)
		{
			File.SetAttributes(text2, FileAttributes.Normal);
		}
		return text2;
	}

	public string ICH_ContiGetATFParameter(string ATFFile, string Parameter)
	{
		string text = "";
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		List<string> list3 = new List<string>();
		if (!ICH_ContiReadATF(ATFFile, list, list2, list3))
		{
			return "Erro1r";
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (list2[i].Trim().Contains(Parameter))
			{
				text = list3[i];
			}
		}
		return DelPostChar(ATFFile, "\\") + "\\DATA\\" + text.Trim();
	}

	public string ICH_ContiOutputATFParameter(string ATFFile, string TAP2RTS)
	{
		string result = "";
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		List<string> list3 = new List<string>();
		if (!ICH_ContiReadATF(ATFFile, list, list2, list3))
		{
			return "Erro1r";
		}
		FileCreate(TAP2RTS);
		for (int i = 0; i < list.Count; i++)
		{
			WriteFile(TAP2RTS, list2[i] + "=" + list3[i]);
		}
		return result;
	}

	public void CompalReadDDB(string Path, List<string> SingleByteList, List<string> LineList)
	{
		FileStream fileStream = new FileStream(Path, FileMode.Open, FileAccess.Read);
		BinaryReader binaryReader = new BinaryReader(fileStream);
		int num = (int)fileStream.Length;
		int num2 = 0;
		string text = string.Empty;
		while (num > 0)
		{
			num2++;
			string text2 = Convert.ToString(binaryReader.ReadByte(), 16);
			if (text2.Length == 1)
			{
				text2 = "0" + text2;
			}
			text = text + text2 + " ";
			if (num2 % 16 == 0 && num2 > 0)
			{
				LineList.Add(text.ToUpper());
				text = string.Empty;
			}
			num--;
			SingleByteList.Add(text2);
		}
		fileStream.Close();
		binaryReader.Close();
	}

	public string CompalBitConvert(string Input)
	{
		string text = "";
		string text2 = ":";
		List<string> list = new List<string>();
		new List<string>();
		for (int i = 0; i < Input.Length; i++)
		{
			string input = HexConvert(Input[i].ToString(), 2);
			input = StringReverse(input);
			input = BinaryConvert(input, 16);
			list.Add(input.ToUpper());
		}
		for (int j = 0; j < list.Count; j += 2)
		{
			if (j == list.Count - 2)
			{
				text2 = "";
			}
			text = text + list[j + 1] + list[j] + text2;
		}
		return text;
	}

	public string CompalBitConvertEXO(string Input)
	{
		string text = "";
		string text2 = ":";
		List<string> list = new List<string>();
		new List<string>();
		int num = 1;
		string text3 = "";
		for (int i = 0; i < Input.Length; i++)
		{
			text3 += Input[i];
			if (num % 2 == 0)
			{
				list.Add(text3.ToUpper());
				text3 = "";
			}
			num++;
		}
		for (num = 0; num < list.Count; num++)
		{
			if (num == list.Count - 1)
			{
				text2 = "";
			}
			text = text + list[list.Count - num - 1] + text2;
		}
		return text;
	}

	public string CompalCheckSumCal(string MACAddress, List<string> SingleByteList)
	{
		string text = "";
		string text2 = "";
		int num = 0;
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		string text3 = "";
		list2 = StrToList(MACAddress, ':', ToLower: false);
		for (int i = 0; i < list2.Count; i += 2)
		{
			text3 = list2[i + 1] + list2[i];
			text3 = HexConvert(text3, 10);
			list.Add(text3);
		}
		for (int j = 1030; j < 1150; j += 2)
		{
			text2 = SingleByteList[j + 1] + SingleByteList[j];
			text2 = CompalBitConvert(text2);
			text2 = text2.Replace(":", "");
			text2 = HexConvert(text2, 10);
			list.Add(text2);
		}
		for (int k = 0; k < list.Count; k++)
		{
			num += StrToInt(list[k]);
		}
		num = num + StrToInt(HexConvert("A505", 10)) - StrToInt(list[19]);
		num %= 65536;
		num = StrToInt(HexConvert("BABA", 10)) - num;
		if (num < 0)
		{
			num += 65536;
		}
		text = num.ToString();
		text = DecimalConvert(text, 16);
		if (text.Length == 3)
		{
			text = "0" + text;
		}
		else if (text.Length == 2)
		{
			text = "00" + text;
		}
		else if (text.Length == 1)
		{
			text = "000" + text;
		}
		return CompalBitConvert(text);
	}

	public string CompalCheckSumCalEXO(string MACAddress, List<string> SingleByteList)
	{
		string text = "";
		new List<string>();
		int num = 0;
		uint num2 = 0u;
		uint num3 = 0u;
		uint num4 = uint.MaxValue;
		uint num5 = 3988292384u;
		for (num = 0; num < 136; num++)
		{
			num2 = Convert.ToUInt32(SingleByteList[num + 1140], 16);
			num3 = 0u;
			while (num3 < 8)
			{
				text = text + num4.ToString("x") + "_";
				num4 = (num4 >> 1) ^ ((((num4 ^ num2) & 1) == 1) ? num5 : 0);
				num3++;
				num2 >>= 1;
			}
		}
		text = (~num4).ToString("x");
		return CompalBitConvertEXO(text);
	}

	public void CompalWriteDDB(string Path, string MACAddress, string CheckSumStr, List<string> SingleByteList)
	{
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		FileDelete(Path);
		FileCreate(Path);
		FileStream fileStream = new FileStream(Path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream, Encoding.Default);
		list = StrToList(MACAddress, ':', ToLower: false);
		list2 = StrToList(CheckSumStr, ':', ToLower: false);
		for (int i = 0; i < SingleByteList.Count; i++)
		{
			switch (i)
			{
			case 1024:
			{
				byte[] buffer9 = HexStringToByteArray(list[0]);
				binaryWriter.Write(buffer9);
				break;
			}
			case 1025:
			{
				byte[] buffer8 = HexStringToByteArray(list[1]);
				binaryWriter.Write(buffer8);
				break;
			}
			case 1026:
			{
				byte[] buffer7 = HexStringToByteArray(list[2]);
				binaryWriter.Write(buffer7);
				break;
			}
			case 1027:
			{
				byte[] buffer6 = HexStringToByteArray(list[3]);
				binaryWriter.Write(buffer6);
				break;
			}
			case 1028:
			{
				byte[] buffer5 = HexStringToByteArray(list[4]);
				binaryWriter.Write(buffer5);
				break;
			}
			case 1029:
			{
				byte[] buffer4 = HexStringToByteArray(list[5]);
				binaryWriter.Write(buffer4);
				break;
			}
			case 1150:
			{
				byte[] buffer3 = HexStringToByteArray(list2[1]);
				binaryWriter.Write(buffer3);
				break;
			}
			case 1151:
			{
				byte[] buffer2 = HexStringToByteArray(list2[0]);
				binaryWriter.Write(buffer2);
				break;
			}
			default:
			{
				byte[] buffer = HexStringToByteArray(SingleByteList[i]);
				binaryWriter.Write(buffer);
				break;
			}
			}
		}
		fileStream.Close();
	}

	public void CompalWriteDDBEX(string Path, string MACAddress, int Location, List<string> SingleByteList)
	{
		List<string> list = new List<string>();
		new List<string>();
		FileDelete(Path);
		FileCreate(Path);
		FileStream fileStream = new FileStream(Path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream, Encoding.Default);
		list = StrToList(MACAddress, ':', ToLower: false);
		for (int i = 0; i < SingleByteList.Count; i++)
		{
			if (i == Location)
			{
				byte[] buffer = HexStringToByteArray(list[0]);
				binaryWriter.Write(buffer);
			}
			else if (i == Location + 1)
			{
				byte[] buffer2 = HexStringToByteArray(list[1]);
				binaryWriter.Write(buffer2);
			}
			else if (i == Location + 2)
			{
				byte[] buffer3 = HexStringToByteArray(list[2]);
				binaryWriter.Write(buffer3);
			}
			else if (i == Location + 3)
			{
				byte[] buffer4 = HexStringToByteArray(list[3]);
				binaryWriter.Write(buffer4);
			}
			else if (i == Location + 4)
			{
				byte[] buffer5 = HexStringToByteArray(list[4]);
				binaryWriter.Write(buffer5);
			}
			else if (i == Location + 5)
			{
				byte[] buffer6 = HexStringToByteArray(list[5]);
				binaryWriter.Write(buffer6);
			}
			else
			{
				byte[] buffer7 = HexStringToByteArray(SingleByteList[i]);
				binaryWriter.Write(buffer7);
			}
		}
		fileStream.Close();
	}

	public void CompalWriteDDBEXO(string Path, string MACAddress, int MACLocation, string CheckSumStr, int SumLocation, List<string> SingleByteList)
	{
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		FileDelete(Path);
		FileCreate(Path);
		FileStream fileStream = new FileStream(Path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream, Encoding.Default);
		list = StrToList(MACAddress, ':', ToLower: false);
		list2 = StrToList(CheckSumStr, ':', ToLower: false);
		for (int i = 0; i < SingleByteList.Count; i++)
		{
			if (i == MACLocation)
			{
				byte[] buffer = HexStringToByteArray(list[0]);
				binaryWriter.Write(buffer);
			}
			else if (i == MACLocation + 1)
			{
				byte[] buffer2 = HexStringToByteArray(list[1]);
				binaryWriter.Write(buffer2);
			}
			else if (i == MACLocation + 2)
			{
				byte[] buffer3 = HexStringToByteArray(list[2]);
				binaryWriter.Write(buffer3);
			}
			else if (i == MACLocation + 3)
			{
				byte[] buffer4 = HexStringToByteArray(list[3]);
				binaryWriter.Write(buffer4);
			}
			else if (i == MACLocation + 4)
			{
				byte[] buffer5 = HexStringToByteArray(list[4]);
				binaryWriter.Write(buffer5);
			}
			else if (i == MACLocation + 5)
			{
				byte[] buffer6 = HexStringToByteArray(list[5]);
				binaryWriter.Write(buffer6);
			}
			else if (i == SumLocation)
			{
				byte[] buffer7 = HexStringToByteArray(list2[3]);
				binaryWriter.Write(buffer7);
			}
			else if (i == SumLocation + 1)
			{
				byte[] buffer8 = HexStringToByteArray(list2[2]);
				binaryWriter.Write(buffer8);
			}
			else if (i == SumLocation + 2)
			{
				byte[] buffer9 = HexStringToByteArray(list2[1]);
				binaryWriter.Write(buffer9);
			}
			else if (i == SumLocation + 3)
			{
				byte[] buffer10 = HexStringToByteArray(list2[0]);
				binaryWriter.Write(buffer10);
			}
			else
			{
				byte[] buffer11 = HexStringToByteArray(SingleByteList[i]);
				binaryWriter.Write(buffer11);
			}
		}
		fileStream.Close();
	}

	public string MolexSNGenerate(string PanelSN, int PlusNumber)
	{
		string text = "";
		if (StrLength(PanelSN) < 4)
		{
			return "ERROR";
		}
		if (PanelSN.Substring(0, 4) != "0000")
		{
			return "ERROR";
		}
		string input = PanelSN.Substring(PanelSN.IndexOf("|") + 1);
		List<string> list = new List<string>();
		list = StrToList(input, ',', ToLower: false);
		for (int i = 0; i < list.Count; i++)
		{
			string text2 = list[i];
			int num = -1;
			num = ((i >= 9) ? StrToInt(text2.Substring(0, 2)) : StrToInt(text2.Substring(0, 1)));
			if (num < 0)
			{
				return "ERROR";
			}
			if (num == PlusNumber)
			{
				text = DelPreChar(text2, "|");
			}
		}
		if (text == "")
		{
			return "ERROR";
		}
		return text;
	}

	public string ZFSNGenerate(string PanelSN, int PlusNumber)
	{
		string text = PanelSN.Substring(0, PanelSN.Length - 7);
		string input = PanelSN.Substring(PanelSN.Length - 7, 7);
		string text2 = (StrToInt(input) + PlusNumber).ToString();
		if (StrLength(text2) == 5)
		{
			text2 = "0" + text2;
		}
		if (StrLength(text2) == 5)
		{
			text2 = "00" + text2;
		}
		if (StrLength(text2) == 4)
		{
			text2 = "000" + text2;
		}
		if (StrLength(text2) == 3)
		{
			text2 = "0000" + text2;
		}
		if (StrLength(text2) == 2)
		{
			text2 = "00000" + text2;
		}
		if (StrLength(text2) == 1)
		{
			text2 = "000000" + text2;
		}
		return text + text2;
	}
}
