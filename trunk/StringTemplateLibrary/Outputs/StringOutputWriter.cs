using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.Stringtemplate.Outputs
{
    internal class StringOutputWriter : IOutputWriter
    {
        private string _buffer;
        public StringOutputWriter()
        {
            _buffer = null;
        }

        public void Clear()
        {
            _buffer = null;
        }

        public override string ToString()
        {
            return _buffer;
        }

        #region IOutputWriter Members

        public void Append(string text)
        {
            _buffer = (_buffer == null ? "" : _buffer) + text;
        }

        #endregion
    }
}
