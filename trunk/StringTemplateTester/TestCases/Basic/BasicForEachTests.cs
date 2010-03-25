using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Stringtemplate;


namespace StringTemplateTester.TestCases.Basic
{
    public class BasicForEachTests : ITest 
    {
        #region ITest Members

        public string Name
        {
            get { return "Basic foreach"; }
        }

        public bool InvokeTest()
        {
            //basic forloop
            Template tp = new Template("$entries:{entry |$entry$\n}$");
            tp.SetParameter("entries", new string[] { "line1", "line2", "line3" });
            if (tp.ToString() != "line1\nline2\nline3\n")
            {
                Console.WriteLine("Basic foreach loop test failed with results: " + tp.ToString());
                return false;
            }

            //basic dictionary forloop
            tp = new Template("$entries:{entry|$entry.Key$=$entry.Value$\n}$");
            Dictionary<string, string> vars = new Dictionary<string, string>();
            vars.Add("FirstName", "Bob");
            vars.Add("LastName", "LobLaw");
            tp.SetParameter("entries", vars);
            if (tp.ToString() != "FirstName=Bob\nLastName=LobLaw\n")
            {
                Console.WriteLine("Basic foreach using dictionary test failed with results: " + tp.ToString());
                return false;
            }

            //dictionary array test
            tp = new Template("$entries:{entry |$entry.FirstName$ $entry.LastName$\n}$");
            List<Dictionary<string, string>> arVars = new List<Dictionary<string, string>>();
            arVars.Add(vars);
            vars = new Dictionary<string, string>();
            vars.Add("FirstName", "Amanda");
            vars.Add("LastName", "Hugnkiss");
            arVars.Add(vars);
            tp.SetParameter("entries", arVars);
            if (tp.ToString() != "Bob LobLaw\nAmanda Hugnkiss\n")
            {
                Console.WriteLine("Basic foreach using array of dictionaries failed with results: " + tp.ToString());
                return false;
            }

            tp = new Template(Program.ReadEmbeddedResource("StringTemplateTester.resources.test.st"));
            arVars = new List<Dictionary<string, string>>();
            Dictionary<string, string> file = new Dictionary<string, string>();
            file.Add("Name", "MyFile");
            file.Add("JScriptName", "MyFile");
            arVars.Add(file);
            file = new Dictionary<string, string>();
            file.Add("Name", "My Name's");
            file.Add("JScriptName", "My Name\\'s");
            arVars.Add(file);
            tp.SetAttribute("files", arVars);
            System.Diagnostics.Debug.WriteLine(tp.ToString());

            return true;
        }

        #endregion
    }
}

