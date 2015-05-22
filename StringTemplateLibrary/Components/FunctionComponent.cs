using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Stringtemplate.Interfaces;
using System.Text.RegularExpressions;
using Org.Reddragonit.Stringtemplate.Tokenizers;
using Org.Reddragonit.Stringtemplate.Outputs;

namespace Org.Reddragonit.Stringtemplate.Components
{
    public abstract class FunctionComponent : IParentComponent
    {
        protected abstract string FunctionName{get;}
        protected abstract string[] FunctionParameters { get; }
        protected List<IComponent> _children;

        protected object _LocateObjectInVariables(string variableName, Dictionary<string, object> variables)
        {
            return Utility.LocateObjectInVariables(variableName, variables);
        }

        private Regex regBaseFunctionCall;
        private Regex regComplexFunctionCall;
        private Regex regAttachedVarCall;

        public FunctionComponent()
        {
            if (FunctionParameters == null)
            {
                regBaseFunctionCall = new Regex("^"+FormatFunctionName(FunctionName)+"\\(\\)$", RegexOptions.Compiled | RegexOptions.ECMAScript);
                regAttachedVarCall = new Regex("^.+\\." + FormatFunctionName(FunctionName) + "\\(\\)$", RegexOptions.Compiled | RegexOptions.ECMAScript);
                regComplexFunctionCall = regBaseFunctionCall;
            }
            else
            {
                if (FunctionParameters.Length == 1)
                    regBaseFunctionCall = new Regex("^" + FormatFunctionName(FunctionName) + "\\((.+)\\)$", RegexOptions.Compiled | RegexOptions.ECMAScript);
                else
                    regBaseFunctionCall = new Regex("^" + FormatFunctionName(FunctionName) + "\\(((.+),){" + (FunctionParameters.Length - 1).ToString() + "}(.+)\\)$", RegexOptions.Compiled | RegexOptions.ECMAScript);
                regAttachedVarCall = new Regex("^.+\\." + FormatFunctionName(FunctionName) + "\\(\\)$");
                string tmp = "";
                foreach (string str in FunctionParameters)
                {
                    tmp += FormatFunctionName(str) + "\\s*=\\s*(.+),";
                }
                tmp = tmp.Substring(0, tmp.Length - 1);
                regComplexFunctionCall = new Regex("^" + FormatFunctionName(FunctionName) + "\\(" + tmp + "\\)$", RegexOptions.Compiled | RegexOptions.ECMAScript);
            }
            _children = new List<IComponent>();
        }

        internal static string FormatFunctionName(string name)
        {
            string ret = "(";
            foreach (char c in name.ToCharArray())
            {
                ret += "[" + c.ToString().ToUpper() + "|" + c.ToString() + "]";
            }
            return ret+")";
        }

        #region IParentComponent Members

        public List<IComponent> Children
        {
            get { return _children; }
        }

        #endregion

        #region IComponent Members



        public virtual bool CanLoad(Org.Reddragonit.Stringtemplate.Tokenizers.Token token)
        {
            return regBaseFunctionCall.IsMatch(token.Content) || regAttachedVarCall.IsMatch(token.Content) || regComplexFunctionCall.IsMatch(token.Content);
        }

        public abstract bool Load(Queue<Token> tokens, Type tokenizerType,TemplateGroup group);
        public abstract void Append(ref Dictionary<string, object> variables, IOutputWriter writer);
        public abstract IComponent NewInstance();

        #endregion
    }
}
