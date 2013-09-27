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
    public class reCaptchaClass : Contensive.BaseClasses.AddonBaseClass
    {
        //
        // execute method is the only public
        //


        public override object Execute(CPBaseClass cp)
        {
            const string requestnameProcess = "prcs";
            const string captchaProcessAddonGuid = "{030AC5B0-F796-4EA4-B94C-986B1C29C16C}";
            const string captchaFormAddonGuid = "{E9E51C6E-9152-4284-A44F-D3ABC423AB90}";

            string returnHtml = "";
            try
            {
                bool previousFormPosted = false;
                CPCSBaseClass cs = cp.CSNew();
                string captchaRespone, optionStr, response;
                bool openSuccessfully = false;



                previousFormPosted = cp.Doc.GetBoolean(requestnameProcess);
                if (previousFormPosted)
                {
                    openSuccessfully = cp.Version == "3.4.060" ? cs.Open("Add-Ons", string.Format("ccGUID='{0}'", captchaProcessAddonGuid), "ID,Name,ccGUID") :
                              cs.Open("Add-Ons", string.Format("aoGUID={0}", captchaProcessAddonGuid));

                    if (openSuccessfully)
                    {
                        optionStr = "Challenge=" + cp.Doc.GetText("recaptcha_challenge_field");
                        optionStr = optionStr + "&Response=" + cp.Doc.GetText("recaptcha_response_field");
                        response = cp.Utils.ExecuteAddon(optionStr, CPUtilsBaseClass.addonContext.ContextAdmin);
                    }
                    cp.Doc.SetProperty("Challenge", cp.Doc.GetText("recaptcha_challenge_field"));
                    cp.Doc.SetProperty("Response", cp.Doc.GetText("recaptcha_response_field"));
                    captchaRespone = cp.Utils.ExecuteAddon(captchaProcessAddonGuid,CPUtilsBaseClass.addonContext.ContextSimple);
                    captchaRespone = cp.Utils.EncodeText(captchaRespone);
                    if (captchaRespone != "")
                    {
                        cp.UserError.Add(captchaRespone);
                    }
                }
                returnHtml = cp.Utils.ExecuteAddon(captchaFormAddonGuid);
                returnHtml += cp.Html.Hidden(requestnameProcess, "1");

            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex, "Unexpeced trap");
            }
            return returnHtml;
        }


    }
}
