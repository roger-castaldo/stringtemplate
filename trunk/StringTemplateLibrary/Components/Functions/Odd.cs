using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Stringtemplate.Tokenizers;
using Org.Reddragonit.Stringtemplate.Interfaces;
using Org.Reddragonit.Stringtemplate.Components.Base;
using System.Text.RegularExpressions;

namespace Org.Reddragonit.Stringtemplate.Components.Functions
{
    public class Odd : FunctionComponent 
    {
        private static readonly Regex _reg = new Regex("^[O|o][D|d][D|d]\\(([V|v][A|a][L|l]\\s*=\\s*)?(.+)\\)$",RegexOptions.Compiled|RegexOptions.ECMAScript);

        private IComponent _val;

        protected override string FunctionName
        {
            get { return "odd"; }
        }

        protected override string[] FunctionParameters
        {
            get { return new string[]{"val"}; }
        }

        public override bool Load(Queue<Token> tokens, Type tokenizerType, TemplateGroup group)
        {
            Token t = tokens.Dequeue();
            string tmp = _reg.Match(t.Content).Groups[2].ToString();
            Tokenizer tok = (Tokenizer)tokenizerType.GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { tmp });
            _val = tok.TokenizeStream(group)[0];
            return true;
        }

        public override string GenerateString(ref Dictionary<string, object> variables)
        {
            try
            {

                Decimal d = Decimal.Parse(_val.GenerateString(ref variables));
                if (d % 2 == 0)
                    return false.ToString();
            }
            catch (Exception e)
            {
                if (_val is GenericComponent)
                {
                    string str = Utility.GenerateStringFromObject(Utility.LocateObjectInVariables(_val.GenerateString(ref variables), variables));
                    if (str != null)
                    {
                        try
                        {
                            decimal d = Decimal.Parse(str);
                            if (d % 2 == 0)
                                return false.ToString();
                        }
                        catch (Exception ex)
                        {
                            return false.ToString();
                        }
                    }
                    else
                        return false.ToString();
                }else
                    return false.ToString();
            }
            return true.ToString();
        }

        public override IComponent NewInstance()
        {
            return new Odd();
        }
    }
}
