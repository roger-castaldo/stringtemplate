/*
 * Created by SharpDevelop.
 * User: Roger
 * Date: 26/01/2010
 * Time: 10:51 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using Org.Reddragonit.Stringtemplate.Interfaces;
using System.Reflection;

using Org.Reddragonit.Stringtemplate.Tokenizers;

namespace Org.Reddragonit.Stringtemplate.Components.Base
{
	/// <summary>
	/// Description of ParameterComponent.
	/// </summary>
	public class ParameterComponent : IComponent
	{
		private static readonly char[] _reserved = new char[]{
			'{','}','[',']','\\','(',')'
		};
		
		private string _variableName;
		private string _seperator = null;
		
		public ParameterComponent()
		{
		}
		
		public bool CanLoad(Org.Reddragonit.Stringtemplate.Tokenizers.Token token)
		{
			foreach(char c in _reserved)
			{
				if (token.Content.Contains(c.ToString()))
					return false;
			}
			return true;
		}

        public bool Load(Queue<Token> tokens, Type tokenizerType,TemplateGroup group)
		{
			_variableName = tokens.Dequeue().Content;
			if (_variableName.Contains(";"))
			{
				_seperator = _variableName.Substring(_variableName.IndexOf(";")+1);
				_variableName = _variableName.Substring(0,_variableName.IndexOf(";"));
				_seperator = _seperator.Substring(_seperator.IndexOf("\"")+1);
				_seperator = _seperator.Substring(0,_seperator.IndexOf("\""));
			}
			return true;
		}
		
		public string GenerateString(ref Dictionary<string, object> variables)
		{
            if (_variableName.StartsWith("!"))
            {
                bool ret = !Utility.IsComponentTrue(Utility.GenerateStringFromObject(Utility.LocateObjectInVariables(_variableName.Substring(1), variables), _seperator));
                return ret.ToString();
            }
            else
                return Utility.GenerateStringFromObject(Utility.LocateObjectInVariables(_variableName, variables), _seperator);
		}

        public IComponent NewInstance()
        {
            return new ParameterComponent();
        }
	}
}
