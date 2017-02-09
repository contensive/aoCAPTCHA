using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;

namespace Contensive.addons.aoRecaptcha
{
    //
    // 1) Change the namespace to the collection name
    // 2) Change this class name to the addon name
    // 3) Create a Contensive Addon record with the namespace apCollectionName.ad
    // 3) add reference to CPBase.DLL, typically installed in c:\program files\kma\contensive\
    //
    public class displayClass : Contensive.BaseClasses.AddonBaseClass
    {
        //
        // execute method is the only public
        //
        public override object Execute(CPBaseClass cp)
        {
            string publicKey;
            try
            {
                //
                appendDebug(cp, "Contensive.addons.aoRecaptcha.displayClass.execute()-enter");
                //
                CPCSBaseClass cs = cp.CSNew();
                publicKey = cp.Site.GetText(redCaptchPrivateKey, parameter);
                if (string.IsNullOrEmpty(publicKey))
                {
                    cp.Site.SetProperty(redCaptchPrivateKey, parameter);
                    publicKey = cp.Site.GetText(redCaptchPrivateKey, parameter);
                }

                if (cp.Request.Secure)
                {
                    stream.Append("<script type='text/javascript'>");
                    stream.AppendLine("var RecaptchaOptions = {");
                    stream.Append("   theme : 'white',");
                    stream.Append("   tabindex : 0");
                    stream.Append("};");
                    stream.AppendLine("</script>");
                    stream.AppendLine(string.Format("<script type='text/javascript' src='https://api.recaptcha.net/challenge?k={0}'></script>", publicKey));
                    stream.AppendLine("<noscript>");
                    stream.AppendLine(string.Format("<iframe src='https://api.recaptcha.net/noscript?k={0}' frameborder='1'></iframe><br>", publicKey));
                    stream.AppendLine("<textarea name='recaptcha_challenge_field' rows='3' cols='40'></textarea>");
                    stream.AppendLine("<input type='hidden' name='recaptcha_response_field' value='manual_challenge'>");
                    stream.AppendLine("</noscript>");
                }
                else
                {
                    stream.Append("<script type='text/javascript'>");
                    stream.AppendLine("var RecaptchaOptions = {");
                    stream.Append("   theme : 'white',");
                    stream.Append("   tabindex : 0");
                    stream.Append("};");
                    stream.AppendLine("</script>");
                    stream.AppendLine(string.Format("<script type='text/javascript' src='http://api.recaptcha.net/challenge?k={0}'></script>", publicKey));
                    stream.AppendLine("<noscript>");
                    stream.AppendLine(string.Format("<iframe src='http://api.recaptcha.net/noscript?k={0}' frameborder='1'></iframe><br>", publicKey));
                    stream.AppendLine("<textarea name='recaptcha_challenge_field' rows='3' cols='40'></textarea>");
                    stream.AppendLine("<input type='hidden' name='recaptcha_response_field' value='manual_challenge'>");
                    stream.AppendLine("</noscript>");
                }
                stream.AppendLine(cp.Html.Hidden("prcs", "1"));
                //
                appendDebug(cp, "Contensive.addons.aoRecaptcha.displayClass.execute()-exit");
                //
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex, "Unexpeced trap");
            }
            return stream.ToString();
        }
        //
        private void appendDebug(CPBaseClass cp, string logMsg)
        {
            //cp.Utils.AppendLog("aoCaptcha.log", logMsg);
        }
        //
        StringBuilder stream = new StringBuilder();
        private const string redCaptchPrivateKey = "reCAPTCHA Public Key";
        private const string parameter = "6Ld_AgYAAAAAALQ66lAKd0-_YOKY3EVHQt0i0tQJ";
    }
}
