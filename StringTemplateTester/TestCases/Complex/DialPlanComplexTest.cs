/*
 * Created by SharpDevelop.
 * User: Roger
 * Date: 27/01/2010
 * Time: 9:08 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using Org.Reddragonit.Stringtemplate;
using System.Text.RegularExpressions;


namespace StringTemplateTester.TestCases.Complex
{
	/// <summary>
	/// Description of DialPlanComplexTest.
	/// </summary>
	public class DialPlanComplexTest : ITest 
	{
		private const string TemplateCode = "<extension name=\"conference_$intercom_extension$\">\n" +
        " <condition field=\"destination_number\" expression=\"^$intercom_extension$\\$\" break=\"never\">\n" +
        " <action application=\"set\" data=\"api_hangup_hook=conference $intercom_extension$ kick all\"/>\n" +
        " <action application=\"answer\"/>\n" +
        " <action application=\"export\" data=\"sip_invite_params=intercom=true\"/>\n" +
        " <action application=\"export\" data=\"sip_auto_answer=true\"/>\n" +
        " <action application=\"set\" data=\"conference_auto_outcall_caller_id_name=\\$\\${effective_caller_id_name}\"/>\n" +
        " <action application=\"set\" data=\"conference_auto_outcall_caller_id_number=\\$\\${effective_caller_id_number}\"/>\n" +
        " <action application=\"set\" data=\"conference_auto_outcall_timeout=5\"/>\n" +
        "$if(intercom_only)$"+
        "<action application=\"set\" data=\"conference_auto_outcall_flags=mute\"/>"+
        "$else$"+
        "<action application=\"set\" data=\"conference_auto_outcall_flags=none\"/>" +
        "$endif$"+
        "\n" +
        " </condition>\n" +
        " $extensions:{extension | "+
        " <condition field=\"caller_id_number\" expression=\"^$extension$\\$\" break=\"never\">\n" +
        " $extensions:{ext | "+
        " $if((extension ne ext))$"+
        " <action application=\"conference_set_auto_outcall\" data=\"user/$ext$@\\$\\${domain}\"/>\n"+
        " $endif$"+
        " }$"+
        " </condition>\n"+
        " }$"+
        " <condition field=\"caller_id_number\" expression=\"^($extensions; separator=\"|\"$)\\$\" break=\"never\">\n"+
        " $extensions:{extension | "+
        " <anti-action application=\"conference_set_auto_outcall\" data=\"user/$extension$@\\$\\${domain}\"/>\n"+
        " }$"+
        " </condition>\n"+
        " <condition field=\"destination_number\" expression=\"^$intercom_extension$\\$\">\n" +
        " <action application=\"conference\" data=\"$intercom_extension$@intercom\"/>\n" +
        " <action application=\"conference\" data=\"$intercom_extension$ kick all\"/>\n" +
        " </condition>\n" +
        " </extension>";

        private const string TemplateResult = "<extension name=\"conference_1003\">\n"+
 " <condition field=\"destination_number\" expression=\"^1003$\" break=\"never\">\n" +
 " <action application=\"set\" data=\"api_hangup_hook=conference 1003 kick all\"/>\n"+
 " <action application=\"answer\"/>\n"+
 " <action application=\"export\" data=\"sip_invite_params=intercom=true\"/>\n"+
 " <action application=\"export\" data=\"sip_auto_answer=true\"/>\n"+
 " <action application=\"set\" data=\"conference_auto_outcall_caller_id_name=$${effective_caller_id_name}\"/>\n"+
 " <action application=\"set\" data=\"conference_auto_outcall_caller_id_number=$${effective_caller_id_number}\"/>\n"+
 " <action application=\"set\" data=\"conference_auto_outcall_timeout=5\"/>\n"+
" <action application=\"set\" data=\"conference_auto_outcall_flags=mute\"/>\n"+
 " </condition>\n"+
   " <condition field=\"caller_id_number\" expression=\"^1001$\" break=\"never\">\n"+
       " <action application=\"conference_set_auto_outcall\" data=\"user/1002@$${domain}\"/>\n"+
   " </condition>\n"+
   " <condition field=\"caller_id_number\" expression=\"^1002$\" break=\"never\">\n"+
    " <action application=\"conference_set_auto_outcall\" data=\"user/1001@$${domain}\"/>\n"+
      " </condition>\n"+
  " <condition field=\"caller_id_number\" expression=\"^(1001|1002)$\" break=\"never\">\n"+
   " <anti-action application=\"conference_set_auto_outcall\" data=\"user/1001@$${domain}\"/>\n"+
   " <anti-action application=\"conference_set_auto_outcall\" data=\"user/1002@$${domain}\"/>\n"+
  " </condition>\n"+
 " <condition field=\"destination_number\" expression=\"^1003$\">\n"+
 " <action application=\"conference\" data=\"1003@intercom\"/>\n"+
 " <action application=\"conference\" data=\"1003 kick all\"/>\n"+
 " </condition>\n"+
 " </extension>";
		
		public DialPlanComplexTest()
		{
		}
		
		public string Name {
			get {
				return "Dial Plan Complex Test";
			}
		}

        public static bool StringsEqualIgnoreWhitespace(string str1, string str2)
        {
            if (str1 == null)
            {
                if (str2 != null)
                    return false;
                return true;
            }
            else if (str2 == null)
            {
                return false;
            }
            Regex r = new Regex("\\s+");
            return r.Replace(str1, "").Equals(r.Replace(str2, ""));
        }

		public bool InvokeTest()
		{
			Template tp = new Template(TemplateCode);
			
			tp.SetParameter("intercom_extension","1003");
			tp.SetParameter("extensions",new string[]{"1001","1002"});
			tp.SetParameter("intercom_only",true);

            if (!StringsEqualIgnoreWhitespace(tp.ToString(), TemplateResult))
            {
                Console.WriteLine("Complex dialplan check failed with result: " + tp.ToString());
                return false;
            }

            tp = new Template(Program.ReadEmbeddedResource("StringTemplateTester.resources.ExtensionConfig.st"));
            System.Diagnostics.Debug.WriteLine(tp.ToString());
			
			return true;
		}
	}
}
