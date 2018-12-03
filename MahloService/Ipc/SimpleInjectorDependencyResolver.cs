using System;
using System.Collections.Generic;
using System.Linq;

namespace Mahlo.Ipc
{
  class SimpleInjectorDependencyResolver : DefaultDependencyResolver
  {
    private readonly Container container;

    public SimpleInjectorDependencyResolver(Container container)
    {
      this.container = container;
    }

    public override object GetService(Type serviceType)
    {
      return ((IServiceProvider)container).GetService(serviceType) ??
        base.GetService(serviceType);
    }

    public override IEnumerable<object> GetServices(Type serviceType)
    {
      return this.container.GetAllInstances(serviceType).Concat(base.GetServices(serviceType));
    }
  }
}
