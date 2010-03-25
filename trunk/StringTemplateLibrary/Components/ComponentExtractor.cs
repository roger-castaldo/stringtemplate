/*
 * Created by SharpDevelop.
 * User: Roger
 * Date: 01/12/2008
 * Time: 2:59 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using Org.Reddragonit.Stringtemplate.Components.Base;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Org.Reddragonit.Stringtemplate.Interfaces;


namespace Org.Reddragonit.Stringtemplate.Components
{
	/// <summary>
	/// Description of ComponentExtractor.
	/// </summary>
	public class ComponentExtractor
	{
		
		private static Mutex mut = new Mutex(false);
		private static List<IComponent> _components=null;
		
		static ComponentExtractor()
		{
			mut.WaitOne();
			if (_components==null)
			{
				_components = new List<IComponent>();
				foreach (Type type in Utility.LocateTypeInstances(typeof(IComponent)))
                {
                    if (!type.Equals(typeof(GenericComponent))&&!type.Equals(typeof(ParameterComponent))&&!type.IsInterface&&!type.IsAbstract&&!type.Equals(typeof(FunctionComponent))&&!type.Equals(typeof(SubTemplateComponent)))
                        _components.Add((IComponent)type.GetConstructor(Type.EmptyTypes).Invoke(new object[0]));
				}
                _components.Add(new ParameterComponent());
			}
			mut.ReleaseMutex();
		}
		
		public static IComponent ExtractComponent(Queue<Tokenizers.Token> tokens,Type tokenizerType,TemplateGroup group)
		{
            IComponent ret = null;
            if (tokens.Peek().Type == Tokenizers.TokenType.TEXT)
            {
                ret = new GenericComponent();
                ret.Load(tokens,tokenizerType,group);
                return ret;
            }
			//mut.WaitOne();
			foreach (IComponent comp in _components)
			{
				if (comp.CanLoad(tokens.Peek()))
				{
                    ret = comp.NewInstance();
					ret.Load(tokens,tokenizerType,group);
					break;
				}
			}
			//mut.ReleaseMutex();
            if ((group != null)&&(ret==null))
            {
                foreach (IComponent comp in group.Components)
                {
                    if (comp.CanLoad(tokens.Peek()))
                    {
                        ret = comp.NewInstance();
                        ret.Load(tokens, tokenizerType, group);
                        break;
                    }
                }
            }
            if (ret == null)
            {
                ret = new GenericComponent();
                ret.Load(tokens,tokenizerType,group);
                return ret;
            }
			return ret;
		}
	}
}
