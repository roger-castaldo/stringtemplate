using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.Stringtemplate.Outputs
{
    public interface IOutputWriter
    {
        void Append(string text);
    }
}
