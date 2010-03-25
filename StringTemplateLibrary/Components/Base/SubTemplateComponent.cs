using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Stringtemplate.Interfaces;
using Org.Reddragonit.Stringtemplate.Tokenizers;
using System.Text.RegularExpressions;


namespace Org.Reddragonit.Stringtemplate.Components.Base
{
    internal class SubTemplateComponent : IComponent
    {
        private string _groupName;
        private string _functionName;
        private List<IComponent> _components;
        private TemplateGroup _group;

        internal List<IComponent> Components
        {
            get { return _components; }
            set {_components = value;}
        }

        private string[] _parameters;
        private Regex _functionReg;
        private Dictionary<string, string> _parameterMap;
        private IComponent _it=null;

        public SubTemplateComponent(string GroupName,string FunctionName,List<IComponent> Components,string[] Parameters,TemplateGroup group ) {
            _groupName = GroupName;
            _functionName = FunctionName;
            _components = Components;
            _parameters = Parameters;
            string reg = "^((" + GroupName + "/)";
            if ((_parameters == null) || (_parameters.Length == 0))
                reg += "|((.+):)";
            reg+=")?"+_functionName+"\\(";
            if ((_parameters != null) && (_parameters.Length > 0))
            {
                foreach (string str in _parameters)
                {
                    reg += "(\\s*(" + str + "\\s*=\\s*)?([^\\(]+)),";
                }
                reg = reg.Substring(0, reg.Length - 1);
            }
            reg += "\\)$";
            _functionReg = new Regex(reg, RegexOptions.Compiled | RegexOptions.ECMAScript);
            _group = group;
        }



        #region IComponent Members

        public bool CanLoad(Token token)
        {
            return _functionReg.IsMatch(token.Content);
        }

        public bool Load(Queue<Token> tokens, Type tokenizerType,TemplateGroup group)
        {
            _parameterMap = new Dictionary<string, string>();
            Token t = tokens.Dequeue();
            Match m = _functionReg.Match(t.Content);
            if ((_parameters == null) || (_parameters.Length == 0))
            {
                Queue<Token> toks = new Queue<Token>();
                toks.Enqueue(new Token(m.Groups[4].Value.Trim(),TokenType.COMPONENT));
                _it = ComponentExtractor.ExtractComponent(toks, tokenizerType, group);
                if (_it is ParameterComponent)
                {
                    _it = null;
                    _parameterMap.Add("it", m.Groups[4].Value.Trim());
                }
            }
            else
            {
                int x = 5;
                foreach (string str in _parameters)
                {
                    _parameterMap.Add(str, m.Groups[x].Value.Trim());
                    x += 3;
                }
            }
            return true;
        }

        public string GenerateString(ref Dictionary<string, object> variables)
        {
            if (_components == null)
            {
                foreach (IComponent comp in _group.Components)
                {
                    if (((SubTemplateComponent)comp)._functionReg.ToString() == _functionReg.ToString())
                    {
                        _components = ((SubTemplateComponent)comp).Components;
                        break;
                    }
                }
            }
            Dictionary<string, object> vars = new Dictionary<string, object>();
            if (_it != null)
            {
                vars.Add("it", _it.GenerateString(ref variables));
            }
            else
            {
                foreach (string str in _parameterMap.Keys)
                {
                    object obj = Utility.LocateObjectInVariables(_parameterMap[str], variables);
                    if (obj != null)
                        vars.Add(str, obj);
                }
            }
            string ret = "";
            foreach (IComponent comp in _components)
            {
                ret += comp.GenerateString(ref vars);
            }
            return ret;
        }

        public IComponent NewInstance() {
            return new SubTemplateComponent(_groupName, _functionName, _components, _parameters,_group);
        }

        #endregion
    }
}
