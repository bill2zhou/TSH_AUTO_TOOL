using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace TSHlog2Dell;

public class TSHDell
{
	private List<string> heads = new List<string>();

	private string inputpath;

	private string outputpath;

	private string set;

	private static string oldsku;

	public TSHDell(List<string> pp, string input, string output, string config)
	{
		heads = pp;
		inputpath = input;
		outputpath = output;
		set = config;
	}

	public static void Getmatch(string patten, string matstr, string[] output)
	{
		foreach (Match item in new Regex(patten).Matches(matstr))
		{
			GroupCollection groups = item.Groups;
			int num = 0;
			foreach (Group item2 in groups)
			{
				output[num] = item2.Value;
				num++;
			}
		}
	}

	public static bool Ismatchs(string patten, string matstr)
	{
		if (new Regex(patten).IsMatch(matstr))
		{
			return true;
		}
		return false;
	}

	public int proc_log(string text, string output1, string output2, Heads HD)
	{
		bool flag = false;
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		List<string> list3 = new List<string>();
		List<string> list4 = new List<string>();
		List<string> list5 = new List<string>();
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		Dictionary<string, List<string>> dictionary2 = new Dictionary<string, List<string>>();
		Dictionary<string, List<string>> dictionary3 = new Dictionary<string, List<string>>();
		string path = HD.SKU + ".txt";
		// SKU_FILE_PROC = 0 时，即使存在 SKU 文件也不做该文件的解析与补写处理
		bool skuFileProc = ConfigurationManager.AppSettings["SKU_FILE_PROC"] != "0";
		StreamWriter streamWriter = new StreamWriter(output1);
		StreamWriter streamWriter2 = new StreamWriter(output2);
		streamWriter.WriteLine(HD.HEAD);
		streamWriter2.WriteLine(HD.HEAD);
		streamWriter.WriteLine("\n\n\n");
		streamWriter2.WriteLine("\n\n\n");
		streamWriter.WriteLine(HD.STATION);
		streamWriter2.WriteLine(HD.STATION);
		streamWriter.WriteLine(HD.MACHINE_ID);
		streamWriter2.WriteLine(HD.MACHINE_ID);
		streamWriter.WriteLine(HD.FIXTURE_ID);
		streamWriter2.WriteLine(HD.FIXTURE_ID);
		streamWriter.WriteLine(HD.OP_ID);
		streamWriter2.WriteLine(HD.OP_ID);
		streamWriter.WriteLine(HD.MODEL);
		streamWriter2.WriteLine(HD.MODEL);
		streamWriter.WriteLine(HD.SERIAL_NUMBER1);
		streamWriter2.WriteLine(HD.SERIAL_NUMBER2);
		streamWriter.WriteLine(HD.PPID1);
		streamWriter2.WriteLine(HD.PPID2);
		streamWriter.WriteLine(HD.MODULENAME);
		streamWriter2.WriteLine(HD.MODULENAME);
		streamWriter.WriteLine(HD.TEST_START_TIME);
		streamWriter2.WriteLine(HD.TEST_START_TIME);
		streamWriter.WriteLine(HD.TEST_END_TIME);
		streamWriter2.WriteLine(HD.TEST_END_TIME);
		streamWriter.WriteLine(HD.TEST_TIME);
		streamWriter2.WriteLine(HD.TEST_TIME);
		streamWriter.WriteLine(HD.TEST_RESULT1);
		streamWriter2.WriteLine(HD.TEST_RESULT2);
		streamWriter.WriteLine("\n\n\n\n");
		streamWriter2.WriteLine("\n\n\n\n");
		using (StreamReader streamReader = new StreamReader(text))
		{
			int num = 0;
			string matstr;
			while ((matstr = streamReader.ReadLine()) != null)
			{
				string patten = "Test Site 1.*";
				if (Ismatchs(patten, matstr))
				{
					num++;
				}
				if (num == 3)
				{
					break;
				}
				patten = "Test Site 2.*";
				if (Ismatchs(patten, matstr))
				{
					flag = true;
				}
				patten = "(\\w+)(.)(\\d+\\.\\w+)\\(([\\w.]*)\\,([\\w.]*)\\)R";
				if (Ismatchs(patten, matstr))
				{
					string[] array = new string[20];
					Getmatch(patten, matstr, array);
					string item = ((!(array[2] == "=")) ? (array[1] + ";" + array[3] + ";" + array[4] + ";" + array[5] + ";Ohm;FAIL;Resistor") : (array[1] + ";" + array[3] + ";" + array[4] + ";" + array[5] + ";Ohm;PASS;Resistor"));
					if (flag)
					{
						list2.Add(item);
					}
					else
					{
						list.Add(item);
					}
				}
				patten = "(\\w+)(.)(\\d+\\.\\w+)\\(([\\w.]*)\\,([\\w.]+)\\)C";
				if (Ismatchs(patten, matstr))
				{
					string[] array2 = new string[20];
					Getmatch(patten, matstr, array2);
					string item = ((!(array2[2] == "=")) ? (array2[1] + ";" + array2[3] + ";" + array2[4] + ";" + array2[5] + ";F;FAIL;Capacitor") : (array2[1] + ";" + array2[3] + ";" + array2[4] + ";" + array2[5] + ";F;PASS;Capacitor"));
					if (flag)
					{
						list2.Add(item);
					}
					else
					{
						list.Add(item);
					}
				}
				patten = "(\\w+)(.)(\\d+\\.\\w+)\\(([\\w.]+)\\,([\\w.]+)\\)V";
				if (Ismatchs(patten, matstr))
				{
					string[] array3 = new string[20];
					Getmatch(patten, matstr, array3);
					string item = ((!(array3[2] == "=")) ? (array3[1] + ";" + array3[3] + ";" + array3[4] + ";" + array3[5] + ";V(dc);FAIL;Power") : (array3[1] + ";" + array3[3] + ";" + array3[4] + ";" + array3[5] + ";V(dc);PASS;Power"));
					if (flag)
					{
						list2.Add(item);
					}
					else
					{
						list.Add(item);
					}
				}
				patten = "\\(S\\s*([,0-9]+)";
				if (!Ismatchs(patten, matstr))
				{
					continue;
				}
				string[] array4 = new string[20];
				Getmatch(patten, matstr, array4);
				string[] array5 = array4[1].Split(',');
				foreach (string item2 in array5)
				{
					if (flag)
					{
						list4.Add(item2);
					}
					else
					{
						list3.Add(item2);
					}
				}
			}
		}
		if (skuFileProc && File.Exists(path))
		{
			Random random = new Random();
			using (StreamReader streamReader2 = new StreamReader(path))
			{
				bool flag2 = false;
				string key = "";
				string matstr;
				while ((matstr = streamReader2.ReadLine()) != null)
				{
					if (matstr.Contains(":"))
					{
						key = matstr.Split(':')[1];
						if (!dictionary2.ContainsKey(key))
						{
							dictionary2[key] = new List<string>();
						}
					}
					if (matstr.Contains("begin"))
					{
						flag2 = true;
						continue;
					}
					if (matstr.Contains("end"))
					{
						flag2 = false;
					}
					if (flag2)
					{
						dictionary2[key].Add(matstr.Trim());
					}
				}
			}
			dictionary3 = dictionary2;
			foreach (string key6 in dictionary2.Keys)
			{
				string text2 = "";
				string text3 = "";
				double num2 = random.NextDouble() * 10.0;
				if (num2 < 1.0)
				{
					num2 *= 1000.0;
					text3 = $"{num2:F6}M";
				}
				else
				{
					text3 = $"{num2:F6}";
				}
				foreach (string item3 in list3)
				{
					if (!(item3 == key6))
					{
						continue;
					}
					text2 = item3;
					foreach (string item4 in dictionary2[key6])
					{
						string value = item4 + ";" + text3 + ";10;;Ohm;FAIL;Open";
						streamWriter.WriteLine(value);
					}
				}
				if (!(key6 != text2))
				{
					continue;
				}
				foreach (string item5 in dictionary2[key6])
				{
					string value2 = item5 + ";" + text3 + ";10;;Ohm;PASS;Open";
					streamWriter.WriteLine(value2);
				}
			}
			foreach (string key7 in dictionary3.Keys)
			{
				string text4 = "";
				string text5 = "";
				double num3 = random.NextDouble() * 10.0;
				if (num3 < 1.0)
				{
					num3 *= 1000.0;
					text5 = $"{num3:F6}M";
				}
				else
				{
					text5 = $"{num3:F6}";
				}
				foreach (string item6 in list4)
				{
					if (!(item6 == key7))
					{
						continue;
					}
					text4 = item6;
					foreach (string item7 in dictionary3[key7])
					{
						string value3 = item7 + ";" + text5 + ";10;;Ohm;FAIL;Open";
						streamWriter2.WriteLine(value3);
					}
				}
				if (!(key7 != text4))
				{
					continue;
				}
				foreach (string item8 in dictionary3[key7])
				{
					string value4 = item8 + ";" + text5 + ";10;;Ohm;PASS;Open";
					streamWriter2.WriteLine(value4);
				}
			}
		}
		foreach (string item9 in list)
		{
			string key2 = item9.Split(';')[0];
			if (!dictionary.ContainsKey(key2))
			{
				dictionary[key2] = 1;
				list5.Add($"{dictionary[key2]}" + "#" + item9);
			}
			else
			{
				dictionary[key2]++;
				list5.Add($"{dictionary[key2]}" + "#" + item9);
			}
		}
		foreach (string item10 in list5)
		{
			string text6 = item10.Split('#')[1];
			string key3 = text6.Split(';')[0];
			if (dictionary[key3] > 1)
			{
				streamWriter.WriteLine("(" + item10.Split('#')[0] + ")" + text6);
			}
			else
			{
				streamWriter.WriteLine(text6);
			}
		}
		dictionary.Clear();
		list5.Clear();
		foreach (string item11 in list2)
		{
			string key4 = item11.Split(';')[0];
			if (!dictionary.ContainsKey(key4))
			{
				dictionary[key4] = 1;
				list5.Add($"{dictionary[key4]}" + "#" + item11);
			}
			else
			{
				dictionary[key4]++;
				list5.Add($"{dictionary[key4]}" + "#" + item11);
			}
		}
		foreach (string item12 in list5)
		{
			string text7 = item12.Split('#')[1];
			string key5 = text7.Split(';')[0];
			if (dictionary[key5] > 1)
			{
				streamWriter2.WriteLine("(" + item12.Split('#')[0] + ")" + text7);
			}
			else
			{
				streamWriter2.WriteLine(text7);
			}
		}
		dictionary.Clear();
		list5.Clear();
		streamWriter.Close();
		streamWriter2.Close();
		return 0;
	}

	public void process()
	{
		string text = "";
		string text2 = "";
		string text3 = "";
		Heads heads = new Heads();
		string[] array = this.heads[1].Split('_');
		string[] array2 = this.heads[3].Split('_');
		if (array.Length > 2 && array[2].Length == 23)
		{
			text2 = array[2] + "_" + array[1];
			text3 = array2[2] + "_" + array2[1];
			heads.PPID1 = "PPID: " + array[2];
			heads.PPID2 = "PPID: " + array2[2];
			heads.MODULENAME = "MODELNAME: " + array[0] + "_" + array[1];
			if (Getbom.getbom(array[2]))
			{
				text = Getbom.sku;
			}
		}
		else
		{
			text2 = this.heads[1];
			text3 = this.heads[3];
			if (Getbom.getbom(array[0]))
			{
				text = Getbom.sku;
				heads.MODULENAME = "MODELNAME: " + Getbom.model;
				heads.PPID1 = "PPID: " + Getbom.ppid1;
			}
			if (Getbom.getbom(array2[0]))
			{
				heads.PPID2 = "PPID: " + Getbom.ppid1;
			}
		}
		heads.SKU = text;
		heads.HEAD = "/*DELL Confidential Information */";
		heads.STATION = "STATION: ICT";
		heads.MACHINE_ID = "MACHINE_ID: " + Environment.MachineName;
		heads.FIXTURE_ID = "FIXTURE_ID: " + this.heads[0];
		heads.OP_ID = "OP_ID: FIX_OPID";
		heads.MODEL = "MODEL: " + text;
		heads.SERIAL_NUMBER1 = "SERIAL_NUMBER: " + this.heads[1];
		heads.SERIAL_NUMBER2 = "SERIAL_NUMBER: " + this.heads[3];
		heads.TEST_START_TIME = "TEST_START_TIME: " + this.heads[5];
		heads.TEST_END_TIME = "TEST_END_TIME: " + this.heads[6];
		heads.TEST_TIME = "TEST_CYCLE_TIME: " + this.heads[7] + "s";
		heads.TEST_RESULT1 = "TEST_RESULT: " + this.heads[2];
		heads.TEST_RESULT2 = "TEST_RESULT: " + this.heads[4];
		string[] files = Directory.GetFiles(inputpath);
		string text4 = outputpath + "\\" + DateTime.Now.ToString("yyyyMMdd") + "\\PASS\\";
		string text5 = outputpath + "\\" + DateTime.Now.ToString("yyyyMMdd") + "\\FAIL\\";
		if (!Directory.Exists(text4))
		{
			Directory.CreateDirectory(text4);
		}
		if (!Directory.Exists(text5))
		{
			Directory.CreateDirectory(text5);
		}
		string output = ((!(this.heads[2].ToUpper() == "PASS")) ? (text5 + text2 + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".log") : (text4 + text2 + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".log"));
		string output2 = ((!(this.heads[4].ToUpper() == "PASS")) ? (text5 + text3 + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".log") : (text4 + text3 + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".log"));
		if (files.Length != 1)
		{
			Thread.Sleep(1000);
			return;
		}
		string[] array3 = files;
		foreach (string text6 in array3)
		{
			FileInfo fileInfo = new FileInfo(text6);
			long length = fileInfo.Length;
			long num = 0L;
			string text7 = Path.GetExtension(text6).ToLower();
			if (fileInfo.Length <= 1 || !(text7 == ".log"))
			{
				continue;
			}
			if (length > num)
			{
				if (set == "1")
				{
					if (!(text != oldsku))
					{
						try
						{
							File.Delete(text6);
						}
						catch
						{
							Thread.Sleep(1000);
							File.Delete(text6);
						}
						continue;
					}
					proc_log(text6, output, output2, heads);
					oldsku = text;
				}
				else
				{
					proc_log(text6, output, output2, heads);
				}
			}
			try
			{
				File.Delete(text6);
			}
			catch
			{
				Thread.Sleep(1000);
				File.Delete(text6);
			}
		}
	}
}
