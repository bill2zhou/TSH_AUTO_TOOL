using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace WCFSampleTesing.ServiceReference1;

[Serializable]
[DebuggerStepThrough]
[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
[DataContract(Name = "SingleLogItemDef", Namespace = "http://schemas.datacontract.org/2004/07/EquipELinkServiceLibrary")]
public class SingleLogItemDef : IExtensibleDataObject, INotifyPropertyChanged
{
	[NonSerialized]
	private ExtensionDataObject extensionDataField;

	[OptionalField]
	private string KeyStrField;

	[OptionalField]
	private string KeyValueField;

	[Browsable(false)]
	public ExtensionDataObject ExtensionData
	{
		get
		{
			return extensionDataField;
		}
		set
		{
			extensionDataField = value;
		}
	}

	[DataMember]
	public string KeyStr
	{
		get
		{
			return KeyStrField;
		}
		set
		{
			if ((object)KeyStrField != value)
			{
				KeyStrField = value;
				RaisePropertyChanged("KeyStr");
			}
		}
	}

	[DataMember]
	public string KeyValue
	{
		get
		{
			return KeyValueField;
		}
		set
		{
			if ((object)KeyValueField != value)
			{
				KeyValueField = value;
				RaisePropertyChanged("KeyValue");
			}
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	protected void RaisePropertyChanged(string propertyName)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
