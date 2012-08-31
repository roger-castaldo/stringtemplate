using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Org.Reddragonit.Stringtemplate.Tokenizers;
using Org.Reddragonit.Stringtemplate.Interfaces;
using Org.Reddragonit.Stringtemplate.Outputs;

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

        public override void Append(ref Dictionary<string, object> variables, Org.Reddragonit.Stringtemplate.Outputs.IOutputWriter writer)
        {
            StringOutputWriter swo = new StringOutputWriter();
            _eval.Append(ref variables,swo);
            string tmp = swo.ToString();
            swo.Clear();
            _search.Append(ref variables, swo);
            string search = swo.ToString();
            swo.Clear();
            _replace.Append(ref variables, swo);
            string replace = swo.ToString(); 
            if ((tmp != null)&&(search!=null)&&(replace!=null))
            {
                writer.Append(tmp.Replace(search, replace));
            }
        }

        public override IComponent NewInstance()
        {
            return new Replace();
        }
    }
}
