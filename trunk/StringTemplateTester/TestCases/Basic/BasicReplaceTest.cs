using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Stringtemplate;

namespace StringTemplateTester.TestCases.Basic
{
    class BasicReplaceTest : ITest
    {
        #region ITest Members

        public string Name
        {
            get { return "Basic Replace Test"; }
        }

        public bool InvokeTest()
        {
            Template tp = new Template("$replace($hello$,',\\')$");
            tp.SetAttribute("hello", "Hi y'all");
            Console.WriteLine(tp.ToString());


            return true;
        }

        #endregion
    }
}
