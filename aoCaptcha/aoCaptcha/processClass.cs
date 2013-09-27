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
        private string privateKey, stream, challenge, response;
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
                privateKey = cp.Site.GetProperty(privateKeyField, privateKeyValue);
                challenge = cp.Doc.GetText("Challenge");
                response = cp.Doc.GetText("Response");
                if (string.IsNullOrEmpty(challenge))
                {
                    challenge = cp.Doc.GetText("recaptcha_challenge_field");
                    response = cp.Doc.GetText("recaptcha_challenge_field");
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
                responseArray = objXmlHttp.responseText.Split( new char[] { '\n' });
                if (responseArray.Length >= 2)
                {
                    stream = responseArray[0] == "true" ? string.Empty : responseArray[1];
                }
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex, "Unexpeced trap");
            }
            return stream;
        }
    }
}
