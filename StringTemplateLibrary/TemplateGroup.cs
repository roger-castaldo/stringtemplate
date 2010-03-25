using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Stringtemplate.Tokenizers;
using System.Text.RegularExpressions;
using Org.Reddragonit.Stringtemplate.Interfaces;
using Org.Reddragonit.Stringtemplate.Components.Base;

using System.IO;

namespace Org.Reddragonit.Stringtemplate
{
    public class TemplateGroup
    {
        private static Regex regFunctionExtractor = new Regex("^([^\\(]+)\\(([^\\)]*)\\)\\s*::=\\s*<<",RegexOptions.Compiled|RegexOptions.Multiline);

        private List<IComponent> _tokenized=null;
        private string _groupName = null;

        public TemplateGroup(TextReader reader)
            : this(reader, typeof(DefaultTokenizer))
        {
        }

        public TemplateGroup(TextReader reader, Type tokenizerType)
            : this(reader.ReadToEnd(), tokenizerType)
        {
            reader.Close();
        }

        public TemplateGroup(StreamReader reader)
            : this(reader,typeof(DefaultTokenizer))
        {
        }

        public TemplateGroup(StreamReader reader,Type tokenizerType)
            : this(reader.ReadToEnd(),tokenizerType)
        {
            reader.Close();
        }
		
		public TemplateGroup(string templateCode) : this(templateCode,typeof(DefaultTokenizer))
		{
		}
		
		public TemplateGroup(string templateCode, Type tokenizerType){
            if (templateCode.StartsWith("group"))
            {
                _groupName = templateCode.Substring(0, templateCode.IndexOf(";"));
                _groupName = _groupName.Substring("group".Length).Trim();
                templateCode = templateCode.Substring(templateCode.IndexOf(";")+1);
            }

            _tokenized = new List<IComponent>();

            Match m;
            while ((m = regFunctionExtractor.Match(templateCode)) != null)
            {
                if (m.Value.Length == 0)
                {
                    break;
                }
                else
                {
                    string funcName = m.Groups[1].Value.Trim();
                    List<string> pars = new List<string>(m.Groups[2].Value.Split(','));
                    for (int x = 0; x < pars.Count; x++)
                    {
                        if (pars[x].Length==0){
                            pars.RemoveAt(x);
                            x--;
                        }
                    }
                    string code = templateCode.Substring(m.Value.Length + 1, templateCode.IndexOf(">>") - m.Value.Length - 1);
                    code = code.Substring(0, code.Length - 2).Trim();
                    Tokenizer t = (Tokenizer)tokenizerType.GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { code });
                    _tokenized.Add(new SubTemplateComponent(_groupName, funcName, null, pars.ToArray(),this));
                    ((SubTemplateComponent)_tokenized[_tokenized.Count - 1]).Components = t.TokenizeStream(this);
                    templateCode = templateCode.Substring(templateCode.IndexOf(">>") + 2).Trim();
                }
            }
		}

        public List<IComponent> Components
        {
            get { return _tokenized; }
        }
    }
}
