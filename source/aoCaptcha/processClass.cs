using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;
using System.Xml;
using MSXML2;

namespace Contensive.addons.aoRecaptcha
{
    //
    // 1) Change the namespace to the collection name
    // 2) Change this class name to the addon name
    // 3) Create a Contensive Addon record with the namespace apCollectionName.ad
    // 3) add reference to CPBase.DLL, typically installed in c:\program files\kma\contensive\
    //
    public class processClass : Contensive.BaseClasses.AddonBaseClass
    {
        public processClass()
        {
            VarString = new StringBuilder();
            objXmlHttp = new ServerXMLHTTP();
        }
        private const string privateKeyField = "reCAPTCHA Private Key";
        private const string privateKeyValue = "6Ld_AgYAAAAAAH2GRsRZHth-Rud3nTKjx8d019pE";
        private string privateKey, returnResult, challenge, response;
        private string[] responseArray;
        private readonly StringBuilder VarString;
        private readonly ServerXMLHTTP objXmlHttp;
        private const string vbCr = "\r";
        private const string vbLf = "\n";
        private const string vbCrLf = "\r\n";
        //
        public override object Execute(CPBaseClass cp)
        {
            try
            {
                //
                appendDebug(cp, "Contensive.addons.aoRecaptcha.processClass.execute()--enter");
                //
                returnResult = cp.Doc.GetText("recaptcha first result");
                appendDebug(cp, "Contensive.addons.aoRecaptcha.processClass.execute()--previous result [" + returnResult + "]");
                if (string.IsNullOrEmpty(returnResult))
                {
                    //
                    // recapcha can only be processed once per page
                    //
                    privateKey = cp.Site.GetProperty(privateKeyField, privateKeyValue);
                    challenge = cp.Doc.GetText("Challenge");
                    response = cp.Doc.GetText("Response");
                    if (string.IsNullOrEmpty(challenge))
                    {
                        challenge = cp.Doc.GetText("recaptcha_challenge_field");
                        response = cp.Doc.GetText("recaptcha_response_field");
                    }
                    if (string.IsNullOrEmpty(privateKey))
                    {
                        cp.Site.SetProperty(privateKeyField, privateKeyValue);
                        privateKey = cp.Site.GetProperty(privateKeyField, privateKeyValue);
                    }
                    VarString.Append(string.Format("privatekey={0}", privateKey));
                    VarString.Append(string.Format("&remoteip={0}", cp.Request.RemoteIP)); // Need to ask how to get Main.VisitRemoteIP herer
                    VarString.Append(string.Format("&challenge={0}", challenge));
                    VarString.Append(string.Format("&response={0}", response));
                    objXmlHttp.open("POST", "http://api-verify.recaptcha.net/verify", false);
                    objXmlHttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                    objXmlHttp.send(VarString.ToString());
                    responseArray = objXmlHttp.responseText.Split(new char[] { '\n' });
                    if (responseArray.Length >= 2)
                    {
                        returnResult = responseArray[0] == "true" ? string.Empty : responseArray[1];
                    }
                    //
                    // save the first result for future calls on this page (empty string is reserved for the case where it did not previously run, so substitute)
                    //
                    if (returnResult == "")
                    {
                        returnResult = "empty";
                    }
                    cp.Doc.SetProperty("recaptcha first result", returnResult);
                }
                if (returnResult == "empty")
                {
                    returnResult = "";
                } else if (returnResult == "incorrect-captcha-sol") 
                {
                    returnResult = "The Recaptcha term did not match correctly. Please try again.";
                }
                //
                appendDebug(cp, "Contensive.addons.aoRecaptcha.processClass.execute()--exit");
                //
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex, "Unexpeced trap");
            }
            return returnResult;
        }
        //
        private void appendDebug(CPBaseClass cp, string logMsg)
        {
            //cp.Utils.AppendLog("aoCaptcha.log", logMsg);
        }
    }
}
