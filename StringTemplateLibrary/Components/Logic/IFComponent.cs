using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Stringtemplate.Interfaces;
using System.Text.RegularExpressions;
using Org.Reddragonit.Stringtemplate.Tokenizers;
using Org.Reddragonit.Stringtemplate.Outputs;


namespace Org.Reddragonit.Stringtemplate.Components.Logic
{
    public class IFComponent : IComponent 
    {

        private static Regex regIf = new Regex("^([i|I][f|F])\\((.+)\\)$", RegexOptions.Compiled | RegexOptions.ECMAScript);
        private static Regex regElseIf = new Regex("^([e|E][L|l][S|s][e|E][i|I][f|F])\\((.+)\\)$", RegexOptions.Compiled | RegexOptions.ECMAScript);
        private static Regex regElse = new Regex("^([e|E][L|l][S|s][e|E])$", RegexOptions.Compiled | RegexOptions.ECMAScript);
        private static Regex regEndIf = new Regex("^([E|e][N|n][D|d][I|i][F|f])$", RegexOptions.Compiled | RegexOptions.ECMAScript);
        
        private struct IfStatement
        {
            private IComponent _condition;
            public IComponent Condition
            {
                get { return _condition; }
            }

            private List<IComponent> _children;
            public List<IComponent> Children
            {
                get { return _children; }
            }

            public IfStatement(IComponent condition, List<IComponent> children)
            {
                _condition = condition;
                _children = children;
            }
        }

        private List<IfStatement> _statements = new List<IfStatement>();

        public bool CanLoad(Token token)
        {
            return regIf.IsMatch(token.Content);
        }

        public bool Load(Queue<Token> tokens, Type tokenizerType,TemplateGroup group)
        {
            Token curToken = tokens.Dequeue();
            Queue<Token> tmp = new Queue<Token>();
            tmp.Enqueue(new Token(regIf.Match(curToken.Content).Groups[2].Value, TokenType.COMPONENT));
            IComponent curCondition = ComponentExtractor.ExtractComponent(tmp, tokenizerType, group);
            List<IComponent> curChildren = new List<IComponent>();
            while ((tokens.Count>0)&&!regEndIf.IsMatch(tokens.Peek().Content))
            {
                if (regIf.IsMatch(tokens.Peek().Content))
                {
                    IFComponent ifc = new IFComponent();
                    ifc.Load(tokens, tokenizerType, group);
                    curChildren.Add(ifc);
                }
                else
                {
                    curToken = tokens.Peek();
                    if (regElseIf.IsMatch(curToken.Content))
                    {
                        _statements.Add(new IfStatement(curCondition, curChildren));
                        curChildren = new List<IComponent>();
                        curToken = tokens.Dequeue();
                        tmp.Clear();
                        tmp.Enqueue(new Token(regElseIf.Match(curToken.Content).Groups[2].Value, TokenType.COMPONENT));
                        curCondition = ComponentExtractor.ExtractComponent(tmp, tokenizerType, group);
                    }
                    else if (regElse.IsMatch(curToken.Content))
                    {
                        _statements.Add(new IfStatement(curCondition, curChildren));
                        curChildren = new List<IComponent>();
                        curToken = tokens.Dequeue();
                        tmp.Clear();
                        tmp.Enqueue(new Token("true", TokenType.TEXT));
                        curCondition = ComponentExtractor.ExtractComponent(tmp, tokenizerType, group);
                    }
                    else
                        curChildren.Add(ComponentExtractor.ExtractComponent(tokens, tokenizerType, group));
                }
            }
            _statements.Add(new IfStatement(curCondition, curChildren));
            if (tokens.Count != 0)
                tokens.Dequeue();
            return true;
        }

        public void Append(ref Dictionary<string, object> variables, IOutputWriter writer)
        {
            StringOutputWriter swo = new StringOutputWriter();
            foreach (IfStatement state in _statements)
            {
                swo.Clear();
                state.Condition.Append(ref variables, swo);
                if (Utility.IsComponentTrue(swo.ToString()))
                {
                    foreach (IComponent child in state.Children)
                        child.Append(ref variables, writer);
                    break;
                }
            }
        }

        public IComponent NewInstance() {
            return new IFComponent();
        }
    }
}
