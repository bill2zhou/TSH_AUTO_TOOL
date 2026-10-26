using System.CodeDom.Compiler;
using System.ServiceModel;

namespace WCFSampleTesing.ServiceReference1;

[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[ServiceContract(ConfigurationName = "ServiceReference1.IServiceLibrary")]
public interface IServiceLibrary
{
	[OperationContract(Action = "http://tempuri.org/IServiceLibrary/CompalClientTransferEquipStrucData", ReplyAction = "http://tempuri.org/IServiceLibrary/CompalClientTransferEquipStrucDataResponse")]
	ExecutionResult CompalClientTransferEquipStrucData(ClientTransferData cTmpClientInfo);

	[OperationContract(Action = "http://tempuri.org/IServiceLibrary/CompalClientTransferEquipSerialData", ReplyAction = "http://tempuri.org/IServiceLibrary/CompalClientTransferEquipSerialDataResponse")]
	string CompalClientTransferEquipSerialData(string cTmpClientInfo);

	[OperationContract(Action = "http://tempuri.org/IServiceLibrary/SayHello", ReplyAction = "http://tempuri.org/IServiceLibrary/SayHelloResponse")]
	string SayHello(string cTmpStr);
}
