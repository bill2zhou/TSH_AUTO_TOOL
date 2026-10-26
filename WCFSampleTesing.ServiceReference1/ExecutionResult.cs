using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace WCFSampleTesing.ServiceReference1;

[Serializable]
[DebuggerStepThrough]
[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
[DataContract(Name = "ExecutionResult", Namespace = "http://schemas.datacontract.org/2004/07/EquipELinkServiceLibrary")]
[KnownType(typeof(ClientTransferData))]
[KnownType(typeof(SingleLogItemDef[]))]
[KnownType(typeof(SingleLogItemDef))]
[KnownType(typeof(object[]))]
public class ExecutionResult : IExtensibleDataObject, INotifyPropertyChanged
{
	[NonSerialized]
	private ExtensionDataObject extensionDataField;

	[OptionalField]
	private object AnythingField;

	[OptionalField]
	private object[] ArgesField;

	[OptionalField]
	private string MSGCodeField;

	[OptionalField]
	private string MessageField;

	[OptionalField]
	private bool StatusField;

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
	public object Anything
	{
		get
		{
			return AnythingField;
		}
		set
		{
			if (AnythingField != value)
			{
				AnythingField = value;
				RaisePropertyChanged("Anything");
			}
		}
	}

	[DataMember]
	public object[] Arges
	{
		get
		{
			return ArgesField;
		}
		set
		{
			if (ArgesField != value)
			{
				ArgesField = value;
				RaisePropertyChanged("Arges");
			}
		}
	}

	[DataMember]
	public string MSGCode
	{
		get
		{
			return MSGCodeField;
		}
		set
		{
			if ((object)MSGCodeField != value)
			{
				MSGCodeField = value;
				RaisePropertyChanged("MSGCode");
			}
		}
	}

	[DataMember]
	public string Message
	{
		get
		{
			return MessageField;
		}
		set
		{
			if ((object)MessageField != value)
			{
				MessageField = value;
				RaisePropertyChanged("Message");
			}
		}
	}

	[DataMember]
	public bool Status
	{
		get
		{
			return StatusField;
		}
		set
		{
			if (!StatusField.Equals(value))
			{
				StatusField = value;
				RaisePropertyChanged("Status");
			}
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	protected void RaisePropertyChanged(string propertyName)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
