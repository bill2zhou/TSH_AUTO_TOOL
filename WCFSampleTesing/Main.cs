using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using KCString;
using Newtonsoft.Json;
using TSHlog2Dell;
using WCFSampleTesing.ServiceReference1;

namespace WCFSampleTesing;

public class Main : Form
{
	// ---- TAP 文件路径 ----
	private const string DefaultCsvPath = "C:\\ProgramData\\Teradyne\\TSDevPro\\TAP\\TAP_Controller\\";

	private const string DefaultVersionFile = "C:\\Teradyne\\TSDevPro\\TestStationfixedsprs.txt";

	// ---- 运行时状态 ----
	private readonly KCSTRING kcString = new KCSTRING();

	private string CSVPath = DefaultCsvPath;

	private string VersionFile = DefaultVersionFile;

	// 目录 / 文件缺失只提示一次，避免每秒刷屏
	private bool CsvPathWarned;

	private bool VersionFileWarned;

	private string VER = "";

	private bool ReadConfigFlag = true;

	// ---- 上传服务器用的配置项，来自 app.config ----
	private string SAPPLANT = "";

	private string LINENAME = "";

	private string GROUPNAME = "";

	private string CUSTCODE = "";

	private string PROCESS = "";

	private string FACTORYSN = "";

	private string MOUNTSIDE = "";

	private string EQUIPTYPE = "";

	private string OPEUSER = "";

	private string LOGFILEPATH = "";

	private string SENDMSG2SERVER = "";

	private string APPSTOP = "";

	private string DELLLOG = "";

	private string INPUTPATH = "";

	private string OUTPUTPATH = "";

	// ---- 界面控件 ----
	private IContainer components;

	private MenuStrip menuStrip1;

	private TableLayoutPanel layoutPanel;

	private GroupBox groupBoxSend;

	private ToolStripMenuItem setupToolStripMenuItem;

	private ToolStripMenuItem configToolStripMenuItem;

	private RichTextBox richTextBoxSend;

	private GroupBox groupBoxReceive;

	private Label labelResult;

	private GroupBox groupBoxTester;

	private Label labelStatus;

	private ListBox listBoxStatus;

	private GroupBox groupBoxStatus;

	private Timer pollTimer;

	private Label labelTester;

	private ToolStripMenuItem exitToolStripMenuItem;

	// ---- 托盘与开机启动 ----
	private const string AutoStartRegistryKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";

	private const string AutoStartValueName = "TSH_AUTO_TOOL";

	private NotifyIcon trayIcon;

	private ContextMenuStrip trayMenu;

	private ToolStripMenuItem showWindowMenuItem;

	private ToolStripMenuItem autoStartMenuItem;

	private ToolStripMenuItem exitAppMenuItem;

	// 已确认退出（托盘退出 / 关机），不再拦截关闭
	private bool exitConfirmed;

	private bool trayTipShown;

	public Main()
	{
		InitializeComponent();
		base.FormClosing += Main_FormClosing;
		InitializeTrayIcon();
	}

	/// <summary>
	/// 轮询主流程：检查运行配置、TAP 版本文件和 csv 目录，每次处理一个待处理文件。
	/// </summary>
	private void pollTimer_Tick(object sender, EventArgs e)
	{
		APPSTOP = AppConfig.Get("APPSTOP");
		if (APPSTOP == "1")
		{
			return;
		}
		pollTimer.Interval = 10000;
		if (listBoxStatus.Items.Count > 200)
		{
			listBoxStatus.Items.Clear();
		}
		if (ReadConfigFlag)
		{
			ReadConfig();
			ReadConfigFlag = false;
			ScrollStatusToBottom();
		}
		// 版本文件与 csv 目录可能比本程序晚就绪，先检查，都就绪后再处理
		bool versionReady = UpdateSoftVersion();
		string[] csvFiles = GetCsvFiles();
		if (!versionReady)
		{
			pollTimer.Interval = 1000;
			return;
		}
		foreach (string csvFile in csvFiles)
		{
			if (!IsCsvFile(csvFile))
			{
				continue;
			}
			if (csvFile.Contains("State_TSH"))
			{
				ProcessStateFile(csvFile);
				break;
			}
			if (csvFile.Contains("Results_"))
			{
				ProcessResultsFile(csvFile);
				break;
			}
		}
		pollTimer.Interval = 1000;
	}

	/// <summary>
	/// 处理 State_TSH_*.csv：移动到当日日志目录，读取前两行（键名 / 键值）后上传。
	/// </summary>
	private void ProcessStateFile(string csvFile)
	{
		ReadConfig();
		AppendStatus(csvFile);
		string logFile = MoveToLogFolder(csvFile, "State_TSH_", "Status_ATE_");
		AppendStatus(logFile);
		List<string> keys;
		List<string> values;
		using (StreamReader reader = new StreamReader(logFile, Encoding.GetEncoding("gb2312")))
		{
			string keyLine = reader.ReadLine();
			if (string.IsNullOrEmpty(keyLine))
			{
				return;
			}
			AppendStatus(keyLine);
			keys = kcString.StrToList(keyLine, ',', ToLower: false);
			string valueLine = reader.ReadLine();
			if (string.IsNullOrEmpty(valueLine))
			{
				return;
			}
			AppendStatus(valueLine);
			values = kcString.StrToList(valueLine, ',', ToLower: false);
		}
		for (int i = 0; i < values.Count; i++)
		{
			AppendStatus(values[i]);
			// 第 2、3 个字段是 Tester 信息，显示在右上角
			if (i == 1)
			{
				labelTester.Text = values[i];
			}
			if (i == 2)
			{
				labelTester.Text = labelTester.Text + "  " + values[i];
			}
		}
		SendStateFile(keys, values, logFile, "State");
		ScrollStatusToBottom();
	}

	/// <summary>
	/// 处理 Results_*.csv：移动到当日日志目录，读取表头和全部数据行后上传。
	/// </summary>
	private void ProcessResultsFile(string csvFile)
	{
		ReadConfig();
		AppendStatus(csvFile);
		string logFile = MoveToLogFolder(csvFile, "Results_", "Data_ATE_");
		AppendStatus(logFile);
		List<string> keys;
		List<string> values = new List<string>();
		using (StreamReader reader = new StreamReader(logFile, Encoding.GetEncoding("gb2312")))
		{
			string keyLine = reader.ReadLine();
			if (string.IsNullOrEmpty(keyLine))
			{
				return;
			}
			AppendStatus(keyLine);
			keys = kcString.StrToList(keyLine, ',', ToLower: false);
			string valueLine;
			while ((valueLine = reader.ReadLine()) != null)
			{
				AppendStatus(valueLine);
				values.AddRange(kcString.StrToList(valueLine, ',', ToLower: false));
			}
		}
		SendStateFile(keys, values, logFile, "Results");
		ScrollStatusToBottom();
	}

	/// <summary>
	/// 把 TAP 生成的 csv 移动到当日日志目录并改名，返回移动后的完整路径。
	/// </summary>
	/// <param name="sourcePrefix">源文件名前缀，例如 State_TSH_。</param>
	/// <param name="targetPrefix">目标文件名前缀，例如 Status_ATE_。</param>
	private string MoveToLogFolder(string csvFile, string sourcePrefix, string targetPrefix)
	{
		string fileName = kcString.DelPreChar(csvFile, "\\").Replace(sourcePrefix, targetPrefix);
		string folder = LOGFILEPATH + DateTime.Now.ToString("yyyyMMdd") + "\\";
		kcString.FolderCreate2(folder);
		string logFile = folder + fileName;
		if (File.Exists(logFile))
		{
			kcString.FileDelete(logFile);
		}
		kcString.FileMove(csvFile, logFile);
		return logFile;
	}

	/// <summary>在状态栏追加一行带时间戳的消息。</summary>
	private void AppendStatus(string message)
	{
		listBoxStatus.Items.Add(DateTime.Now.ToString("yyyyMMddHHmmss: ") + message);
	}

	/// <summary>状态栏滚动到最新一行。</summary>
	private void ScrollStatusToBottom()
	{
		if (listBoxStatus.Items.Count > 0)
		{
			listBoxStatus.TopIndex = listBoxStatus.Items.Count - 1;
		}
	}

	/// <summary>服务器回执只有 True / False：True 显示 OK（绿色），False 显示 NG（红色）并把原因写进日志。</summary>
	private void UpdateResultLabel(bool status, string message)
	{
		labelResult.Text = status ? "OK" : "NG";
		labelResult.ForeColor = status ? Color.Green : Color.Red;
		if (!status && !string.IsNullOrEmpty(message))
		{
			AppendStatus("SERVER RESPONSE = " + message);
		}
	}

	/// <summary>右上角显示 TAP 版本号与上传开关状态。</summary>
	private void UpdateStatusLabel()
	{
		string text = "VER " + (VER.Length == 0 ? "-" : VER) + "    PUSH " + (SENDMSG2SERVER == "1" ? "ON" : "OFF");
		if (labelStatus.Text != text)
		{
			labelStatus.Text = text;
		}
	}

	/// <summary>判断文件是否为 .csv 文件（忽略大小写）。</summary>
	private static bool IsCsvFile(string path)
	{
		return string.Equals(Path.GetExtension(path), ".csv", StringComparison.OrdinalIgnoreCase);
	}

	/// <summary>
	/// 按 <paramref name="model"/> 组装日志文件内容并上传到服务器。
	/// </summary>
	/// <param name="keyList">字段名列表，方法内会改写为固定表头。</param>
	/// <param name="valueList">字段值列表，方法内会改写为上传用的值。</param>
	/// <param name="logFileName">本地日志文件完整路径。</param>
	/// <param name="model">State（设备状态）或 Results（测试结果）。</param>
	/// <returns>上传成功返回 true。</returns>
	public bool SendStateFile(List<string> keyList, List<string> valueList, string logFileName, string model)
	{
		bool sendFailed = false;
		List<string> dellLogItems = new List<string>();
		if (model == "State")
		{
			if (keyList.Count != valueList.Count || keyList.Count != 5)
			{
				return false;
			}
			List<string> sourceValues = new List<string>(valueList);
			keyList.Clear();
			keyList.Add("Time");
			keyList.Add("EventCode");
			keyList.Add("EventMessage");
			keyList.Add("ErrorCode");
			keyList.Add("ErrorMessage");
			valueList.Clear();
			valueList.Add(DataStr2Data(sourceValues[0]));
			valueList.Add(sourceValues[1]);
			valueList.Add(EventCodeToMessage(sourceValues[1]));
			valueList.Add(sourceValues[3]);
			valueList.Add(sourceValues[4]);
			WriteLogFile(logFileName, BuildHeaderLine(keyList), BuildDataLine(valueList));
		}
		else if (model == "Results")
		{
			const int FieldsPerRecord = 11;
			if (keyList.Count != FieldsPerRecord || valueList.Count % FieldsPerRecord != 0)
			{
				return false;
			}
			int recordCount = valueList.Count / FieldsPerRecord;
			List<string> sourceValues = new List<string>(valueList);
			keyList.Clear();
			keyList.Add("Time");
			keyList.Add("FixtureID");
			keyList.Add("ProgramName");
			keyList.Add("InWaitingTime");
			keyList.Add("OutWaitingTime");
			for (int i = 0; i < recordCount; i++)
			{
				keyList.Add("SN" + (i + 1));
				keyList.Add("Result" + (i + 1));
			}
			valueList.Clear();
			valueList.Add(DataStr2Data(sourceValues[0]));
			valueList.Add(sourceValues[1]);
			dellLogItems.Add(sourceValues[1]);
			valueList.Add(kcString.DelPreChar(sourceValues[2], "\\"));
			valueList.Add(ToSeconds(sourceValues[3]).ToString());
			int outWaitingSeconds = ToSeconds(sourceValues[6]);
			if (outWaitingSeconds == 0)
			{
				outWaitingSeconds++;
			}
			valueList.Add(outWaitingSeconds.ToString());
			for (int i = 0; i < recordCount; i++)
			{
				int offset = FieldsPerRecord * i;
				string barcode = sourceValues[offset + 9];
				string[] barcodeParts = barcode.Split('_');
				dellLogItems.Add(barcode);
				valueList.Add(barcodeParts.Length > 2 ? barcodeParts[2] : "errorbarcode");
				valueList.Add(sourceValues[offset + 10]);
				dellLogItems.Add(sourceValues[offset + 10]);
			}
			string headerLine = BuildHeaderLine(keyList);
			string dataLine = BuildDataLine(valueList);
			labelTester.Text = "";
			for (int i = 0; i < recordCount; i++)
			{
				int offset = 2 * i;
				headerLine = headerLine + "," + keyList[5 + offset] + "," + keyList[6 + offset];
				dataLine = dataLine + "," + valueList[5 + offset] + "," + valueList[6 + offset];
				labelTester.Text = labelTester.Text + keyList[5 + offset] + ":" + valueList[5 + offset] + "  " + keyList[6 + offset] + ":" + valueList[6 + offset] + "  ";
			}
			headerLine += ",CT";
			headerLine += ",Version";
			keyList.Add("CT");
			keyList.Add("Version");
			DateTime startTime = DateTime.ParseExact(DataStr2Data(sourceValues[5]), "yyyy/MM/dd HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US"));
			DateTime endTime = DateTime.ParseExact(DataStr2Data(sourceValues[7]), "yyyy/MM/dd HH:mm:ss", CultureInfo.CreateSpecificCulture("en-US"));
			TimeSpan cycleTime = kcString.TimeDiff(startTime, endTime);
			int cycleSeconds = cycleTime.Minutes * 60 + cycleTime.Seconds;
			dellLogItems.Add(startTime.ToString("yyyy-MM-dd HH:mm:ss"));
			dellLogItems.Add(endTime.ToString("yyyy-MM-dd HH:mm:ss"));
			dataLine = dataLine + "," + cycleSeconds;
			valueList.Add(cycleSeconds.ToString());
			dellLogItems.Add(cycleSeconds.ToString());
			dataLine = dataLine + "," + VER;
			valueList.Add(VER);
			WriteLogFile(logFileName, headerLine, dataLine);
		}
		else
		{
			return false;
		}
		// 交给 Dell 日志工具处理（DELLLOG=0 表示只清空输入目录）
		if (dellLogItems.Count > 5)
		{
			if (DELLLOG != "0")
			{
				new TSHDell(dellLogItems, INPUTPATH, OUTPUTPATH, DELLLOG).process();
			}
			else
			{
				foreach (string path in Directory.GetFiles(INPUTPATH))
				{
					kcString.FileDelete(path);
				}
			}
		}
		ClientTransferData clientTransferData = new ClientTransferData
		{
			CustCode = CUSTCODE,
			EquipType = EQUIPTYPE,
			FactorySN = FACTORYSN,
			GroupName = GROUPNAME,
			LineName = LINENAME,
			LogFileName = kcString.DelPreChar(logFileName, "\\"),
			MountSide = MOUNTSIDE,
			OpeUser = OPEUSER,
			Process = PROCESS,
			SapPlant = SAPPLANT
		};
		List<SingleLogItemDef> logFieldList = new List<SingleLogItemDef>();
		for (int i = 0; i < keyList.Count; i++)
		{
			logFieldList.Add(new SingleLogItemDef
			{
				KeyStr = keyList[i],
				KeyValue = valueList[i]
			});
		}
		clientTransferData.LogFieldList = logFieldList.ToArray();
		richTextBoxSend.Text = BuildSendPreview(clientTransferData);
		richTextBoxSend.SelectionStart = 0;
		richTextBoxSend.SelectionLength = 0;
		if (SENDMSG2SERVER == "1")
		{
			ExecutionResult executionResult = new ExecutionResult();
			ServiceLibraryClient serviceLibraryClient = new ServiceLibraryClient();
			try
			{
				string response = serviceLibraryClient.CompalClientTransferEquipSerialData(JsonConvert.SerializeObject(clientTransferData));
				executionResult = JsonConvert.DeserializeObject<ExecutionResult>(response);
				serviceLibraryClient.Close();
			}
			catch (Exception ex)
			{
				executionResult.Status = false;
				executionResult.Message = ex.Message;
				sendFailed = true;
			}
			UpdateResultLabel(executionResult.Status, executionResult.Message);
		}
		else
		{
			// 未开启上传时明确显示，避免残留上一次的 OK / NG
			labelResult.Text = "-";
			labelResult.ForeColor = SystemColors.GrayText;
		}
		return !sendFailed;
	}

	/// <summary>组合日志表头，第 2、3 列固定为产线号与机台号。</summary>
	private string BuildHeaderLine(List<string> keys)
	{
		return keys[0] + ",Line,MachineID," + keys[1] + "," + keys[2] + "," + keys[3] + "," + keys[4];
	}

	/// <summary>组合日志数据行，第 2、3 列固定为产线号与机台号。</summary>
	private string BuildDataLine(List<string> values)
	{
		return values[0] + "," + LINENAME + "," + FACTORYSN + "," + values[1] + "," + values[2] + "," + values[3] + "," + values[4];
	}

	/// <summary>把表头和数据行写入日志文件（先清空旧文件）。</summary>
	private void WriteLogFile(string logFileName, string headerLine, string dataLine)
	{
		kcString.FileDelete(logFileName);
		kcString.FileCreate(logFileName);
		kcString.WriteFile(logFileName, headerLine);
		kcString.WriteFile(logFileName, dataLine);
	}

	/// <summary>
	/// 把待上传报文整理成“一行一个 key : value”显示在界面上，方便现场核对；
	/// 真正发给服务器的仍然是完整 JSON。
	/// </summary>
	private static string BuildSendPreview(ClientTransferData data)
	{
		List<KeyValuePair<string, string>> headerLines = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("CustCode", data.CustCode),
			new KeyValuePair<string, string>("EquipType", data.EquipType),
			new KeyValuePair<string, string>("FactorySN", data.FactorySN),
			new KeyValuePair<string, string>("GroupName", data.GroupName),
			new KeyValuePair<string, string>("LineName", data.LineName),
			new KeyValuePair<string, string>("LogFileName", data.LogFileName),
			new KeyValuePair<string, string>("MountSide", data.MountSide),
			new KeyValuePair<string, string>("OpeUser", data.OpeUser),
			new KeyValuePair<string, string>("Process", data.Process),
			new KeyValuePair<string, string>("SapPlant", data.SapPlant)
		};
		// 键名对齐，等宽字体下列看起来更整齐
		int keyWidth = 0;
		foreach (KeyValuePair<string, string> line in headerLines)
		{
			keyWidth = Math.Max(keyWidth, line.Key.Length);
		}
		if (data.LogFieldList != null)
		{
			foreach (SingleLogItemDef item in data.LogFieldList)
			{
				keyWidth = Math.Max(keyWidth, (item.KeyStr ?? "").Length);
			}
		}
		StringBuilder builder = new StringBuilder();
		foreach (KeyValuePair<string, string> line in headerLines)
		{
			builder.AppendLine(line.Key.PadRight(keyWidth) + " : " + (line.Value ?? ""));
		}
		if (data.LogFieldList != null && data.LogFieldList.Length > 0)
		{
			builder.AppendLine(new string('-', keyWidth + 3));
			foreach (SingleLogItemDef item in data.LogFieldList)
			{
				builder.AppendLine((item.KeyStr ?? "").PadRight(keyWidth) + " : " + (item.KeyValue ?? ""));
			}
		}
		return builder.ToString();
	}

	/// <summary>TAP 事件代码转文字说明。</summary>
	private static string EventCodeToMessage(string eventCode)
	{
		switch (eventCode)
		{
		case "C01":
			return "ProductionStart";
		case "C02":
			return "TroubleStopStart";
		case "C03":
			return "TroubleStopEnd";
		case "C04":
			return "ProductionEnd";
		default:
			return "";
		}
	}

	/// <summary>把 hh:mm:ss 格式的时间转成秒，格式不对时返回 0。</summary>
	private int ToSeconds(string time)
	{
		List<string> parts = kcString.StrToList(time, ':', ToLower: false);
		if (parts.Count < 3)
		{
			return 0;
		}
		return kcString.StrToInt(parts[0]) * 3600 + kcString.StrToInt(parts[1]) * 60 + kcString.StrToInt(parts[2]);
	}

	/// <summary>
	/// 把 TAP 的日期时间文本统一成 yyyy/MM/dd HH:mm:ss。
	/// 支持 "M/d/yyyy h:mm:ss tt" 和 "d-M-yyyy h:mm:ss tt" 两种写法，无法识别时原样返回。
	/// </summary>
	public string DataStr2Data(string dataStr)
	{
		List<string> parts = kcString.StrToList(dataStr, ' ', ToLower: false);
		if (parts.Count != 3)
		{
			return dataStr;
		}
		string datePart;
		if (dataStr.Contains("/"))
		{
			List<string> dateParts = kcString.StrToList(parts[0], '/', ToLower: false);
			if (dateParts.Count != 3)
			{
				return dataStr;
			}
			datePart = Pad2(dateParts[0]) + "/" + Pad2(dateParts[1]) + "/" + dateParts[2];
		}
		else if (dataStr.Contains("-"))
		{
			List<string> dateParts = kcString.StrToList(parts[0], '-', ToLower: false);
			if (dateParts.Count != 3)
			{
				return dataStr;
			}
			datePart = Pad2(dateParts[1]) + "/" + Pad2(dateParts[2]) + "/" + dateParts[0];
		}
		else
		{
			return dataStr;
		}
		List<string> timeParts = kcString.StrToList(parts[1], ':', ToLower: false);
		if (timeParts.Count != 3)
		{
			return dataStr;
		}
		string text = datePart + " " + Pad2(timeParts[0]) + ":" + Pad2(timeParts[1]) + ":" + Pad2(timeParts[2]) + " " + parts[2];
		DateTime value;
		if (!DateTime.TryParseExact(text, "MM/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out value))
		{
			return dataStr;
		}
		return value.ToString("yyyy/MM/dd HH:mm:ss");
	}

	/// <summary>小于 10 的数字补前导 0。</summary>
	private string Pad2(string value)
	{
		return kcString.StrToInt(value) < 10 && value.Length < 2 ? "0" + value : value;
	}

	/// <summary>从 app.config 重新读取全部配置项，并在状态栏回显。</summary>
	public void ReadConfig()
	{
		SAPPLANT = AppConfig.Get("SAPPLANT");
		LINENAME = AppConfig.Get("LINENAME");
		GROUPNAME = AppConfig.Get("GROUPNAME");
		CUSTCODE = AppConfig.Get("CUSTCODE");
		PROCESS = AppConfig.Get("PROCESS");
		MOUNTSIDE = AppConfig.Get("MOUNTSIDE");
		EQUIPTYPE = AppConfig.Get("EQUIPTYPE");
		OPEUSER = AppConfig.Get("OPEUSER");
		LOGFILEPATH = AppConfig.Get("LOGFILEPATH");
		SENDMSG2SERVER = AppConfig.Get("SENDMSG2SERVER");
		FACTORYSN = AppConfig.Get("FACTORYSN");
		APPSTOP = AppConfig.Get("APPSTOP");
		DELLLOG = AppConfig.Get("DELLLOG", "ALL");
		INPUTPATH = AppConfig.Get("INPUTPATH");
		OUTPUTPATH = AppConfig.Get("OUTPUTPATH");
		UpdateSoftVersion();
		AppendStatus("SAPPLANT = " + SAPPLANT);
		AppendStatus("LINENAME = " + LINENAME);
		AppendStatus("GROUPNAME = " + GROUPNAME);
		AppendStatus("CUSTCODE = " + CUSTCODE);
		AppendStatus("PROCESS = " + PROCESS);
		AppendStatus("MOUNTSIDE = " + MOUNTSIDE);
		AppendStatus("EQUIPTYPE = " + EQUIPTYPE);
		AppendStatus("OPEUSER = " + OPEUSER);
		AppendStatus("FACTORYSN = " + FACTORYSN);
		AppendStatus("LOGFILEPATH = " + LOGFILEPATH);
		AppendStatus("INPUTPATH = " + INPUTPATH);
		AppendStatus("OUTPUTPATH = " + OUTPUTPATH);
		UpdateStatusLabel();
	}

	/// <summary>
	/// 读取 TAP 版本文件；文件不存在时只在状态栏提示一次，等文件生成后自动读取并继续处理。
	/// </summary>
	/// <returns>版本文件已就绪返回 true。</returns>
	private bool UpdateSoftVersion()
	{
		VersionFile = AppConfig.Get("VERSIONFILE", DefaultVersionFile);
		if (VersionFile.Length == 0)
		{
			VersionFile = DefaultVersionFile;
		}
		if (!File.Exists(VersionFile))
		{
			if (!VersionFileWarned)
			{
				VersionFileWarned = true;
				AppendStatus("VERSION FILE NOT FOUND, WAITING = " + VersionFile);
				ScrollStatusToBottom();
			}
			return false;
		}
		VersionFileWarned = false;
		try
		{
			string version = kcString.Soft_Version();
			if (version != VER)
			{
				VER = version;
				AppendStatus("TSDevPro VERSION = " + VER);
				ScrollStatusToBottom();
				UpdateStatusLabel();
			}
			return true;
		}
		catch (Exception ex)
		{
			AppendStatus("READ VERSION FILE FAILED = " + ex.Message);
			ScrollStatusToBottom();
			return false;
		}
	}

	/// <summary>
	/// 读取 TAP_Controller 目录下的文件；目录不存在时只提示一次并返回空数组，等目录出现后自动继续处理。
	/// </summary>
	private string[] GetCsvFiles()
	{
		string csvPath = AppConfig.Get("CSVPATH", DefaultCsvPath);
		if (csvPath.Length == 0)
		{
			csvPath = DefaultCsvPath;
		}
		if (!csvPath.EndsWith("\\"))
		{
			csvPath += "\\";
		}
		CSVPath = csvPath;
		if (!Directory.Exists(CSVPath))
		{
			if (!CsvPathWarned)
			{
				CsvPathWarned = true;
				AppendStatus("CSVPATH NOT FOUND, WAITING = " + CSVPath);
				ScrollStatusToBottom();
			}
			return new string[0];
		}
		CsvPathWarned = false;
		try
		{
			return Directory.GetFiles(CSVPath);
		}
		catch (Exception ex)
		{
			AppendStatus("READ CSVPATH FAILED = " + ex.Message);
			ScrollStatusToBottom();
			return new string[0];
		}
	}

	private void Main_Load(object sender, EventArgs e)
	{
		labelTester.Text = "";
		labelResult.Text = "-";
		labelResult.ForeColor = SystemColors.GrayText;
		UpdateStatusLabel();
		// 启动时清掉上次残留的 csv，避免重复上传
		foreach (string csvFile in GetCsvFiles())
		{
			if (IsCsvFile(csvFile))
			{
				kcString.FileDelete(csvFile);
			}
		}
	}

	/// <summary>
	/// 关闭窗口时最小化到托盘；只有托盘“退出”、关机或任务管理器结束时才真正退出。
	/// </summary>
	private void Main_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (exitConfirmed || e.CloseReason == CloseReason.WindowsShutDown || e.CloseReason == CloseReason.TaskManagerClosing)
		{
			DisposeTrayIcon();
			return;
		}
		e.Cancel = true;
		HideToTray();
	}

	/// <summary>
	/// 加载 exe 内置的 app.ico 作为窗口与托盘图标，保证和 exe 文件图标一致。
	/// </summary>
	private static Icon LoadApplicationIcon()
	{
		try
		{
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("app.ico"))
			{
				if (stream != null)
				{
					return new Icon(stream);
				}
			}
		}
		catch (Exception)
		{
			// 资源缺失时退回系统默认图标
		}
		return SystemIcons.Application;
	}

	/// <summary>创建托盘图标与右键菜单。</summary>
	private void InitializeTrayIcon()
	{
		showWindowMenuItem = new ToolStripMenuItem("显示主界面");
		showWindowMenuItem.Click += delegate
		{
			ShowMainWindow();
		};
		autoStartMenuItem = new ToolStripMenuItem("开机自动启动");
		autoStartMenuItem.Checked = IsAutoStartEnabled();
		autoStartMenuItem.Click += AutoStartMenuItem_Click;
		exitAppMenuItem = new ToolStripMenuItem("退出");
		exitAppMenuItem.Click += ExitAppMenuItem_Click;
		trayMenu = new ContextMenuStrip();
		trayMenu.Items.AddRange(new ToolStripItem[4]
		{
			showWindowMenuItem,
			autoStartMenuItem,
			new ToolStripSeparator(),
			exitAppMenuItem
		});
		trayIcon = new NotifyIcon
		{
			Icon = this.Icon ?? SystemIcons.Application,
			Text = "TSH_AUTO_TOOL",
			Visible = true,
			ContextMenuStrip = trayMenu
		};
		trayIcon.DoubleClick += delegate
		{
			ShowMainWindow();
		};
		if (components != null)
		{
			components.Add(trayMenu);
			components.Add(trayIcon);
		}
	}

	/// <summary>隐藏主窗口，只在托盘保留图标；首次隐藏时气泡提示一次。</summary>
	private void HideToTray()
	{
		ShowInTaskbar = false;
		Hide();
		if (!trayTipShown && trayIcon != null)
		{
			trayTipShown = true;
			trayIcon.ShowBalloonTip(3000, "TSH_AUTO_TOOL", "程序已最小化到托盘，双击图标可恢复窗口", ToolTipIcon.Info);
		}
	}

	/// <summary>从托盘恢复主窗口。</summary>
	private void ShowMainWindow()
	{
		ShowInTaskbar = true;
		Show();
		if (WindowState == FormWindowState.Minimized)
		{
			WindowState = FormWindowState.Normal;
		}
		Activate();
	}

	/// <summary>退出前移除托盘图标，避免留下失效图标。</summary>
	private void DisposeTrayIcon()
	{
		if (trayIcon == null)
		{
			return;
		}
		trayIcon.Visible = false;
		trayIcon.Dispose();
		trayIcon = null;
	}

	/// <summary>托盘菜单：切换开机自动启动。</summary>
	private void AutoStartMenuItem_Click(object sender, EventArgs e)
	{
		bool enable = !autoStartMenuItem.Checked;
		if (!SetAutoStart(enable))
		{
			return;
		}
		autoStartMenuItem.Checked = enable;
		AppendStatus("AUTO START = " + (enable ? "ON" : "OFF"));
	}

	/// <summary>托盘菜单：真正退出程序。</summary>
	private void ExitAppMenuItem_Click(object sender, EventArgs e)
	{
		exitConfirmed = true;
		Close();
	}

	/// <summary>开机启动写入注册表的命令行；exe 路径加引号，避免路径含空格时启动失败。</summary>
	private static string AutoStartCommand => "\"" + Application.ExecutablePath + "\"";

	/// <summary>是否已设置开机自动启动；顺带校验登记的路径是否还是当前 exe。</summary>
	private static bool IsAutoStartEnabled()
	{
		try
		{
			using (RegistryKey key = Registry.CurrentUser.OpenSubKey(AutoStartRegistryKey, false))
			{
				string command = key?.GetValue(AutoStartValueName) as string;
				return string.Equals(command, AutoStartCommand, StringComparison.OrdinalIgnoreCase);
			}
		}
		catch (Exception)
		{
			return false;
		}
	}

	/// <summary>写入 / 删除 HKCU 的 Run 项，成功返回 true。</summary>
	private bool SetAutoStart(bool enabled)
	{
		try
		{
			using (RegistryKey key = Registry.CurrentUser.OpenSubKey(AutoStartRegistryKey, true) ?? Registry.CurrentUser.CreateSubKey(AutoStartRegistryKey))
			{
				if (key == null)
				{
					AppendStatus("AUTO START FAILED = REGISTRY NOT ACCESSIBLE");
					return false;
				}
				if (enabled)
				{
					key.SetValue(AutoStartValueName, AutoStartCommand);
				}
				else
				{
					key.DeleteValue(AutoStartValueName, false);
				}
			}
			return true;
		}
		catch (Exception ex)
		{
			AppendStatus("AUTO START FAILED = " + ex.Message);
			return false;
		}
	}

	private void exitToolStripMenuItem_Click(object sender, EventArgs e)
	{
		exitConfirmed = true;
		Close();
	}

	private void configToolStripMenuItem_Click(object sender, EventArgs e)
	{
		AppConfig.Set("APPSTOP", "1");
		ReadConfigFlag = true;
		new Config().Show();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this.menuStrip1 = new System.Windows.Forms.MenuStrip();
		this.setupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.configToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.layoutPanel = new System.Windows.Forms.TableLayoutPanel();
		this.groupBoxSend = new System.Windows.Forms.GroupBox();
		this.richTextBoxSend = new System.Windows.Forms.RichTextBox();
		this.groupBoxReceive = new System.Windows.Forms.GroupBox();
		this.labelResult = new System.Windows.Forms.Label();
		this.groupBoxTester = new System.Windows.Forms.GroupBox();
		this.labelTester = new System.Windows.Forms.Label();
		this.labelStatus = new System.Windows.Forms.Label();
		this.groupBoxStatus = new System.Windows.Forms.GroupBox();
		this.listBoxStatus = new System.Windows.Forms.ListBox();
		this.pollTimer = new System.Windows.Forms.Timer(this.components);
		this.menuStrip1.SuspendLayout();
		this.layoutPanel.SuspendLayout();
		this.groupBoxSend.SuspendLayout();
		this.groupBoxReceive.SuspendLayout();
		this.groupBoxTester.SuspendLayout();
		this.groupBoxStatus.SuspendLayout();
		base.SuspendLayout();
		this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.setupToolStripMenuItem });
		this.menuStrip1.Location = new System.Drawing.Point(0, 0);
		this.menuStrip1.Name = "menuStrip1";
		this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
		this.menuStrip1.Size = new System.Drawing.Size(529, 24);
		this.menuStrip1.TabIndex = 1;
		this.menuStrip1.Text = "menuStrip1";
		this.setupToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.configToolStripMenuItem, this.exitToolStripMenuItem });
		this.setupToolStripMenuItem.Name = "setupToolStripMenuItem";
		this.setupToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
		this.setupToolStripMenuItem.Text = "Setup";
		this.configToolStripMenuItem.Name = "configToolStripMenuItem";
		this.configToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
		this.configToolStripMenuItem.Text = "Config";
		this.configToolStripMenuItem.Click += new System.EventHandler(configToolStripMenuItem_Click);
		this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
		this.exitToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
		this.exitToolStripMenuItem.Text = "Exit";
		this.exitToolStripMenuItem.Click += new System.EventHandler(exitToolStripMenuItem_Click);
		// 四个区域交给表格布局，窗口缩放时同步拉伸；回执只占一行
		this.layoutPanel.ColumnCount = 1;
		this.layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
		this.layoutPanel.Controls.Add(this.groupBoxSend, 0, 0);
		this.layoutPanel.Controls.Add(this.groupBoxReceive, 0, 1);
		this.layoutPanel.Controls.Add(this.groupBoxTester, 0, 2);
		this.layoutPanel.Controls.Add(this.groupBoxStatus, 0, 3);
		this.layoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutPanel.Location = new System.Drawing.Point(0, 24);
		this.layoutPanel.Name = "layoutPanel";
		this.layoutPanel.Padding = new System.Windows.Forms.Padding(6);
		this.layoutPanel.RowCount = 4;
		this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
		this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
		this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 56F));
		this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
		this.layoutPanel.Size = new System.Drawing.Size(884, 611);
		this.layoutPanel.TabIndex = 2;
		// 发送报文：只读，JSON 是单行，关闭自动换行并给出横向滚动条
		this.groupBoxSend.Controls.Add(this.richTextBoxSend);
		this.groupBoxSend.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupBoxSend.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
		this.groupBoxSend.Name = "groupBoxSend";
		this.groupBoxSend.Padding = new System.Windows.Forms.Padding(8, 4, 8, 8);
		this.groupBoxSend.TabIndex = 0;
		this.groupBoxSend.TabStop = false;
		this.groupBoxSend.Text = "Send Message";
		this.richTextBoxSend.BackColor = System.Drawing.SystemColors.Window;
		this.richTextBoxSend.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.richTextBoxSend.Dock = System.Windows.Forms.DockStyle.Fill;
		this.richTextBoxSend.Font = new System.Drawing.Font("Consolas", 9F);
		this.richTextBoxSend.Name = "richTextBoxSend";
		this.richTextBoxSend.ReadOnly = true;
		this.richTextBoxSend.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Both;
		this.richTextBoxSend.TabIndex = 0;
		this.richTextBoxSend.Text = "";
		this.richTextBoxSend.WordWrap = false;
		// 服务器回执：只占一行，True 显示 OK，False 显示 NG
		this.groupBoxReceive.Controls.Add(this.labelResult);
		this.groupBoxReceive.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupBoxReceive.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
		this.groupBoxReceive.Name = "groupBoxReceive";
		this.groupBoxReceive.Padding = new System.Windows.Forms.Padding(8, 2, 8, 2);
		this.groupBoxReceive.TabIndex = 1;
		this.groupBoxReceive.TabStop = false;
		this.groupBoxReceive.Text = "Receive Message";
		this.labelResult.Dock = System.Windows.Forms.DockStyle.Fill;
		this.labelResult.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
		this.labelResult.Name = "labelResult";
		this.labelResult.TabIndex = 0;
		this.labelResult.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		// 机台信息：左侧 Tester，右侧版本与上传开关
		this.groupBoxTester.Controls.Add(this.labelTester);
		this.groupBoxTester.Controls.Add(this.labelStatus);
		this.groupBoxTester.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupBoxTester.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
		this.groupBoxTester.Name = "groupBoxTester";
		this.groupBoxTester.Padding = new System.Windows.Forms.Padding(8, 2, 8, 2);
		this.groupBoxTester.TabIndex = 2;
		this.groupBoxTester.TabStop = false;
		this.groupBoxTester.Text = "Tester Information";
		this.labelTester.AutoSize = true;
		this.labelTester.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
		this.labelTester.Location = new System.Drawing.Point(12, 22);
		this.labelTester.Name = "labelTester";
		this.labelTester.Size = new System.Drawing.Size(43, 19);
		this.labelTester.TabIndex = 0;
		this.labelTester.Text = "Tester";
		this.labelStatus.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelStatus.AutoSize = true;
		this.labelStatus.ForeColor = System.Drawing.SystemColors.GrayText;
		this.labelStatus.Location = new System.Drawing.Point(690, 26);
		this.labelStatus.Name = "labelStatus";
		this.labelStatus.Size = new System.Drawing.Size(0, 12);
		this.labelStatus.TabIndex = 1;
		this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		// 运行日志：长路径不折行，需要时出现横向滚动条
		this.groupBoxStatus.Controls.Add(this.listBoxStatus);
		this.groupBoxStatus.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupBoxStatus.Margin = new System.Windows.Forms.Padding(0);
		this.groupBoxStatus.Name = "groupBoxStatus";
		this.groupBoxStatus.Padding = new System.Windows.Forms.Padding(8, 4, 8, 8);
		this.groupBoxStatus.TabIndex = 3;
		this.groupBoxStatus.TabStop = false;
		this.groupBoxStatus.Text = "Status";
		this.listBoxStatus.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.listBoxStatus.Dock = System.Windows.Forms.DockStyle.Fill;
		this.listBoxStatus.Font = new System.Drawing.Font("Consolas", 9F);
		this.listBoxStatus.FormattingEnabled = true;
		this.listBoxStatus.HorizontalScrollbar = true;
		this.listBoxStatus.IntegralHeight = false;
		this.listBoxStatus.Name = "listBoxStatus";
		this.listBoxStatus.TabIndex = 0;
		this.pollTimer.Enabled = true;
		this.pollTimer.Interval = 1000;
		this.pollTimer.Tick += new System.EventHandler(pollTimer_Tick);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(884, 635);
		base.Controls.Add(this.layoutPanel);
		base.Controls.Add(this.menuStrip1);
		base.Icon = LoadApplicationIcon();
		base.MainMenuStrip = this.menuStrip1;
		base.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		base.MinimumSize = new System.Drawing.Size(720, 520);
		base.Name = "Main";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "TSH_AUTO_TOOL";
		base.Load += new System.EventHandler(Main_Load);
		this.menuStrip1.ResumeLayout(false);
		this.menuStrip1.PerformLayout();
		this.layoutPanel.ResumeLayout(false);
		this.groupBoxSend.ResumeLayout(false);
		this.groupBoxReceive.ResumeLayout(false);
		this.groupBoxTester.ResumeLayout(false);
		this.groupBoxTester.PerformLayout();
		this.groupBoxStatus.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
