using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Stringtemplate;

namespace StringTemplateTester.TestCases.Basic
{
    public class BasicFormatTests : ITest 
    {
        #region ITest Members

        public string Name
        {
            get { return "Basic Format Tests"; }
        }

        public bool InvokeTest()
        {
            Template t = new Template("$format(mydate,dd-MMM-yyyy HH:mm:ss)$");
            DateTime tmp = DateTime.Now;
            t.SetAttribute("mydate", tmp);
            if (t.ToString() != tmp.ToString("dd-MMM-yyyy HH:mm:ss"))
            {
                Console.WriteLine("Failed basic format test using a datetime variable with result: " + t.ToString());
                return false;
            }

            t = new Template("$format(myNumber,'#,###.00')$");
            t.SetAttribute("myNumber", 9999.99);
            if (t.ToString() != ((double)9999.99).ToString("#,###.00"))
            {
                Console.WriteLine("Failed basic format test using a double variable with result: "+t.ToString());
                return false;
            }

            return true;
        }

        #endregion
    }
}
