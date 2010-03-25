using System;
using System.Collections.Generic;
using System.Text;

namespace StringTemplateTester
{
    interface ITest
    {
        string Name { get; }
        bool InvokeTest();
    }
}
