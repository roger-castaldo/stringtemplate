﻿/*
 * Created by SharpDevelop.
 * User: Roger
 * Date: 27/01/2010
 * Time: 8:21 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Org.Reddragonit.Stringtemplate.Interfaces;
using Org.Reddragonit.Stringtemplate.Tokenizers;

namespace Org.Reddragonit.Stringtemplate.Components.Logic
{
	/// <summary>
	/// Description of CompareComponent.
	/// </summary>
	public class CompareComponent : IComponent 
	{
		private enum CompareType{
			EQUAL,
			NOT_EQUAL,
			GREATER_THAN,
			LESS_THAN,
			GREATER_THAN_OR_EQUAL_TO,
			LESS_THAN_OR_EQUAL_TO
		}
		
		private static readonly Regex regFunctionStyle = new Regex("^("+
			"(([N|n][O|o][T|t])?[E|e][Q|q][U|u][A|a][L|l])|"+
			"((([G|g][R|r][E|e][A|a][T|t][E|e][R|r])|"+
			"([L|l][E|e][S|s][S|s]))([T|t][H|h][A|a][N|n])"+
			"([O|o][R|r][E|e][Q|q][U|u][A|a][L|l][T|t][O|o])?))"+
			"\\((.+),(.+)\\)$"
			,RegexOptions.Compiled|RegexOptions.ECMAScript
		);
		
		private static readonly Regex regSimpleStyle = new Regex("^\\((.+)\\s+(([E|e][Q|q])|([N|n][E|e])|([G|g][T|t])|([L|l][T|t])|([G|g][E|e])|([L|l][E|e]))\\s+(.+)\\)$",
		                                                        RegexOptions.Compiled|RegexOptions.ECMAScript);
		
		private CompareType _type = CompareType.EQUAL;
		private IComponent _left;
		private IComponent _right;
		
		public bool CanLoad(Token token)
		{
			return regFunctionStyle.IsMatch(token.Content)||regSimpleStyle.IsMatch(token.Content);
		}
		
		public bool Load(Queue<Token> tokens, Type tokenizerType,TemplateGroup group)
		{
			Token t = tokens.Dequeue();
			Queue<Token> toks = new Queue<Token>();
			Match m;
			if (regSimpleStyle.IsMatch(t.Content)){
				m = regSimpleStyle.Match(t.Content);
				toks.Enqueue(new Token(m.Groups[1].Value.Trim(), TokenType.COMPONENT));
				_left = ComponentExtractor.ExtractComponent(toks,tokenizerType,group);
				toks.Clear();
                switch (m.Groups[2].Value.ToUpper())
                {
                    case "NE":
                        _type = CompareType.NOT_EQUAL;
                        break;
                    case "GT":
                        _type = CompareType.GREATER_THAN;
                        break;
                    case "GE":
                        _type = CompareType.GREATER_THAN_OR_EQUAL_TO;
                        break;
                    case "LT":
                        _type = CompareType.LESS_THAN;
                        break;
                    case "LE":
                        _type = CompareType.LESS_THAN_OR_EQUAL_TO;
                        break;
                }
				toks.Enqueue(new Token(m.Groups[9].Value.Trim(),TokenType.COMPONENT));
				_right = ComponentExtractor.ExtractComponent(toks,tokenizerType,group);
			}else{
				m = regFunctionStyle.Match(t.Content);
                switch (m.Groups[1].Value.ToUpper())
                {
                    case "NOTEQUAL":
                        _type = CompareType.NOT_EQUAL;
                        break;
                    case "GREATERTHAN":
                        _type = CompareType.GREATER_THAN;
                        break;
                    case "GREATERTHANOREQUALTO":
                        _type = CompareType.GREATER_THAN_OR_EQUAL_TO;
                        break;
                    case "LESSTHAN":
                        _type = CompareType.LESS_THAN;
                        break;
                    case "LESSTHANOREQUALTO":
                        _type = CompareType.LESS_THAN_OR_EQUAL_TO;
                        break;
                }
				toks.Enqueue(new Token(m.Groups[10].Value,TokenType.COMPONENT));
                _left = ComponentExtractor.ExtractComponent(toks, tokenizerType, group);
				toks.Clear();
				toks.Enqueue(new Token(m.Groups[11].Value,TokenType.COMPONENT));
                _right = ComponentExtractor.ExtractComponent(toks, tokenizerType, group);
			}
			return true;
		}
		
		public string GenerateString(ref Dictionary<string, object> variables)
		{
			string ret = "false";
			string l = _left.GenerateString(ref variables);
			string r = _right.GenerateString(ref variables);
			switch (_type){
				case CompareType.EQUAL:
					ret = Utility.StringsEqual(_left.GenerateString(ref variables),_right.GenerateString(ref variables)).ToString();
					break;
				case CompareType.GREATER_THAN:
					ret = ((l!=null)&&((r==null)||(l.CompareTo(r)>0))).ToString();
					break;
				case CompareType.GREATER_THAN_OR_EQUAL_TO:
					ret = (((l==null)&&(r==null))||((l!=null)&&((r==null)||(l.CompareTo(r)>=0)))).ToString();
					break;
				case CompareType.LESS_THAN:
					ret = ((r!=null)&&((l==null)||l.CompareTo(r)<0)).ToString();
					break;
				case CompareType.LESS_THAN_OR_EQUAL_TO:
					ret = (((l==null)&&(r==null))||((r!=null)&&((l==null)||l.CompareTo(r)<=0))).ToString();
					break;
				case CompareType.NOT_EQUAL:
					ret = (!Utility.StringsEqual(_left.GenerateString(ref variables),_right.GenerateString(ref variables))).ToString();
					break;
			}
			return ret;
		}

        public IComponent NewInstance()
        {
            return new CompareComponent();
        }
	}
}
