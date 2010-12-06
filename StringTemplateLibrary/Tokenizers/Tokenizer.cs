/*
 * Created by SharpDevelop.
 * User: Roger
 * Date: 01/12/2008
 * Time: 2:49 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.IO;
using Org.Reddragonit.Stringtemplate.Interfaces;
using Org.Reddragonit.Stringtemplate.Components;

namespace Org.Reddragonit.Stringtemplate.Tokenizers
{

    public enum TokenType
    {
        TEXT,
        COMPONENT
    }

    public struct Token
    {
        private string _content;
        public string Content
        {
            get { return _content; }
        }

        private TokenType _type;
        public TokenType Type
        {
            get { return _type; }
        }

        public Token(string content, TokenType type)
        {
            _content = content;
            _type = type;
        }
    }
	/// <summary>
	/// Description of ITokenizer.
	/// </summary>
	public abstract class Tokenizer
	{
        protected abstract char TokenChar { get; }
        protected abstract char EscapeChar { get; }

        private const char EOF = (char)0;

        private string _template;
        private int _index;
        private char _lastChar;
        private char _curChar=EOF;
        private char _nextChar;
        private string _curChunk;
        private Queue<Token> _chunks;

        public Tokenizer(TextReader tr)
        {
            _template = tr.ReadToEnd();
        }

        public Tokenizer(string template)
        {
            _template = template;
        }

        private void Next()
        {
            if (_index==_template.Length)
            {
                _curChar = EOF;
                return;
            }
            _lastChar = _curChar;
            _curChar = _template[_index];
            _index++;
            if (_index == _template.Length)
                _nextChar = EOF;
            else
                _nextChar = _template[_index];
        }

        private void Consume()
        {
            _curChunk += _curChar;
            Next();
        }

        private void ConsumeBracket(char startBracket)
        {
            Consume();
            char exitChar = '}';
            switch (startBracket){
                case '[':
                    exitChar=']';
                    break;
                case '(':
                    exitChar=')';
                    break;
            }
            while ((_curChar!=exitChar)&&(_curChar!=EOF))
            {
            	if ((_curChar == '(') || (_curChar == '{') || (_curChar == '['))
            		ConsumeBracket(_curChar);
            	else
                	Consume();
            }
            if (_curChar!=EOF || _curChar==exitChar)
                Consume();
        }

        private void AddCurrentChunk(TokenType type)
        {
            _chunks.Enqueue(new Token(_curChunk, type));
            _curChunk = "";
        }

        public List<IComponent> TokenizeStream(TemplateGroup group)
        {
            _chunks = new Queue<Token>();
            _curChunk = "";
            _index = 0;
            Next();
            while (_curChar != EOF)
            {
                if ((_curChar == EscapeChar)&&(_nextChar==TokenChar))
                {
                    Next();
                    Consume();
                }
                else if (_curChar == TokenChar)
                {
                    if (_curChunk.Length > 0)
                    {
                        AddCurrentChunk(TokenType.TEXT);
                    }
                    Next();
                    while ((_curChar != TokenChar)&&(_lastChar!=EscapeChar)&&(_curChar!=EOF))
                    {
                        if ((_curChar == '(') || (_curChar == '{') || (_curChar == '['))
                            ConsumeBracket(_curChar);
                        else if ((_curChar == EscapeChar) && (_nextChar == TokenChar))
                        {
                            Next();
                            Consume();
                        }
                        else
                            Consume();
                    }
                    if (_curChar == TokenChar)
                        Next();
                    _curChunk = _curChunk.Trim();
                    AddCurrentChunk(TokenType.COMPONENT);
                }
                else
                    Consume();
            }
            if (_curChunk.Length > 0)
                AddCurrentChunk(TokenType.TEXT);
            List<IComponent> ret = new List<IComponent>();
            while (_chunks.Count > 0)
            {
            	System.Diagnostics.Debug.WriteLine("Analyzing chunk: "+_chunks.Peek().Content);
                ret.Add(ComponentExtractor.ExtractComponent(_chunks,this.GetType(),group));
            }
            return ret;
        }
	}
}
