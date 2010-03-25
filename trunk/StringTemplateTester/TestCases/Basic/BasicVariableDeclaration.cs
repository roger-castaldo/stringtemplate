using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Stringtemplate;

namespace StringTemplateTester.TestCases.Basic
{
    public class Name
    {
        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
        }

        private string _lastName;
        public string LastName
        {
            get { return _lastName; }
        }

        public Name(string firstName, string lastName)
        {
            _firstName = firstName;
            _lastName = lastName;
        }

        public static Name ConstructNew(string firstName, string lastName)
        {
            return new Name(firstName, lastName);
        }
    }

    public class BasicVariableDeclaration : ITest
    {
        #region ITest Members

        string ITest.Name
        {
            get { return "Basic Variable Declaration"; }
        }

        bool ITest.InvokeTest()
        {
            Template t = new Template("$DECLARE name as String = \"bob\"$$name$");
            if (t.ToString() != "bob")
            {
                Console.WriteLine("Testing basic variable failed with results: " + t.ToString());
                return false;
            }

            t = new Template("$DECLARE names as String[] = {\"bob\",\"fred\",$jack$}$$names:{name|$name$,}$");
            t.SetAttribute("jack","jack");
            if (t.ToString() != "bob,fred,jack,")
            {
                Console.WriteLine("Testing basic array declaration with variable failed with results: " + t.ToString());
                return false;
            }

            t = new Template("$DECLARE myname as StringTemplateTester.TestCases.Basic.Name = StringTemplateTester.TestCases.Basic.Name.ConstructNew($bob$,$jack$)$$myname.FirstName$,$myname.LastName$");
            t.SetAttribute("jack", "jack");
            t.SetAttribute("bob", "bob");
            if (t.ToString() != "bob,jack")
            {
                Console.WriteLine("Testing basic static call with variable failed with results: " + t.ToString());
                return false;
            }

            t = new Template("$DECLARE myname as StringTemplateTester.TestCases.Basic.Name = NEW StringTemplateTester.TestCases.Basic.Name($bob$,$jack$)$$myname.FirstName$,$myname.LastName$");
            t.SetAttribute("jack", "jack");
            t.SetAttribute("bob", "bob");
            if (t.ToString() != "bob,jack")
            {
                Console.WriteLine("Testing basic constructor call with variable failed with results: " + t.ToString());
                return false;
            }
            return true;
        }

        #endregion
    }
}
