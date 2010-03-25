using System;
using System.Collections.Generic;
using System.Text;

using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using Org.Reddragonit.Stringtemplate;


namespace StringTemplateTester
{
    class Program
    {
        internal static TemplateGroup TestGroup;

        static void Main(string[] args)
        {
            string groupContent = ReadEmbeddedResource("StringTemplateTester.resources.main.stg");
            TestGroup = new TemplateGroup(groupContent);

            List<ITest> cases = new List<ITest>();
            int cntPass = 0;
            int cntFail = 0;
            Console.WriteLine("Loading All test cases...");
            foreach (Type t in Utility.LocateTypeInstances(typeof(ITest),typeof(Program).Assembly))
                cases.Add((ITest)t.GetConstructor(Type.EmptyTypes).Invoke(new object[0]));
            Console.WriteLine("Loading test cases complete...Executing now");
            foreach (ITest test in cases)
            {
                Console.WriteLine("Executing test case " + test.Name + "...");
                if (test.InvokeTest())
                {
                    Console.WriteLine("Test case " + test.Name + " passed successfully");
                    cntPass++;
                }
                else
                {
                    Console.WriteLine("Test case " + test.Name + " failed");
                    cntFail++;
                }
            }
            Console.WriteLine("Test case results:");
            Console.WriteLine("Passes: " + cntPass.ToString());
            Console.WriteLine("Fails: " + cntFail.ToString());
            Console.WriteLine("Total cases: " + cases.Count.ToString());
            Console.WriteLine("Hit enter to exit...");
            Console.ReadLine();
        }

        public static Stream LocateEmbededResource(string name)
        {
            Stream ret = typeof(Utility).Assembly.GetManifestResourceStream(name);
            if (ret == null)
            {
                foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        if (!ass.GetName().Name.Contains("mscorlib") && !ass.GetName().Name.StartsWith("System") && !ass.GetName().Name.StartsWith("Microsoft"))
                        {
                            ret = ass.GetManifestResourceStream(name);
                            if (ret != null)
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
            return ret;
        }

        public static string ReadEmbeddedResource(string name)
        {
            Stream s = LocateEmbededResource(name);
            string ret = "";
            if (s != null)
            {
                TextReader tr = new StreamReader(s);
                ret = tr.ReadToEnd();
                tr.Close();
            }
            return ret;
        }
    }
}
