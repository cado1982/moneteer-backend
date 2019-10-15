using System;
using System.Collections.Generic;
using System.Text;

namespace Moneteer.Domain.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message)
        {
        }
    }
}
