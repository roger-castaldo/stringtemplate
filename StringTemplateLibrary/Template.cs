﻿/*
 * Created by SharpDevelop.
 * User: Roger
 * Date: 26/01/2010
 * Time: 11:52 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Org.Reddragonit.Stringtemplate.Tokenizers;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Stringtemplate.Interfaces;
using System.IO;
using System.Collections;
using Org.Reddragonit.Stringtemplate.Outputs;

namespace Org.Reddragonit.Stringtemplate
{
	/// <summary>
	/// Description of Template.
	/// </summary>
	public class Template
	{
		private string _templateCode;
		private Tokenizer _tokenizer;
		private Dictionary<string,object> _parameters;
        private List<IComponent> _tokenized=null;
        private TemplateGroup _templateGroup = null;

        public Template(TextReader reader)
            : this(reader, typeof(DefaultTokenizer))
        {
        }

        public Template(TextReader reader, Type tokenizerType)
            : this(reader, null, tokenizerType)
        {
        }

        public Template(TextReader reader, TemplateGroup templateGroup)
            : this(reader, templateGroup, typeof(DefaultTokenizer))
        {
        }

        public Template(TextReader reader, TemplateGroup templateGroup, Type tokenizerType)
            : this(reader.ReadToEnd(), templateGroup, tokenizerType)
        {
            reader.Close();
        }

        public Template(StreamReader reader)
            : this(reader,typeof(DefaultTokenizer))
        {
        }

        public Template(StreamReader reader,Type tokenizerType)
            : this(reader,null,tokenizerType)
        {
        }

        public Template(StreamReader reader,TemplateGroup templateGroup)
            : this(reader, templateGroup, typeof(DefaultTokenizer))
        {
        }

        public Template(StreamReader reader,TemplateGroup templateGroup,Type tokenizerType)
            : this(reader.ReadToEnd(),templateGroup,tokenizerType)
        {
            reader.Close();
        }

		public Template(string templateCode) : this(templateCode,typeof(DefaultTokenizer))
		{
		}
		
		public Template(string templateCode, Type tokenizerType) : this(templateCode,null,tokenizerType){
		}

        public Template(string templateCode, TemplateGroup templateGroup) : this(templateCode,templateGroup,typeof(DefaultTokenizer))
        { }

        public Template(TemplateGroup templateGroup, string templateCode) : this(templateCode, templateGroup) { }

        public Template(string templateCode, TemplateGroup templateGroup, Type tokenizerType)
        {
            _templateCode = templateCode;
            _tokenizer = (Tokenizer)tokenizerType.GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { templateCode });
            _parameters = new Dictionary<string, object>();
            _templateGroup = templateGroup;
        }

        private Template() { }

        public Template Clone()
        {
            Template ret = new Template();
            ret._templateCode = _templateCode;
            ret._tokenizer = _tokenizer;
            ret._parameters = new Dictionary<string, object>();
            ret._templateGroup = _templateGroup;
            ret._tokenized = _tokenized;
            return ret;
        }

        public void SetParameter(string name, object value)
        {
            if (_parameters.ContainsKey(name))
            {
                ArrayList tmp = new ArrayList();
                tmp.Add(value);
                if (_parameters[name] is ArrayList)
                {
                    tmp.AddRange(((ArrayList)_parameters[name]));
                }
                _parameters.Remove(name);
                _parameters.Add(name,tmp);
            }else
                _parameters.Add(name, value);
        }

        public void SetAttribute(string name, object value) 
        {
            SetParameter(name, value);
        }

        public void ClearParameters()
        {
            _parameters.Clear();
        }

        public void RemoveParameter(string name)
        {
            if (_parameters.ContainsKey(name))
                _parameters.Remove(name);
        }
		
		public override string ToString(){
            if (_tokenized == null)
                _tokenized = _tokenizer.TokenizeStream(_templateGroup);
            StringOutputWriter writer = new StringOutputWriter();
            foreach (IComponent comp in _tokenized)
                comp.Append(ref _parameters,writer);
            return writer.ToString();
		}

        public void WriteToStream(Stream ostream)
        {
            if (_tokenized == null)
                _tokenized = _tokenizer.TokenizeStream(_templateGroup);
            StreamOutputWriter writer = new StreamOutputWriter(ostream);
            foreach (IComponent comp in _tokenized)
                comp.Append(ref _parameters, writer);
            writer.Flush();
        }

        public void WriteToOutputWriter(IOutputWriter writer)
        {
            if (_tokenized == null)
                _tokenized = _tokenizer.TokenizeStream(_templateGroup);
            foreach (IComponent comp in _tokenized)
                comp.Append(ref _parameters, writer);
        }
	}
}
