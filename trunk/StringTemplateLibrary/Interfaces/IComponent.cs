/*
 * Created by SharpDevelop.
 * User: Roger
 * Date: 01/12/2008
 * Time: 2:46 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;
using System.Collections.Generic;

namespace Org.Reddragonit.Stringtemplate.Interfaces
{
	/// <summary>
	/// Description of IComponent.
	/// </summary>
	public interface IComponent
	{	
		bool CanLoad(Tokenizers.Token token);
        bool Load(Queue<Tokenizers.Token> tokens, Type tokenizerType,TemplateGroup group);
		string GenerateString(ref Dictionary<string, object> variables);
        IComponent NewInstance();
	}
}
