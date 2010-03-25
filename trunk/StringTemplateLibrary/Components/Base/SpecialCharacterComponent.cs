/*
 * Created by SharpDevelop.
 * User: Roger
 * Date: 17/02/2010
 * Time: 8:43 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using Org.Reddragonit.Stringtemplate.Interfaces;
using Org.Reddragonit.Stringtemplate.Tokenizers;
using System.Text.RegularExpressions;

namespace Org.Reddragonit.Stringtemplate.Components.Base
{
	/// <summary>
	/// Description of SpecialCharacterComponent.
	/// </summary>
	public class SpecialCharacterComponent : IComponent
	{
		
		private static Regex _reg = new Regex("^(NEW_LINE|TAB|SPACE)$",RegexOptions.Compiled|RegexOptions.ECMAScript);
		
		private enum Values{
			NEW_LINE,
			TAB,
			SPACE
		}
		
		private Values _val;
		
		public SpecialCharacterComponent()
		{
		}
		
		public bool CanLoad(Token token)
		{
			return _reg.IsMatch(token.Content);
		}
		
		public bool Load(Queue<Token> tokens, Type tokenizerType, TemplateGroup @group)
		{
			Token t = tokens.Dequeue();
			_val = (Values)Enum.Parse(typeof(Values),t.Content);
			return true;
		}
		
		public string GenerateString(ref Dictionary<string, object> variables)
		{
			string ret="";
			switch(_val){
				case Values.NEW_LINE:
					ret = "\n";
					break;
				case Values.TAB:
					ret = "\t";
					break;
				case Values.SPACE:
					ret = " ";
					break;
			}
			return ret;
		}
		
		public IComponent NewInstance()
		{
			return new SpecialCharacterComponent();
		}
	}
}
