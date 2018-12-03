using System;
using System.Collections.Generic;

namespace MahloService.Opc
{
  class EasyDAClientSimulator : IEasyDAClient
  {
    public IEnumerable<Range<int>> HandleRanges => throw new NotImplementedException();

    public OpcTechnologies SupportedTechnologies => throw new NotImplementedException();

    public event EasyDAItemChangedEventHandler ItemChanged;

    public string[] BrowseAccessPaths([NotNull] ServerDescriptor serverDescriptor, [NotNull] DANodeDescriptor nodeDescriptor)
    {
      throw new NotImplementedException();
    }

    public DANodeElementCollection BrowseNodes([NotNull] ServerDescriptor serverDescriptor, [NotNull] DANodeDescriptor parentNodeDescriptor, [NotNull] DABrowseParameters browseParameters)
    {
      throw new NotImplementedException();
    }

    public DAPropertyElementCollection BrowseProperties([NotNull] ServerDescriptor serverDescriptor, [NotNull] DANodeDescriptor nodeDescriptor)
    {
      throw new NotImplementedException();
    }

    public ServerElementCollection BrowseServers([NotNull] string location, OpcTechnologies technologies)
    {
      throw new NotImplementedException();
    }

    public void ChangeMultipleItemSubscriptions([NotNull] DAHandleGroupArguments[] argumentsArray)
    {
      throw new NotImplementedException();
    }

    public ValueResult[] GetMultiplePropertyValues([NotNull] DAPropertyArguments[] argumentsArray)
    {
      throw new NotImplementedException();
    }

    public bool IsKnownItemSubscriptionHandle(int handle)
    {
      throw new NotImplementedException();
    }

    public DAVtqResult[] ReadMultipleItems([NotNull] DAReadItemArguments[] argumentsArray)
    {
      throw new NotImplementedException();
    }

    public ValueResult[] ReadMultipleItemValues([NotNull] DAReadItemArguments[] argumentsArray)
    {
      throw new NotImplementedException();
    }

    public int[] SubscribeMultipleItems([NotNull] EasyDAItemSubscriptionArguments[] itemSubscriptionArgumentsArray)
    {
      throw new NotImplementedException();
    }

    public void UnsubscribeAllItems()
    {
      throw new NotImplementedException();
    }

    public void UnsubscribeMultipleItems([NotNull] IEnumerable<int> handlesToUnsubscribe)
    {
      throw new NotImplementedException();
    }

    public OperationResult[] WriteMultipleItems([NotNull] DAItemVtqArguments[] argumentsArray)
    {
      throw new NotImplementedException();
    }

    public OperationResult[] WriteMultipleItemValues([NotNull] DAItemValueArguments[] argumentsArray)
    {
      throw new NotImplementedException();
    }
  }
}
