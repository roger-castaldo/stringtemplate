using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Stringtemplate;


namespace StringTemplateTester.TestCases.Basic
{
    class BasicIfWriteTests : ITest
    {

        #region ITest Members

        public string Name
        {
            get { return "If Tests"; }
        }

        public bool InvokeTest()
        {
            Template tp = new Template("$if(bob)$Hello Bob$elseif(fred)$Hello Fred$else$GET OUT STRANGER DANGER$endif$");

            //testing basic if
            tp.SetParameter("bob", true);
            if (tp.ToString() != "Hello Bob")
            {
                Console.WriteLine("Basic If statement test failed with result: " + tp.ToString());
                return false;
            }

            //testing else if 
            tp.RemoveParameter("bob");
            tp.SetParameter("bob", false);
            tp.SetParameter("fred", true);
            if (tp.ToString() != "Hello Fred")
            {
                Console.WriteLine("Basic elseif statement test failed with result: " + tp.ToString());
                return false;
            }

            //testing else using nulls
            tp.ClearParameters();
            if (tp.ToString() != "GET OUT STRANGER DANGER")
            {
                Console.WriteLine("Basic else statement test failed with result: " + tp.ToString());
                return false;
            }

            //testing using complex parameter
            tp = new Template("$if(bob.IsHere)$YAY I found bob$else$BOO bob is dead$endif$");
            Dictionary<string, object> vars = new Dictionary<string, object>();
            vars.Add("IsHere", true);
            tp.SetParameter("bob", vars);
            if (tp.ToString() != "YAY I found bob")
            {
                Console.WriteLine("Basic if using complex parameter failed with result: " + tp.ToString());
                return false;
            }

            //testing compare in if statement
            tp = new Template("$if(equal(bob,fred))$Bob is Fred$else$Bob is not Fred$endif$");
            tp.SetParameter("bob", "bob");
            tp.SetParameter("fred", "bob");
            if (tp.ToString() != "Bob is Fred")
            {
                Console.WriteLine("Basic if test using equal function inside failed with result: " + tp.ToString());
                return false;
            }

            //testing not compare in if statement
            tp = new Template("$if(notequal(bob,fred))$Bob is Fred$else$Bob is not Fred$endif$");
            tp.SetParameter("bob", "bob");
            tp.SetParameter("fred", "bob");
            if (tp.ToString() != "Bob is not Fred")
            {
                Console.WriteLine("Basic if test using notequal function inside failed with result: " + tp.ToString());
                return false;
            }
            
            //testing compare alt format in if statement
            tp = new Template("$if((bob eq fred))$Bob is Fred$else$Bob is not Fred$endif$");
            tp.SetParameter("bob", "bob");
            tp.SetParameter("fred", "bob");
            if (tp.ToString() != "Bob is Fred")
            {
                Console.WriteLine("Basic if test using equal function inside failed with result: " + tp.ToString());
                return false;
            }

            //testing odd function
            tp = new Template("$if(odd(world))$OddBall$else$NotWeird$endif$");
            tp.SetAttribute("world", "1");
            Console.WriteLine(tp.ToString());
            tp.RemoveParameter("world");
            tp.SetAttribute("world", "hello");
            Console.WriteLine(tp.ToString());
            tp.RemoveParameter("world");
            tp.SetAttribute("world", "2");
            Console.WriteLine(tp.ToString());

            return true;
        }

        #endregion
    }
}
