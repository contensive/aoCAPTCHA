VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CAPTCHAClass"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Option Explicit

Const RequestNameProcess = "prcs"

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
    Dim Flag As Boolean
    Dim CS As Long
    Dim optionStr As String
    Dim Response As String
    
    Flag = Main.GetStreamBoolean(RequestNameProcess)

    If Flag Then
        If Main.SiteProperty_BuildVersion >= "3.4.060" Then
            CS = Main.OpenCSContent("Add-Ons", "ccGUID='{030AC5B0-F796-4EA4-B94C-986B1C29C16C}'", , , , , "ID,Name,ccGUID")
        Else
            CS = Main.OpenCSContent("Add-Ons", "aoGUID='{030AC5B0-F796-4EA4-B94C-986B1C29C16C}'", , , , , "ID,Name,aoGUID")
        End If
        If Main.CSOK(CS) Then
            optionStr = "Challenge=" & Main.GetStreamText("recaptcha_challenge_field")
            optionStr = optionStr & "&Response=" & Main.GetStreamText("recaptcha_response_field")
            If Main.SiteProperty_BuildVersion >= "3.4.060" Then
                Response = Main.ExecuteAddon(Main.GetCSInteger(CS, "ID"), Main.GetCSText(CS, "Name"), optionStr, ContextAdmin, "", 0, "", Main.GetCSText(CS, "ccGUID"), 0)
            Else
                Response = Main.GetAddonContent(Main.GetCSInteger(CS, "ID"), Main.GetCSText(CS, "Name"), optionStr, ContextPage, "", 0, "", Main.GetCSText(CS, "aoGUID"))
            End If
            
            If Response <> "" Then
                Call Main.AddUserError(Response)
            End If
            
        End If
        Call Main.CloseCS(CS)
    End If
    
    If Main.SiteProperty_BuildVersion >= "3.4.060" Then
        CS = Main.OpenCSContent("Add-Ons", "ccGUID='{E9E51C6E-9152-4284-A44F-D3ABC423AB90}'", , , , , "ID,Name,ccGUID")
    Else
        CS = Main.OpenCSContent("Add-Ons", "aoGUID='{E9E51C6E-9152-4284-A44F-D3ABC423AB90}'", , , , , "ID,Name,aoGUID")
    End If
    If Main.CSOK(CS) Then
        If Main.SiteProperty_BuildVersion >= "3.4.060" Then
            Stream = Main.ExecuteAddon(Main.GetCSInteger(CS, "ID"), Main.GetCSText(CS, "Name"), "", ContextAdmin, "", 0, "", Main.GetCSText(CS, "ccGUID"), 0)
        Else
            Stream = Main.GetAddonContent(Main.GetCSInteger(CS, "ID"), Main.GetCSText(CS, "Name"), "", ContextPage, "", 0, "", Main.GetCSText(CS, "aoGUID"))
        End If
    End If
    Call Main.CloseCS(CS)
    
    Stream = Stream & Main.GetFormInputHidden(RequestNameProcess, "1")
    
    GetContent = Stream
    
    Exit Function
ErrorTrap:
    Call HandleError("CAPTCHAClass", "GetContent", Err.Number, Err.Source, Err.Description, True, False)
    End Function

