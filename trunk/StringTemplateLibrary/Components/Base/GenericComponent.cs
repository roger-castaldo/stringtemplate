/*
 * Created by SharpDevelop.
 * User: Roger
 * Date: 01/12/2008
 * Time: 3:29 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;
using System.Collections.Generic;
using Org.Reddragonit.Stringtemplate.Interfaces;
using Org.Reddragonit.Stringtemplate.Tokenizers;

namespace Org.Reddragonit.Stringtemplate.Components.Base
{
	/// <summary>
	/// Description of GenericComponent.
	/// </summary>
	public class GenericComponent : IComponent 
	{
		public GenericComponent()
		{
		}
		
		private string _data;

        public bool CanLoad(Org.Reddragonit.Stringtemplate.Tokenizers.Token token)
        {
            return true;
        }

        public bool Load(Queue<Token> tokens, Type tokenizerType,TemplateGroup group)
        {
            _data = tokens.Dequeue().Content;
            return true;
        }

        public string GenerateString(ref Dictionary<string, object> variables)
        {
            return _data;
        }

        public IComponent NewInstance() {
            return new GenericComponent();
        }
    }
}
