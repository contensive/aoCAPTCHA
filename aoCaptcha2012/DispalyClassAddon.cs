using Contensive.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace DisplayClass
{
    public class DispalyClassAddon : AddonBaseClass
    {

        public override object Execute(CPBaseClass cp)
        {
            string publicKey;
            try
            {
                //stream.Append("From the displayClass");
                CPCSBaseClass cs = cp.CSNew();
                publicKey = cp.Site.GetText(redCaptchPrivateKey, parameter);
                if (string.IsNullOrEmpty(publicKey))
                {
                    cp.Site.SetProperty(redCaptchPrivateKey, parameter);
                    publicKey = cp.Site.GetText(redCaptchPrivateKey, parameter);
                }

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
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex, "Unexpeced trap");
            }
            return stream.ToString();
        }


        
        StringBuilder stream = new StringBuilder();
        private const string redCaptchPrivateKey = "reCAPTCHA Public Key";
        private const string parameter = "6Ld_AgYAAAAAALQ66lAKd0-_YOKY3EVHQt0i0tQJ";
    }
}
