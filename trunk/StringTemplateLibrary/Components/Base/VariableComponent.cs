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
using System.Collections;
using System.Reflection;
using Org.Reddragonit.Stringtemplate.Outputs;

namespace Org.Reddragonit.Stringtemplate.Components.Base
{
	/// <summary>
	/// Description of VariableComponent.
	/// </summary>
	public class VariableComponent : IComponent 
	{
        private static readonly Regex _reg = new Regex("^DECLARE\\s*\\(([^\\s]+)\\s+[A|a][S|s]\\s*([^\\s=]+)\\s*=\\s*([N|n][E|e][W|w]\\s*)?(([A-Za-z0-9_]+\\.)*([A-Za-z0-9_]+))?[\\(\\{\\[]?\\s*(.+)[\\)\\}\\]]?\\s*\\)$", RegexOptions.Compiled | RegexOptions.ECMAScript);

		public VariableComponent()
		{
		}
		
		//SAMPLE: DELCARE(x as Integer = 0)
		//SAMPLE: DECLARE(names as String[] = {"bob","fred","jack"})
		//SAMPLE: DECLARE(fred as Org.Sample.Name = NEW Name("bob","loblaw"))
		//SAMPLE: DECLARE(fred as Org.Sample.Name = Name.Construct("bob","loblaw"))
        //SAMPLE: DECLARE(fred as Org.Sample.Name = Name.Construct($names.bob$,$names.loblaw$))
		
		private string _variableName;
		private Type _variableType;
		private bool _constructor=false;
		private bool _staticCall = false;
		private bool _isArray=false;
        private string _constructorCall = null;
        private List<IComponent> _components = null;

        public bool CanLoad(Org.Reddragonit.Stringtemplate.Tokenizers.Token token)
        {
            return _reg.IsMatch(token.Content);
        }

        public bool Load(Queue<Token> tokens, Type tokenizerType,TemplateGroup group)
        {
            Match m = _reg.Match(tokens.Dequeue().Content);
            _variableName = m.Groups[1].Value;
            _variableType = Utility.LocateType(m.Groups[2].Value.Replace("[]",""));
            if (_variableType==null)
                _variableType=Utility.LocateType("System."+m.Groups[2].Value.Replace("[]", ""));
            _isArray = m.Groups[2].Value.Contains("[]");
            if (_variableType == null)
                return false;
            _constructor = m.Groups[3].Value.ToUpper().Trim()=="NEW";
            int y = 0;
            foreach (Group g in m.Groups)
            {
                System.Diagnostics.Debug.WriteLine(y.ToString() + ":" + g.Value);
                y++;
            }
            _constructorCall = m.Groups[4].Value;
            if (!_constructor)
                _staticCall = m.Groups[6].Value.Length > 0;
            string variables = m.Groups[7].Value;
            if (variables.EndsWith(")") || variables.EndsWith("}"))
                variables = variables.Substring(0, variables.Length - 1);
            string tmp = "";
            Tokenizer tok = null;
            if (_isArray || _constructor || _staticCall)
            {
                char[] chars = variables.ToCharArray();
                for (int x = 0; x < chars.Length; x++)
                {
                    if (chars[x] == ',')
                    {
                        tok = (Tokenizer)tokenizerType.GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { tmp });
                        if (_components == null)
                            _components = tok.TokenizeStream(group);
                        else
                            _components.AddRange(tok.TokenizeStream(group).ToArray());
                        tmp = "";
                    }
                    else if ((chars[x] == '\'') || (chars[x] == '"'))
                    {
                        tmp = processQuote(chars, ref x);
                    }
                    else
                        tmp += chars[x].ToString();
                }
                if (tmp.Length > 0)
                {
                    tok = (Tokenizer)tokenizerType.GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { tmp });
                    if (_components == null)
                        _components = tok.TokenizeStream(group);
                    else
                        _components.AddRange(tok.TokenizeStream(group).ToArray());
                }
            }
            else
            {
                if ((variables.StartsWith("\"") || variables.StartsWith("'"))&&(variables.EndsWith("\"") || variables.EndsWith("'")))
                    variables = variables.Substring(1,variables.Length-2);
                tok = (Tokenizer)tokenizerType.GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { variables });
                _components = tok.TokenizeStream(group);
            }
            foreach (IComponent ic in _components)
            {
                System.Diagnostics.Debug.WriteLine(ic.ToString());
            }
            return true;
        }

        private string processQuote(char[] chars, ref int x)
        {
            string ret = "";
            x++;
            switch (chars[x-1]){
                case '"':
                    while (true)
                    {
                        if (chars[x] == '\\' && (x + 1 < chars.Length) && (chars[x + 1] == '"'))
                        {
                            ret += "\"";
                            x += 2;
                        }
                        else if (chars[x] == '"')
                        {
                            x++;
                            break;
                        }
                        else
                        {
                            ret += chars[x].ToString();
                            x++;
                        }
                    }
                    break;
                case '\'':
                    while (true)
                    {
                        if (chars[x] == '\\' && (x + 1 < chars.Length) && (chars[x + 1] == '\''))
                        {
                            ret += "'";
                            x += 2;
                        }
                        else if (chars[x] == '\'')
                        {
                            x++;
                            break;
                        }
                        else
                        {
                            ret += chars[x].ToString();
                            x++;
                        }
                    }
                    break;
            }
            x--;
            return ret;
        }

        public void Append(ref Dictionary<string, object> variables, IOutputWriter writer)
        {
            StringOutputWriter swo = new StringOutputWriter();
            object obj = null;
            Type t = null;
            object[] objs = new object[_components.Count];
            if (_constructor)
            {
                t = Utility.LocateType(_constructorCall);
                foreach (ConstructorInfo c in t.GetConstructors())
                {
                    if (c.GetParameters().Length == _components.Count)
                    {
                        try
                        {
                            for (var x = 0; x < objs.Length; x++)
                            {
                                IComponent ic = _components[x];
                                if (ic is ParameterComponent)
                                    objs[x] = Convert.ChangeType(((ParameterComponent)ic).GetObjectValue(variables), c.GetParameters()[x].ParameterType);
                                else
                                {
                                    swo.Clear();
                                    ic.Append(ref variables, swo);
                                    objs[x] = Convert.ChangeType(swo.ToString(), c.GetParameters()[x].ParameterType);
                                }
                            }
                            obj = c.Invoke(objs);
                            break;
                        }
                        catch (Exception e)
                        {
                        }
                    }
                }
            }
            else if (_staticCall)
            {
                t = Utility.LocateType(_constructorCall.Substring(0, _constructorCall.LastIndexOf(".")));
                MethodInfo mi = null;
                foreach (MethodInfo m in t.GetMethods(BindingFlags.Static|BindingFlags.Public))
                {
                    if (m.Name == _constructorCall.Substring(_constructorCall.LastIndexOf(".") + 1))
                    {
                        if (m.GetParameters().Length == _components.Count)
                        {
                            mi = m;
                            break;
                        }
                    }
                }
                if (mi != null)
                {
                    for (var x = 0; x < objs.Length; x++)
                    {
                        IComponent ic = _components[x];
                        if (ic is ParameterComponent)
                            objs[x] = Convert.ChangeType(((ParameterComponent)ic).GetObjectValue(variables), mi.GetParameters()[x].ParameterType);
                        else
                        {
                            swo.Clear();
                            ic.Append(ref variables, swo);
                            objs[x] = Convert.ChangeType(swo.ToString(), mi.GetParameters()[x].ParameterType);
                        }
                    }
                    obj = mi.Invoke(null, objs);
                }
            }
            else if (_isArray)
            {
                obj = new ArrayList();
                foreach (IComponent ic in _components)
                {
                    if (ic is ParameterComponent)
                        ((ArrayList)obj).Add(Convert.ChangeType(((ParameterComponent)ic).GetObjectValue(variables), _variableType));
                    else
                    {
                        swo.Clear();
                        ic.Append(ref variables, swo);
                        ((ArrayList)obj).Add(Convert.ChangeType(swo.ToString(), _variableType));
                    }
                }
            }
            else
            {
                if (_components.Count == 1)
                {
                    if (_components[0] is ParameterComponent)
                        obj = Convert.ChangeType(((ParameterComponent)_components[0]).GetObjectValue(variables), _variableType);
                    else
                    {
                        swo.Clear();
                        _components[0].Append(ref variables, swo);
                        obj = Convert.ChangeType(swo.ToString(), _variableType);
                    }
                }
                else
                {
                    swo.Clear();
                    foreach (IComponent ic in _components)
                        ic.Append(ref variables, swo);
                    obj = Convert.ChangeType(swo.ToString(),_variableType);
                }
            }
            if (variables.ContainsKey(_variableName))
                variables.Remove(_variableName);
            variables.Add(_variableName, obj);
        }

        #region IComponent Members


        public IComponent NewInstance()
        {
            return new VariableComponent();
        }

        #endregion
    }
}
