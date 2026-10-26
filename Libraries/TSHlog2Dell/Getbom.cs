using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Xml;

namespace TSHlog2Dell;

public class Getbom
{
	public static string model;

	public static string sku;

	public static string batcode_all;

	public static string station_check;

	public static string ppid1;

	public static bool getbom(string sn)
	{
		bool result = false;
		batcode_all = "";
		station_check = "";
		new List<string>();
		new List<string>();
		new Dictionary<string, string>();
		new Dictionary<string, string>();
		new Dictionary<string, string>();
		string text = ConfigurationManager.AppSettings["FMDI_URL"] ?? "";
		string text2 = ConfigurationManager.AppSettings["FMDI_SITE"] ?? "";
		string text3 = ConfigurationManager.AppSettings["FMDI_LINE"] ?? "";
		string text4 = text + "?sn=" + sn + "&customer= SMT&group=ATE&line=" + text3 + "&site=" + text2 + "&intl=0.0.0.11-StandardClientImpl.dll-7.0.0.0";
		Console.WriteLine(text4);
		HttpWebRequest obj = (HttpWebRequest)WebRequest.Create(text4);
		obj.Method = "GET";
		obj.Proxy = null;
		obj.UserAgent = "FMDICLIENT";
		obj.Timeout = 10000;
		obj.ReadWriteTimeout = 10000;
		WebResponse response = obj.GetResponse();
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.Load(response.GetResponseStream());
		foreach (XmlNode childNode in xmlDocument.ChildNodes)
		{
			string innerText = childNode.InnerText;
			string patten = ".*SET MODELNAME=\\s*(\\w+.*)";
			result = true;
			string patten2 = ".*SET SN=(\\d+).*";
			string patten3 = ".*SET SNMULTI=(.*);";
			string patten4 = ".*SET MAC_LAN_1=(\\w+).*";
			string patten5 = ".*SET MACMULTI_LAN_1=(.*);";
			string patten6 = ".*SET SNMACMULTI=(.*);";
			string patten7 = ".*SET SSN=(\\w+).*";
			string patten8 = ".*SET SSNMULTI=(.*);";
			string patten9 = ".*SET GROUPMULTI=(.*);";
			sku = Refun.getbomstr(".*SET SKU=(\\w+).*", innerText);
			model = Refun.getbomstr(patten, innerText);
			Refun.getbomstr(patten2, innerText);
			Refun.getbomstr(patten3, innerText);
			Refun.getbomstr(patten4, innerText);
			Refun.getbomstr(patten5, innerText);
			Refun.getbomstr(patten6, innerText);
			ppid1 = Refun.getbomstr(patten7, innerText);
			Refun.getbomstr(patten8, innerText);
			Refun.getbomstr(patten9, innerText);
		}
		return result;
	}
}
