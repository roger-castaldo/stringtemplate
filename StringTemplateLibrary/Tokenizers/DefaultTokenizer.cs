using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Stringtemplate.Interfaces;
using System.IO;

namespace Org.Reddragonit.Stringtemplate.Tokenizers
{
    public class DefaultTokenizer : Tokenizer
    {
        public DefaultTokenizer(TextReader tr) : base(tr)
        {
        }

        public DefaultTokenizer(string template) : base(template)
        {
        }

        protected override char TokenChar
        {
            get { return '$'; }
        }

        protected override char EscapeChar
        {
            get { return '\\'; }
        }
    }
}
