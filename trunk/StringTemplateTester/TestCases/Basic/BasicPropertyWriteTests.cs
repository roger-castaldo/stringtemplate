using System;
using System.Collections.Generic;
using System.Text;

using System.Collections;
using Org.Reddragonit.Stringtemplate;

namespace StringTemplateTester.TestCases.Basic
{
    class teir1
    {
        private teir2 _child;
        public teir2 Child{
            get{return _child;}
            set{_child=value;}
        }

        public teir1(teir2 child){
            _child=child;
        }
    }

    class teir2{
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public teir2(string name)
        {
            _name = name;
        }
    }

    class BasicPropertyWriteTests : ITest 
    {




        #region ITest Members

        public string Name
        {
            get { return "Basic Property Tests"; }
        }

        public bool InvokeTest()
        {
            //testing string variable
            Template tp = new Template("This is my string: $mystring$");
            tp.SetParameter("mystring", "Hello world");
            if (tp.ToString() != "This is my string: Hello world")
            {
                Console.WriteLine("Basic string test failed with results: "+tp.ToString());
                return false;
            }

            //testing string array
            tp = new Template("This is my string array: $mystringarray$");
            tp.SetParameter("mystringarray", new string[] { "Bob", "Lob", "Law" });
            if (tp.ToString() != "This is my string array: BobLobLaw")
            {
                Console.WriteLine("Basic string array test failed with results: " + tp.ToString());
                return false;
            }

            //testing List<string> object
            tp.ClearParameters();
            tp.SetParameter("mystringarray", new List<string>(new string[] { "Bob", "Lob", "Law" }));
            if (tp.ToString() != "This is my string array: BobLobLaw")
            {
                Console.WriteLine("Basic List<string> array test failed with results: " + tp.ToString());
                return false;
            }

            //testing Hashtable object
            tp = new Template("This is my hashtable: $myhashtable$");
            Hashtable ht = new Hashtable();
            ht.Add("FirstName", "Bob");
            ht.Add("LastName", "LobLaw");
            tp.SetParameter("myhashtable", ht);
            if (tp.ToString() != "This is my hashtable: LastName-->LobLaw, FirstName-->Bob")
            {
                Console.WriteLine("Basic hashtable test failed with results: " + tp.ToString());
                return false;
            }

            //testing Dictionary object
            tp = new Template("This is my dictionary: $mydictionary$");
            Dictionary<string, string> dc = new Dictionary<string, string>();
            dc.Add("FirstName", "Bob");
            dc.Add("LastName", "LobLaw");
            tp.SetParameter("mydictionary",dc);
            if (tp.ToString() != "This is my dictionary: FirstName-->Bob, LastName-->LobLaw")
            {
                Console.WriteLine("Basic Dictionary test failed with results: " + tp.ToString());
                return false;
            }

            //testing Complex Dictionary
            tp = new Template("This is the subelement $mydictionary.name1.FirstName$");
            Dictionary<string, Dictionary<string, string>> cdc = new Dictionary<string, Dictionary<string, string>>();
            cdc.Add("name1", dc);
            tp.SetAttribute("mydictionary", cdc);
            if (tp.ToString() != "This is the subelement Bob")
            {
                Console.Write("Complex Dictionary test failed with results: " + tp.ToString());
                return false;
            }

            //testing Complex Class Dictionary
            tp = new Template("This is the subelement $myteir.Child.Name$");
            tp.SetAttribute("myteir", new teir1(new teir2("Bob")));
            if (tp.ToString() != "This is the subelement Bob")
            {
                Console.Write("Complex Dictionary test failed with results: " + tp.ToString());
                return false;
            }

            //testing access to single dictionary object
            tp = new Template("This is my dictionaries first name value: $mydictionary.FirstName$");
            tp.SetParameter("mydictionary", dc);
            if (tp.ToString() != "This is my dictionaries first name value: Bob")
            {
                Console.WriteLine("Basic Dictionary accessing single value test failed with results: " + tp.ToString());
                return false;
            }

            //testing escape character
            tp = new Template("This is my escape character test worth \\$$money$");
            tp.SetParameter("money", 1000);
            if (tp.ToString() != "This is my escape character test worth $1000")
            {
                Console.WriteLine("Basic escape character test failed with results: " + tp.ToString());
                return false;
            }

            return true;
        }

        #endregion
    }
}
