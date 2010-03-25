/*
 * Created by SharpDevelop.
 * User: Roger
 * Date: 15/01/2009
 * Time: 6:08 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections;

namespace Org.Reddragonit.Stringtemplate
{
	/// <summary>
	/// This class is a main utility class used to house functionality called upong by other
    /// sections of code.
	/// </summary>
	public class Utility
	{
		
        //Called to locate a system type by running through all assemblies within the current
        //domain until it is able to locate the requested Type.
		public static Type LocateType(string typeName)
        {
            Type t = Type.GetType(typeName, false, true);
            if (t == null)
            {
                foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        if (!ass.GetName().Name.Contains("mscorlib") && !ass.GetName().Name.StartsWith("System") && !ass.GetName().Name.StartsWith("Microsoft"))
                        {
                            t = ass.GetType(typeName, false, true);
                            if (t != null)
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        if (e.Message != "The invoked member is not supported in a dynamic assembly.")
                        {
                            throw e;
                        }
                    }
                }
            }
            return t;
        }

        public static List<Type> LocateTypeInstances(Type parent)
        {
            List<Type> ret = new List<Type>();
            foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    if (!ass.GetName().Name.Contains("mscorlib") && !ass.GetName().Name.StartsWith("System") && !ass.GetName().Name.StartsWith("Microsoft"))
                    {
                        foreach (Type t in ass.GetTypes())
                        {
                            if (t.IsSubclassOf(parent) || (parent.IsInterface && new List<Type>(t.GetInterfaces()).Contains(parent)))
                                ret.Add(t);
                        }
                    }
                }
                catch (Exception e)
                {
                    if (e.Message != "The invoked member is not supported in a dynamic assembly.")
                    {
                        throw e;
                    }
                }
            }
            return ret;
        }

        public static List<Type> LocateTypeInstances(Type parent, Assembly ass)
        {
            List<Type> ret = new List<Type>();
            try
            {
                foreach (Type t in ass.GetTypes())
                {
                    if (t.IsSubclassOf(parent) || (parent.IsInterface && new List<Type>(t.GetInterfaces()).Contains(parent)))
                        ret.Add(t);
                }
            }
            catch (Exception e)
            {
                if (e.Message != "The invoked member is not supported in a dynamic assembly.")
                {
                    throw e;
                }
            }
            return ret;
        }
		
        //Merges to string arrays into one, commonly called for the keywords
        //declarations within a connection
		public static string[] MergeStringArrays(string[] array1,string[] array2)
		{
			List<string> ret = new List<string>();
			ret.AddRange(array1);
			ret.AddRange(array2);
			return ret.ToArray();
		}
		
        //called to compare to string whill checkign for nulls
		public static bool StringsEqual(string str1,string str2)
		{
			if (str1==null)
			{
				if (str2!=null)
					return false;
				return true;
			}else if (str2==null)
			{
				return false;
			}
			return str1.Equals(str2);
		}
		
        //called to compare to strings while checking for nulls and ignoring all whitespaces as well
        //as case.  This is typically used to compare stored procedures.
		public static bool StringsEqualIgnoreCaseWhitespace(string str1,string str2)
		{
			if (str1==null)
			{
				if (str2!=null)
					return false;
				return true;
			}else if (str2==null)
			{
				return false;
			}
			Regex r = new Regex("\\s+");
			return r.Replace(str1.ToUpper(),"").Equals(r.Replace(str2.ToUpper(),""));
		}
		
        //called to compare to string while checking for nulls and ignoring all whitespaces.
		public static bool StringsEqualIgnoreWhitespace(string str1,string str2)
		{
			if (str1==null)
			{
				if (str2!=null)
					return false;
				return true;
			}else if (str2==null)
			{
				return false;
			}
			Regex r = new Regex("\\s+");
			return r.Replace(str1,"").Equals(r.Replace(str2,""));
		}
		
        //called to clean up duplicate strings from a string array.  This
        //is used to clean up the keyword arrays made by the connections.
		public static void RemoveDuplicateStrings(ref List<string> list,string[] ignores)
		{
			for(int x=0;x<list.Count;x++)
			{
				bool process=true;
				foreach (string str in ignores)
				{
                    if (StringsEqualIgnoreWhitespace(list[x],str))
					{
						process=false;
						break;
					}
				}
				if (process)
				{
					for (int y=x+1;y<list.Count;y++)
					{
                        if (StringsEqualIgnoreWhitespace(list[y],list[x]))
						{
							list.RemoveAt(y);
							y--;
						}
					}
				}
			}
		}

        internal static List<int> SortDictionaryKeys(Dictionary<int, int>.KeyCollection keys)
        {
            int[] tmp = new int[keys.Count];
            keys.CopyTo(tmp, 0);
            List<int> ret = new List<int>(tmp);
            ret.Sort();
            return ret;
        }

        internal static List<int> SortDictionaryKeys(Dictionary<int, string>.KeyCollection keys)
        {
            int[] tmp = new int[keys.Count];
            keys.CopyTo(tmp, 0);
            List<int> ret = new List<int>(tmp);
            ret.Sort();
            return ret;
        }

        private static bool IsNumeric(object obj){
            try{
                Decimal.Parse(obj.ToString());
            }catch(Exception e){
                return false;
            }
            return true;
        }
        
        public static string GenerateStringFromObject(object obj)
        {
        	return GenerateStringFromObject(obj,null);
        }

        public static string GenerateStringFromObject(object obj,string seperator)
        {
            string ret = "";
            if (obj == null)
                return null;
            if (obj is string)
                return obj.ToString();
            if (obj is IDictionary)
            {
                IDictionaryEnumerator e = ((IDictionary)obj).GetEnumerator();
                while (e.MoveNext())
                {
                    ret += GenerateStringFromObject(e.Key,seperator);
                    ret += "-->";
                    ret += GenerateStringFromObject(e.Value,seperator);
                    ret += ", ";
                }
                if (ret.Length > 0)
                    ret = ret.Substring(0, ret.Length - 2);
            }else if (obj is ArrayList)
            {
            	foreach (object o in (ArrayList)obj){
                    ret += GenerateStringFromObject(o,seperator);
                    if (seperator!=null)
                    	ret+=seperator;
            	}
            	if (seperator!=null)
            		ret = ret.Substring(0,ret.Length-seperator.Length);
            }
            else if (obj.GetType().IsArray || (obj is IEnumerable))
            {
            	foreach (object o in (IEnumerable)obj){
                    ret += GenerateStringFromObject(o,seperator);
                    if (seperator!=null)
                    	ret+=seperator;
            	}
            	if (seperator!=null)
            		ret = ret.Substring(0,ret.Length-seperator.Length);
            }
            else
                ret = obj.ToString();
            return ret;
        }

        public static bool IsComponentTrue(string retValue)
        {
            if ((retValue == null)||(retValue=="0")||(retValue.ToUpper()=="FALSE")||(retValue.Length==0))
                return false;
            return true;
        }

        public static object LocateObjectInVariables(string variableName,Dictionary<string,object> variables){
            object obj = null;
            if (variableName.Contains("."))
            {
                if (variables.ContainsKey(variableName.Substring(0, variableName.IndexOf("."))))
                    obj = RecurLocateValue(variables[variableName.Substring(0, variableName.IndexOf("."))], variableName.Substring(variableName.IndexOf(".") + 1));
            }
            else
            {
                if (variables.ContainsKey(variableName))
                    obj = variables[variableName];
            }
            return obj;
        }

        private static object RecurLocateValue(object val, string varName)
        {
            if (val == null)
                return null;
            object ret = null;
            if (varName.Contains("."))
            {
                if (val is IDictionary)
                {
                    if (((IDictionary)val).Contains(varName.Substring(0, varName.IndexOf("."))))
                        ret = RecurLocateValue(((IDictionary)val)[varName.Substring(0, varName.IndexOf("."))], varName.Substring(varName.IndexOf(".")+1));
                }
                else if (val.GetType().GetProperty(varName.Substring(0, varName.IndexOf("."))) != null)
                    ret = RecurLocateValue(val.GetType().GetProperty(varName.Substring(0, varName.IndexOf("."))).GetValue(val, new object[0]), varName.Substring(varName.IndexOf(".")+1));
                else if (val.GetType().GetMethod(varName.Substring(0, varName.IndexOf(".")), Type.EmptyTypes) != null)
                    ret = RecurLocateValue(val.GetType().GetMethod(varName.Substring(0, varName.IndexOf(".")), Type.EmptyTypes).Invoke(val, new object[0]), varName.Substring(varName.IndexOf(".")+1));
            }
            else
            {
                if (val is IDictionary)
                {
                    if (((IDictionary)val).Contains(varName))
                        ret = ((IDictionary)val)[varName];
                }
                else if (val.GetType().GetProperty(varName) != null)
                    ret = val.GetType().GetProperty(varName).GetValue(val, new object[0]);
                else if (val.GetType().GetMethod(varName, Type.EmptyTypes) != null)
                    ret = val.GetType().GetMethod(varName, Type.EmptyTypes).Invoke(val, new object[0]);
            }
            return ret;
        }
	}
}
