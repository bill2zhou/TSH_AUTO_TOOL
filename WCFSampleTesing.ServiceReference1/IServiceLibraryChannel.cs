using System;
using System.CodeDom.Compiler;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WCFSampleTesing.ServiceReference1;

[GeneratedCode("System.ServiceModel", "4.0.0.0")]
public interface IServiceLibraryChannel : IServiceLibrary, IClientChannel, IContextChannel, IChannel, ICommunicationObject, IExtensibleObject<IContextChannel>, IDisposable
{
}
