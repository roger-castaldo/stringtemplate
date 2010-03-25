/*
 * Created by SharpDevelop.
 * User: Roger
 * Date: 01/12/2008
 * Time: 4:30 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.IO;
using Org.Reddragonit.Stringtemplate.Interfaces;
using System.Text.RegularExpressions;

using Org.Reddragonit.Stringtemplate.Tokenizers;

namespace Org.Reddragonit.Stringtemplate.Components.Base
{
	/// <summary>
	/// Description of VariableComponent.
	/// </summary>
	public class VariableComponent : IComponent 
	{
        private static readonly Regex _reg = new Regex("DECLARE\\s+(.+)\\s+as\\s+(.+)\\s+=\\s+(.+)", RegexOptions.Compiled | RegexOptions.ECMAScript);

		public VariableComponent()
		{
		}
		
		//SAMPLE: DELCARE x as Integer = 0;
		//SAMPLE: DECLARE names as String[] = {"bob","fred","jack"}
		//SAMPLE: DECLARE fred as Org.Sample.Name = NEW Name("bob","loblaw")
		//SAMPLE: DECLARE fred as Org.Sample.Name = Name.Construct("bob","loblaw")
		
		private string _variableName;
		private Type _variableType;
		private bool _constructor=false;
		private bool _directSet = false;
		private bool _staticCall = false;
		private bool _isArray=false;
        private object _val=null;

        public bool CanLoad(Org.Reddragonit.Stringtemplate.Tokenizers.Token token)
        {
            return _reg.IsMatch(token.Content);
        }

        public bool Load(Queue<Token> tokens, Type tokenizerType,TemplateGroup group)
        {
            Match m = _reg.Match(tokens.Dequeue().Content);
            _variableName = m.Groups[0].Value;
            _isArray = m.Groups[1].Value.Contains("[]");
            if (_isArray)
                _variableType = Utility.LocateType(m.Groups[1].Value.Replace("[]", ""));
            else
                _variableType = Utility.LocateType(m.Groups[1].Value);
            if (_variableType == null)
                return false;
            _constructor = m.Groups[2].Value.ToUpper().StartsWith("NEW");
            if (!_constructor)
            {
                if (_isArray)
                    _directSet = m.Groups[2].Value.StartsWith("{");
                _staticCall = m.Groups[2].Value.Contains("(") && ((m.Groups[2].Value.IndexOf("\"") > m.Groups[2].Value.IndexOf("(")) || (!m.Groups[2].Value.Contains("\"") && m.Groups[2].Value.Contains(")")));
            }
            if (!_directSet && !_isArray)
            {
                _directSet = !_constructor && !_staticCall;
            }
            if (!_directSet && !_constructor && !_staticCall)
                return false;
            try
            {
                if (_directSet)
                {
                    if (_isArray)
                    {
                        if (_variableType.Equals(typeof(string)))
                        {

                        }
                    }
                    else
                    {
                        
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public string GenerateString(ref Dictionary<string, object> variables)
        {
            throw new NotImplementedException();
        }

        #region IComponent Members


        public IComponent NewInstance()
        {
            return new VariableComponent();
        }

        #endregion
    }
}
