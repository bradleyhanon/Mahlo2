using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahloService
{
  [Serializable]
  public class SingleInstanceException : Exception
  {
    public SingleInstanceException(string message)
      : base(message)
    {
    }

    protected SingleInstanceException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
    }

    public SingleInstanceException()
      : base()
    {
    }

    public SingleInstanceException(string message, Exception innerException) 
      : base(message, innerException)
    {
    }
  }
}
