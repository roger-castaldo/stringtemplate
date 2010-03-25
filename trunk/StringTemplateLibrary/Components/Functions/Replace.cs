using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Org.Reddragonit.Stringtemplate.Tokenizers;
using Org.Reddragonit.Stringtemplate.Interfaces;

namespace Org.Reddragonit.Stringtemplate.Components.Functions
{
    public class Replace : FunctionComponent
    {
        private static readonly Regex _reg = new Regex("^[R|r][E|e][P|p][L|l][A|a][C|c][E|e]\\((inputString\\s*=\\s*)?(.+),(searchString\\s*=\\s*)?(.+),(replaceString\\s*=\\s*)?(.+)\\)$", 
            RegexOptions.Compiled | RegexOptions.ECMAScript);

        private IComponent _eval;
        private IComponent _search;
        private IComponent _replace;

        protected override string FunctionName
        {
            get { return "replace"; }
        }

        protected override string[] FunctionParameters
        {
            get
            {
                return new string[]{
                    "inputString","searchString","replaceString"
                };
            }
        }

        public override bool Load(Queue<Token> tokens, Type tokenizerType, TemplateGroup group)
        {
            Token t = tokens.Dequeue();
            Match m = _reg.Match(t.Content);
            Tokenizer tok = (Tokenizer)tokenizerType.GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { m.Groups[2].Value });
            _eval = tok.TokenizeStream(group)[0];
            tok = (Tokenizer)tokenizerType.GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { m.Groups[4].Value });
            _search = tok.TokenizeStream(group)[0];
            tok = (Tokenizer)tokenizerType.GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { m.Groups[6].Value });
            _replace = tok.TokenizeStream(group)[0];
            return true;
        }

        public override string GenerateString(ref Dictionary<string, object> variables)
        {
            string tmp = _eval.GenerateString(ref variables);
            string search = _search.GenerateString(ref variables);
            string replace = _replace.GenerateString(ref variables);
            if ((tmp != null)&&(search!=null)&&(replace!=null))
            {
                return tmp.Replace(search, replace);
            }
            return null;
        }

        public override IComponent NewInstance()
        {
            return new Replace();
        }
    }
}
