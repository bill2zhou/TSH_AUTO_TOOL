using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace WCFSampleTesing.ServiceReference1;

[Serializable]
[DebuggerStepThrough]
[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
[DataContract(Name = "ClientTransferData", Namespace = "http://schemas.datacontract.org/2004/07/EquipELinkServiceLibrary")]
public class ClientTransferData : IExtensibleDataObject, INotifyPropertyChanged
{
	[NonSerialized]
	private ExtensionDataObject extensionDataField;

	[OptionalField]
	private string CustCodeField;

	[OptionalField]
	private string EquipTypeField;

	[OptionalField]
	private string FactorySNField;

	[OptionalField]
	private string GroupNameField;

	[OptionalField]
	private string LineNameField;

	[OptionalField]
	private SingleLogItemDef[] LogFieldListField;

	[OptionalField]
	private string LogFileNameField;

	[OptionalField]
	private string MountSideField;

	[OptionalField]
	private string OpeUserField;

	[OptionalField]
	private string ProcessField;

	[OptionalField]
	private string SapPlantField;

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
	public string CustCode
	{
		get
		{
			return CustCodeField;
		}
		set
		{
			if ((object)CustCodeField != value)
			{
				CustCodeField = value;
				RaisePropertyChanged("CustCode");
			}
		}
	}

	[DataMember]
	public string EquipType
	{
		get
		{
			return EquipTypeField;
		}
		set
		{
			if ((object)EquipTypeField != value)
			{
				EquipTypeField = value;
				RaisePropertyChanged("EquipType");
			}
		}
	}

	[DataMember]
	public string FactorySN
	{
		get
		{
			return FactorySNField;
		}
		set
		{
			if ((object)FactorySNField != value)
			{
				FactorySNField = value;
				RaisePropertyChanged("FactorySN");
			}
		}
	}

	[DataMember]
	public string GroupName
	{
		get
		{
			return GroupNameField;
		}
		set
		{
			if ((object)GroupNameField != value)
			{
				GroupNameField = value;
				RaisePropertyChanged("GroupName");
			}
		}
	}

	[DataMember]
	public string LineName
	{
		get
		{
			return LineNameField;
		}
		set
		{
			if ((object)LineNameField != value)
			{
				LineNameField = value;
				RaisePropertyChanged("LineName");
			}
		}
	}

	[DataMember]
	public SingleLogItemDef[] LogFieldList
	{
		get
		{
			return LogFieldListField;
		}
		set
		{
			if (LogFieldListField != value)
			{
				LogFieldListField = value;
				RaisePropertyChanged("LogFieldList");
			}
		}
	}

	[DataMember]
	public string LogFileName
	{
		get
		{
			return LogFileNameField;
		}
		set
		{
			if ((object)LogFileNameField != value)
			{
				LogFileNameField = value;
				RaisePropertyChanged("LogFileName");
			}
		}
	}

	[DataMember]
	public string MountSide
	{
		get
		{
			return MountSideField;
		}
		set
		{
			if ((object)MountSideField != value)
			{
				MountSideField = value;
				RaisePropertyChanged("MountSide");
			}
		}
	}

	[DataMember]
	public string OpeUser
	{
		get
		{
			return OpeUserField;
		}
		set
		{
			if ((object)OpeUserField != value)
			{
				OpeUserField = value;
				RaisePropertyChanged("OpeUser");
			}
		}
	}

	[DataMember]
	public string Process
	{
		get
		{
			return ProcessField;
		}
		set
		{
			if ((object)ProcessField != value)
			{
				ProcessField = value;
				RaisePropertyChanged("Process");
			}
		}
	}

	[DataMember]
	public string SapPlant
	{
		get
		{
			return SapPlantField;
		}
		set
		{
			if ((object)SapPlantField != value)
			{
				SapPlantField = value;
				RaisePropertyChanged("SapPlant");
			}
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	protected void RaisePropertyChanged(string propertyName)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
