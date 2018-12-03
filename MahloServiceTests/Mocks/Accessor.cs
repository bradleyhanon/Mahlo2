using System;
using System.Dynamic;

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
