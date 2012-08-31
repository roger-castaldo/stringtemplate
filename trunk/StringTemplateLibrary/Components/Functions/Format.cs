using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Org.Reddragonit.Stringtemplate.Tokenizers;
using Org.Reddragonit.Stringtemplate.Interfaces;

namespace Org.Reddragonit.Stringtemplate.Components.Functions
{
    public class Format : FunctionComponent
    {

        private static readonly Regex _reg = new Regex("^[F|f][O|o][R|r][M|m][A|a][T|t]\\((variable\\s*=\\s*)?([^,]+),(formatString\\s*=\\s*)?([\"']?.+[\"']?)\\)$",
            RegexOptions.Compiled | RegexOptions.ECMAScript);

        private string _variable;
        private string _format;

        protected override string FunctionName
        {
            get { return "format"; }
        }

        protected override string[] FunctionParameters
        {
            get { return new string[] { "variable", "formatString" }; }
        }

        public override bool Load(Queue<Org.Reddragonit.Stringtemplate.Tokenizers.Token> tokens, Type tokenizerType, TemplateGroup group)
        {
            Token t = tokens.Dequeue();
            Match m = _reg.Match(t.Content);
            _variable = m.Groups[2].Value;
            _format = m.Groups[4].Value;
            if (_format.StartsWith("\"") || _format.StartsWith("'"))
                _format = _format.Substring(1);
            if (_format.EndsWith("\"") || _format.EndsWith("'"))
                _format = _format.Substring(0, _format.Length - 1);
            return true;
        }

        public override void  Append(ref Dictionary<string,object> variables, Org.Reddragonit.Stringtemplate.Outputs.IOutputWriter writer)
        {
            object obj = Utility.LocateObjectInVariables(_variable, variables);
            if (obj != null)
                writer.Append(((IFormattable)Utility.LocateObjectInVariables(_variable, variables)).ToString(_format, null));
        }

        public override Org.Reddragonit.Stringtemplate.Interfaces.IComponent NewInstance()
        {
            return new Format();
        }
    }
}
