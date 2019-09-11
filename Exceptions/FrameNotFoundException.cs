using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Core.Exceptions
{
    public class FrameNotFoundException : Exception
    {
        public FrameNotFoundException(string message): base(message)
        {

        }
    }
}
