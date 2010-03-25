using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Stringtemplate;


namespace StringTemplateTester.TestCases.Complex
{
    class MenuEntry
    {
        private string _title;
        public string Title
        {
            get { return _title; }
        }

        private MenuEntry[] _children;
        public MenuEntry[] Children
        {
            get { return _children; }
        }

        public MenuEntry(string title, MenuEntry[] childs)
        {
            _title = title;
            _children = childs;
        }
    }

    class TemplateGroupTest : ITest 
    {
        #region ITest Members

        public string Name
        {
            get { return "Template Group Test"; }
        }

        public bool InvokeTest()
        {
            //test non parametered call
            Template tp = new Template("$bob:CreateJavascriptElem()$",Program.TestGroup);
            tp.SetParameter("bob", "Hello World");
            if (tp.ToString() != "'Hello World'")
            {
                Console.WriteLine("The basic template group code test failed with results: " + tp.ToString());
                return false;
            }

            //test parametered call by defining variables in full
            tp = new Template("$GenerateLargeForm(content=con,id=ID,title=TITLE,OnClose=Clos,OnMinimize=Min,scriptID=SCID)$", Program.TestGroup);
            tp.SetParameter("con", "TEST CONTENT");
            tp.SetParameter("TITLE", "TESTING");
            tp.SetParameter("SCID", "'test'");
            tp.SetParameter("ID", "test");
            if (tp.ToString() != "<div id=\"test\" onBlur=\"UnHighlightTaskBar('test');\" class=\"Form transparent\" style=\"max-width:800px;max-height:600px;\"><div class=\"tl\"><div class=\"tr\"><div class=\"ControlButtons\"><img src=\"./FormParts/minimize.png\" alt=\"minimize\" onclick=\"MinimizeForm('test');\"><img src=\"./FormParts/cancel.png\" alt=\"close\" onclick=\"CloseForm('test');\"></div></div><div class=\"top\" id=\"test_top\"><center>TESTING</center></div></div><div class=\"left\"><div class=\"right\"><div style=\"overflow:auto;max-width:794px;max-height:552px;background-color:White;padding:5px;color:Black;\">TEST CONTENT</div></div></div><div class=\"bl\"><div class=\"br\"></div><div class=\"bottom\"><div class=\"status\"><div id=\"test_status\" class=\"statusContainer\"></div></div><div style=\"clear:right;\"></div></div></div></div>")
            {
                Console.WriteLine("The basic template group code test failed with results: " + tp.ToString());
                return false;
            }

            //test parametered call without defining variables in full
            tp = new Template("$GenerateLargeForm(con,ID,TITLE,Clos,Min,SCID)$", Program.TestGroup);
            tp.SetParameter("con", "TEST CONTENT");
            tp.SetParameter("TITLE", "TESTING");
            tp.SetParameter("SCID", "'test'");
            tp.SetParameter("ID", "test");
            if (tp.ToString() != "<div id=\"test\" onBlur=\"UnHighlightTaskBar('test');\" class=\"Form transparent\" style=\"max-width:800px;max-height:600px;\"><div class=\"tl\"><div class=\"tr\"><div class=\"ControlButtons\"><img src=\"./FormParts/minimize.png\" alt=\"minimize\" onclick=\"MinimizeForm('test');\"><img src=\"./FormParts/cancel.png\" alt=\"close\" onclick=\"CloseForm('test');\"></div></div><div class=\"top\" id=\"test_top\"><center>TESTING</center></div></div><div class=\"left\"><div class=\"right\"><div style=\"overflow:auto;max-width:794px;max-height:552px;background-color:White;padding:5px;color:Black;\">TEST CONTENT</div></div></div><div class=\"bl\"><div class=\"br\"></div><div class=\"bottom\"><div class=\"status\"><div id=\"test_status\" class=\"statusContainer\"></div></div><div style=\"clear:right;\"></div></div></div></div>")
            {
                Console.WriteLine("The basic template group code test failed with results: " + tp.ToString());
                return false;
            }

            //test parametered call without defining variables in full and specifying group prefix
            tp = new Template("$skin/GenerateLargeForm(con,ID,TITLE,Clos,Min,SCID)$", Program.TestGroup);
            tp.SetParameter("con", "TEST CONTENT");
            tp.SetParameter("TITLE", "TESTING");
            tp.SetParameter("SCID", "'test'");
            tp.SetParameter("ID", "test");
            if (tp.ToString() != "<div id=\"test\" onBlur=\"UnHighlightTaskBar('test');\" class=\"Form transparent\" style=\"max-width:800px;max-height:600px;\"><div class=\"tl\"><div class=\"tr\"><div class=\"ControlButtons\"><img src=\"./FormParts/minimize.png\" alt=\"minimize\" onclick=\"MinimizeForm('test');\"><img src=\"./FormParts/cancel.png\" alt=\"close\" onclick=\"CloseForm('test');\"></div></div><div class=\"top\" id=\"test_top\"><center>TESTING</center></div></div><div class=\"left\"><div class=\"right\"><div style=\"overflow:auto;max-width:794px;max-height:552px;background-color:White;padding:5px;color:Black;\">TEST CONTENT</div></div></div><div class=\"bl\"><div class=\"br\"></div><div class=\"bottom\"><div class=\"status\"><div id=\"test_status\" class=\"statusContainer\"></div></div><div style=\"clear:right;\"></div></div></div></div>")
            {
                Console.WriteLine("The basic template group code test failed with results: " + tp.ToString());
                return false;
            }

            //test parametered call without defining variables in full and specifying group prefix
            tp = new Template("$skin/GenerateLargeForm(con,ID,TITLE,Clos,Min,SCID):CreateJavascriptElem()$", Program.TestGroup);
            tp.SetParameter("con", "TEST CONTENT");
            tp.SetParameter("TITLE", "TESTING");
            tp.SetParameter("SCID", "'test'");
            tp.SetParameter("ID", "test");
            if (tp.ToString() != "'<div id=\"test\" onBlur=\"UnHighlightTaskBar('test');\" class=\"Form transparent\" style=\"max-width:800px;max-height:600px;\"><div class=\"tl\"><div class=\"tr\"><div class=\"ControlButtons\"><img src=\"./FormParts/minimize.png\" alt=\"minimize\" onclick=\"MinimizeForm('test');\"><img src=\"./FormParts/cancel.png\" alt=\"close\" onclick=\"CloseForm('test');\"></div></div><div class=\"top\" id=\"test_top\"><center>TESTING</center></div></div><div class=\"left\"><div class=\"right\"><div style=\"overflow:auto;max-width:794px;max-height:552px;background-color:White;padding:5px;color:Black;\">TEST CONTENT</div></div></div><div class=\"bl\"><div class=\"br\"></div><div class=\"bottom\"><div class=\"status\"><div id=\"test_status\" class=\"statusContainer\"></div></div><div style=\"clear:right;\"></div></div></div></div>'")
            {
                Console.WriteLine("The basic template group code test failed with results: " + tp.ToString());
                return false;
            }

            //testing recur calling template in string template group
            tp = new Template("$GenerateMenu(items)$", Program.TestGroup);
            List<MenuEntry> items = new List<MenuEntry>();
            items.Add(new MenuEntry("test1",
                new MenuEntry[]{
                    new MenuEntry("test1.1",null),
                    new MenuEntry("test1.2",null)
                }));
            items.Add(new MenuEntry("test2",
                            new MenuEntry[]{
                    new MenuEntry("test2.1",null),
                    new MenuEntry("test2.2",null)
                }));
            tp.SetAttribute("items", items);

            System.Diagnostics.Debug.WriteLine(tp.ToString());

            return true;
        }

        #endregion
    }
}
