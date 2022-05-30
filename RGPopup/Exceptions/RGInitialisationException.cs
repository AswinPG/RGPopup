using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RGPopup.Exceptions;

public class RGInitialisationException : Exception
{
    public RGInitialisationException()
    {
    }

    public RGInitialisationException(string message) : base(message)
    {
    }

    public RGInitialisationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected RGInitialisationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
