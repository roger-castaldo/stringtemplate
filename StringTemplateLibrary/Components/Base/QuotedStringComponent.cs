using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Stringtemplate.Interfaces;
using System.Text.RegularExpressions;
using Org.Reddragonit.Stringtemplate.Tokenizers;
using Org.Reddragonit.Stringtemplate.Outputs;

namespace Org.Reddragonit.Stringtemplate.Components.Base
{
    public class QuotedStringComponent : IComponent
    {
        private static Regex regMatch = new Regex("^\".*\"$", RegexOptions.Compiled | RegexOptions.ECMAScript);

        private string _val;

        #region IComponent Members

        public bool CanLoad(Org.Reddragonit.Stringtemplate.Tokenizers.Token token)
        {
            return regMatch.IsMatch(token.Content);
        }

        public bool Load(Queue<Token> tokens, Type tokenizerType,TemplateGroup group)
        {
            _val = tokens.Dequeue().Content;
            _val = _val.Substring(1);
            _val = _val.Substring(0, _val.Length);
            return true;
        }

        public void Append(ref Dictionary<string, object> variables, IOutputWriter writer)
        {
            writer.Append(_val);
        }

        public IComponent NewInstance() {
            return new QuotedStringComponent();
        }

        #endregion
    }
}
