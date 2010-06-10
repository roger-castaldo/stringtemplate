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

        private static readonly Regex _reg = new Regex("^[F|f][O|o][R|r][M|m][A|a][T|t]\\((variable\\s*=\\s*)?(.+),(formatString\\s*=\\s*)?(.+)\\)$",
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
            return true;
        }

        public override string GenerateString(ref Dictionary<string, object> variables)
        {
            object obj = Utility.LocateObjectInVariables(_variable, variables);
            if (obj == null)
                return null;
            return ((IFormattable)Utility.LocateObjectInVariables(_variable, variables)).ToString(_format, null);
        }

        public override Org.Reddragonit.Stringtemplate.Interfaces.IComponent NewInstance()
        {
            return new Format();
        }
    }
}
