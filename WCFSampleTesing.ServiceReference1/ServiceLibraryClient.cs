using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WCFSampleTesing.ServiceReference1;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
public class ServiceLibraryClient : ClientBase<IServiceLibrary>, IServiceLibrary
{
	public ServiceLibraryClient()
	{
	}

	public ServiceLibraryClient(string endpointConfigurationName)
		: base(endpointConfigurationName)
	{
	}

	public ServiceLibraryClient(string endpointConfigurationName, string remoteAddress)
		: base(endpointConfigurationName, remoteAddress)
	{
	}

	public ServiceLibraryClient(string endpointConfigurationName, EndpointAddress remoteAddress)
		: base(endpointConfigurationName, remoteAddress)
	{
	}

	public ServiceLibraryClient(Binding binding, EndpointAddress remoteAddress)
		: base(binding, remoteAddress)
	{
	}

	public ExecutionResult CompalClientTransferEquipStrucData(ClientTransferData cTmpClientInfo)
	{
		return base.Channel.CompalClientTransferEquipStrucData(cTmpClientInfo);
	}

	public string CompalClientTransferEquipSerialData(string cTmpClientInfo)
	{
		return base.Channel.CompalClientTransferEquipSerialData(cTmpClientInfo);
	}

	public string SayHello(string cTmpStr)
	{
		return base.Channel.SayHello(cTmpStr);
	}
}
