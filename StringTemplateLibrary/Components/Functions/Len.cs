using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Org.Reddragonit.Stringtemplate.Tokenizers;
using Org.Reddragonit.Stringtemplate.Outputs;
using Org.Reddragonit.Stringtemplate.Interfaces;
using Org.Reddragonit.Stringtemplate.Components.Base;
using System.Collections;

namespace Org.Reddragonit.Stringtemplate.Components.Functions
{
    public class Len : FunctionComponent
    {
        private static readonly Regex _reg = new Regex("^[L|l][E|e][N|n]\\(([V|v][A|a][L|l]\\s*=\\s*)?(.+)\\)$", RegexOptions.Compiled | RegexOptions.ECMAScript);

        private IComponent _val;

        protected override string FunctionName
        {
            get { return "len"; }
        }

        protected override string[] FunctionParameters
        {
            get { return new string[] { "val" }; }
        }

        public override bool Load(Queue<Token> tokens, Type tokenizerType, TemplateGroup group)
        {
            Token t = tokens.Dequeue();
            string tmp = _reg.Match(t.Content).Groups[2].ToString();
            Tokenizer tok = (Tokenizer)tokenizerType.GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { tmp });
            _val = tok.TokenizeStream(group)[0];
            return true;
        }

        public override void Append(ref Dictionary<string, object> variables, IOutputWriter writer)
        {
            StringOutputWriter swo = new StringOutputWriter();
            object obj = null;
            bool found = false;
            if (_val is ParameterComponent)
                obj = ((ParameterComponent)_val).GetObjectValue(variables);
            else
            {
                found = true;
                swo.Clear();
                _val.Append(ref variables, swo);
                if (_val is GenericComponent)
                {
                    obj = Utility.LocateObjectInVariables(swo.ToString(), variables);
                    if (obj == null)
                        writer.Append(swo.ToString().Length.ToString());
                }
                else
                    writer.Append(swo.ToString().Length.ToString());
            }
            if (obj != null)
            {
                if (obj is ArrayList)
                    writer.Append(((ArrayList)obj).Count.ToString());
                else if (obj is IDictionary)
                    writer.Append(((IDictionary)obj).Count.ToString());
                else if (obj is string)
                    writer.Append(((string)obj).Length.ToString());
                else
                {
                    int cnt = 0;
                    foreach (object o in (IEnumerable)obj)
                        cnt++;
                    writer.Append(cnt.ToString());
                }
            }else if (!found)
                writer.Append("-1");
        }

        public override IComponent NewInstance()
        {
            return new Len();
        }
    }
}
