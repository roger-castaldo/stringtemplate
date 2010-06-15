using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Org.Reddragonit.Stringtemplate.Tokenizers;

namespace Org.Reddragonit.Stringtemplate.Components.Functions
{
    public class TypeName : FunctionComponent
    {
        private static readonly Regex _reg = new Regex("^[T|t][Y|y][P|p][E|e][N|n][A|a][M|m][E|e]\\(([V|v][A|a][R|r]\\s*=\\s*)?(.+)\\)$", RegexOptions.Compiled | RegexOptions.ECMAScript);

        protected override string FunctionName
        {
            get { return "TypeName"; }
        }

        protected override string[] FunctionParameters
        {
            get { return new string[] { "var" }; }
        }

        private string val;

        public override bool Load(Queue<Org.Reddragonit.Stringtemplate.Tokenizers.Token> tokens, Type tokenizerType, TemplateGroup group)
        {
            Token t = tokens.Dequeue();
            val = _reg.Match(t.Content).Groups[2].ToString();
            return true;
        }

        public override string GenerateString(ref Dictionary<string, object> variables)
        {
            object obj = Utility.LocateObjectInVariables(val, variables);
            if (obj == null)
                throw new Exception("Unable to locate a variable of the name " + val + " in order to extract the type name");
            return obj.GetType().FullName;
        }

        public override Org.Reddragonit.Stringtemplate.Interfaces.IComponent NewInstance()
        {
            return new TypeName();
        }
    }
}
