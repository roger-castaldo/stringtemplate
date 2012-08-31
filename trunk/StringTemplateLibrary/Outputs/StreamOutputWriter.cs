using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Org.Reddragonit.Stringtemplate.Outputs
{
    internal class StreamOutputWriter : IOutputWriter
    {
        private StreamOutputWriter _writer;
        public StreamOutputWriter(Stream ostream)
        {
            _writer =new StreamOutputWriter(ostream);
        }

        public void Flush()
        {
            _writer.Flush();
        }

        #region IOutputWriter Members

        public void Append(string text)
        {
            _writer.Append(text);
        }

        #endregion
    }
}
