VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "DisplayClass"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Option Explicit

Private Main As Object
Private CSV As Object

Public Function Execute(CsvObject As Object, MainObject As Object, OptionString As String, FilterInput As String) As String

    Set CSV = CsvObject
    
    Call Init(MainObject)
    
    Execute = GetContent(OptionString)
    
End Function

Public Sub Init(MainObject As Object)
    Set Main = MainObject
    
    Exit Sub
ErrorTrap:
    Call HandleError("CAPTCHAClass", "Init", Err.Number, Err.Source, Err.Description, True, False)
    End Sub

Public Function GetContent(OptionString As String) As String
    On Error GoTo ErrorTrap
    
    Dim Stream As String
    Dim publicKey As String
    
    publicKey = Main.GetSiteProperty("reCAPTCHA Public Key", "6Ld_AgYAAAAAALQ66lAKd0-_YOKY3EVHQt0i0tQJ")
    
    If publicKey = "" Then
        Call Main.SetSiteProperty("reCAPTCHA Public Key", "6Ld_AgYAAAAAALQ66lAKd0-_YOKY3EVHQt0i0tQJ")
        publicKey = Main.GetSiteProperty("reCAPTCHA Public Key", "6Ld_AgYAAAAAALQ66lAKd0-_YOKY3EVHQt0i0tQJ")
    End If
    
    Stream = "<script type=""text/javascript"">"
    Stream = Stream & "var RecaptchaOptions = {"
    Stream = Stream & "   theme : 'white',"
    Stream = Stream & "   tabindex : 0"
    Stream = Stream & "};"
    Stream = Stream & "</script>"

    Stream = Stream & "<script type=""text/javascript"" src=""http://api.recaptcha.net/challenge?k=" & publicKey & """></script>"
    
    Stream = Stream & "<noscript>"
    Stream = Stream & "<iframe src=""http://api.recaptcha.net/noscript?k=" & publicKey & """ frameborder=""1""></iframe><br>"
    Stream = Stream & "<textarea name=""recaptcha_challenge_field"" rows=""3"" cols=""40""></textarea>"
    Stream = Stream & "<input type=""hidden"" name=""recaptcha_response_field"" value=""manual_challenge"">"
    Stream = Stream & "</noscript>"
    
    GetContent = Stream
    
    Exit Function
ErrorTrap:
    Call HandleError("CAPTCHAClass", "GetContent", Err.Number, Err.Source, Err.Description, True, False)
    End Function
