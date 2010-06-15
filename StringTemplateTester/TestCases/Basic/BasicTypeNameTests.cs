using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Stringtemplate;

namespace StringTemplateTester.TestCases.Basic
{
    public class BasicTypeNameTests : ITest
    {
        #region ITest Members

        string ITest.Name
        {
            get { return "BasicTypeNameTests"; }
        }

        bool ITest.InvokeTest()
        {
            Template tp = new Template("$TypeName(fred)$");
            tp.SetAttribute("fred", new BasicTypeNameTests());
            if (tp.ToString() != "StringTemplateTester.TestCases.Basic.BasicTypeNameTests")
            {
                Console.WriteLine("Basic type name test failed with result: " + tp.ToString());
                return false;
            }

            tp = new Template("$(TypeName(fred) eq basicTypeString)$");
            tp.SetAttribute("fred", new BasicTypeNameTests());
            tp.SetAttribute("basicTypeString", "StringTemplateTester.TestCases.Basic.BasicTypeNameTests");
            if (tp.ToString() != "True")
            {
                Console.WriteLine("Basic type name test using equal failed with result: " + tp.ToString());
                return false;
            }

            tp = new Template("$if((TypeName(fred) eq basicTypeString))$Yes it is$else$No its not$endif$");
            tp.SetAttribute("fred", new BasicTypeNameTests());
            tp.SetAttribute("basicTypeString", "StringTemplateTester.TestCases.Basic.BasicReplaceTest");
            if (tp.ToString() != "No its not")
            {
                Console.WriteLine("Basic type name test using equal if failed with result: " + tp.ToString());
                return false;
            }

            return true;
        }

        #endregion
    }
}
