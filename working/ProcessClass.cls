VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "ProcessClass"
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
    Call HandleError("aoCAPTCHA.ProcessClass", "Init", Err.Number, Err.Source, Err.Description, True, False)
    End Sub


Public Function GetContent(OptionString As String) As String
    On Error GoTo ErrorTrap
    
    Dim VarString As String
    Dim objXmlHttp As New MSXML2.ServerXMLHTTP
    Dim ResponseString() As String
    Dim privateKey As String
    Dim Stream As String
    Dim Challenge As String
    Dim Response As String
    
    privateKey = Main.GetSiteProperty("reCAPTCHA Private Key", "6Ld_AgYAAAAAAH2GRsRZHth-Rud3nTKjx8d019pE")
    '
    ' read Challenge and Response from the option string if they are explicitly set by the calling program
    '
    Challenge = Main.GetAggrOption("Challenge", OptionString)
    Response = Main.GetAggrOption("Response", OptionString)
    If Challenge = "" Then
        '
        ' if the process is just run on the process of the form containing the DisplayClass, these are in the stream
        '
        Challenge = Main.GetStreamText("recaptcha_challenge_field")
        Response = Main.GetStreamText("recaptcha_response_field")
    End If
    
    If privateKey = "" Then
        Call Main.SetSiteProperty("reCAPTCHA Private Key", "6Ld_AgYAAAAAAH2GRsRZHth-Rud3nTKjx8d019pE")
        privateKey = Main.GetSiteProperty("reCAPTCHA Private Key", "6Ld_AgYAAAAAAH2GRsRZHth-Rud3nTKjx8d019pE")
    End If
    
    VarString = "privatekey=" & privateKey
    VarString = VarString & "&remoteip=" & Main.VisitRemoteIP
    VarString = VarString & "&challenge=" & Challenge
    VarString = VarString & "&response=" & Response
    
    Call objXmlHttp.open("POST", "http://api-verify.recaptcha.net/verify", False)
    Call objXmlHttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded")
    Call objXmlHttp.send(VarString)
    
    ResponseString = Split(objXmlHttp.responseText, vbLf)
    Set objXmlHttp = Nothing
    
    If ResponseString(0) = "true" Then
       Stream = ""
    Else
       Stream = ResponseString(1)
    End If
    
    GetContent = Stream
    
    Exit Function
ErrorTrap:
    Call HandleError("aoCAPTCHA.ProcessClass", "GetContent", Err.Number, Err.Source, Err.Description, True, False)
    End Function
