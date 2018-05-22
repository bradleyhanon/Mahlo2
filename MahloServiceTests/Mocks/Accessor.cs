using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mahlo2Tests.Mocks
{
  class DynamicAccessor : DynamicObject
  {
    private PrivateObject privateObject;

    public DynamicAccessor(object d)
    {
      this.privateObject = new PrivateObject(d);
    }

    public DynamicAccessor(object d, Type t)
    {
      this.privateObject = new PrivateObject(d, new PrivateType(t));
    }

    public override bool TryInvokeMember(InvokeMemberBinder binder,
                                          object[] args,
                                          out object result)
    {
      try
      {
        result = privateObject.Invoke(binder.Name, args);
        return true;
      }
      catch (MissingMethodException)
      {
        result = null;
        return false;
      }
    }
  }
}
