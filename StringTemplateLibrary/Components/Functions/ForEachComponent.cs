using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Org.Reddragonit.Stringtemplate.Tokenizers;
using Org.Reddragonit.Stringtemplate.Interfaces;

using System.Collections;
using Org.Reddragonit.Stringtemplate.Outputs;

namespace Org.Reddragonit.Stringtemplate.Components.Functions
{
    public class ForEachComponent : FunctionComponent
    {
        private struct KeyValuePair
        {
            private object _key;
            public object Key
            {
                get { return _key; }
            }

            private object _value;
            public object Value
            {
                get { return _value; }
            }

            public KeyValuePair(object key, object value)
            {
                _key = key;
                _value = value;
            }
        }

        private static readonly Regex regAlternateFormat = new Regex("^([^:]+)\\s*:\\s*\\{\\s*([^|]+)\\s*\\|", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ECMAScript);
        private static readonly Regex regEndFor = new Regex("^([E|e][N|n][D|d][F|f][O|o][R|r])$", RegexOptions.Compiled | RegexOptions.ECMAScript);
        private static readonly Regex regVariableExtractor = new Regex("^([F|f][O|o][R|r][E|e][A|a][C|c][H|h])\\((.+),(.+)\\)$",RegexOptions.Compiled|RegexOptions.ECMAScript);
        private static readonly Regex regVarValueExtracor = new Regex("^([V|v][A|a][R|r])\\s*=\\s*(.+)$");
        private static readonly Regex regListValueExtracor = new Regex("^([L|l][I|i][S|s][T|t])\\s*=\\s*(.+)$");

        private string _variableName;
        private string _entryName;

        protected override string FunctionName
        {
            get { return "foreach"; }
        }

        protected override string[] FunctionParameters
        {
            get { return new string[] { "var", "list" }; }
        }

        public override bool CanLoad(Token token)
        {
        	return base.CanLoad(token)||(regAlternateFormat.IsMatch(token.Content)&&token.Content.EndsWith("}"));
        }

        public override bool Load(Queue<Token> tokens, Type tokenizerType,TemplateGroup group)
        {
            Token t = tokens.Dequeue();
            Match m;
            if (regAlternateFormat.IsMatch(t.Content))
            {
                m = regAlternateFormat.Match(t.Content);
                _variableName = m.Groups[1].Value;
                _entryName = m.Groups[2].Value;
                string tmp = t.Content.Substring(t.Content.IndexOf("|", m.Groups[2].Index) + 1);
                tmp = tmp.Substring(0, tmp.Length - 1);
                Tokenizer tok = (Tokenizer)tokenizerType.GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { tmp});
                _children = tok.TokenizeStream(group);
            }
            else
            {
                m = regVariableExtractor.Match(t.Content);
                _entryName = m.Groups[1].Value;
                _variableName = m.Groups[2].Value;
                if (regVarValueExtracor.IsMatch(_entryName))
                    _entryName = regVarValueExtracor.Match(_entryName).Groups[2].Value;
                if (regListValueExtracor.IsMatch(_variableName))
                    _variableName = regListValueExtracor.Match(_variableName).Groups[2].Value;
                while ((tokens.Count>0)&&!regEndFor.IsMatch(tokens.Peek().Content))
                    _children.Add(ComponentExtractor.ExtractComponent(tokens, tokenizerType, group));
                if (tokens.Count > 0)
                    tokens.Dequeue();
            }
            _entryName = _entryName.Trim();
            _variableName = _variableName.Trim();
            return true;
        }

        public override void  Append(ref Dictionary<string,object> variables, IOutputWriter writer)
        {
            if (variables.ContainsKey(_entryName))
                throw new Exception("Unable to process foreach loop because variable name " + _entryName + " already in use");
            object oldi = null;
            int i = 0;
            object obj = Utility.LocateObjectInVariables(_variableName, variables);
            if (obj == null)
                return;
            if (!(obj is ArrayList) && !(obj.GetType().IsArray) && !(obj is IEnumerable) &&!(obj is IDictionary))
                throw new Exception("Unable to process foreach loop because variable " + _variableName + " is not an iterable object");
            string ret = "";
            if (variables.ContainsKey("i"))
                oldi = variables["i"];
            if (obj is ArrayList) {
                foreach (object o in (ArrayList)obj)
                {
                    variables.Remove(_entryName);
                    variables.Remove("i");
                    variables.Add(_entryName, o);
                    variables.Add("i", i);
                    foreach (IComponent comp in _children)
                    {
                        comp.Append(ref variables,writer);
                    }
                    i++;
                }
            }
            else if (obj is IDictionary)
            {
                IDictionaryEnumerator e = ((IDictionary)obj).GetEnumerator();
                while (e.MoveNext())
                {
                    variables.Remove(_entryName);
                    variables.Remove("i");
                    variables.Add(_entryName, new KeyValuePair(e.Key,e.Value));
                    variables.Add("i", i);
                    foreach (IComponent comp in _children)
                    {
                        comp.Append(ref variables,writer);
                    }
                    i++;
                }
            }
            else
            {
                foreach (object o in (IEnumerable)obj)
                {
                    variables.Remove(_entryName);
                    variables.Remove("i");
                    variables.Add(_entryName, o);
                    variables.Add("i", i);
                    foreach (IComponent comp in _children)
                    {
                        comp.Append(ref variables,writer);
                    }
                    i++;
                }
            }
            variables.Remove("i");
            variables.Remove(_entryName);
            if (oldi != null)
                variables.Add("i", oldi);
        }

        public override IComponent NewInstance() {
            return new ForEachComponent();
        }
    }
}
