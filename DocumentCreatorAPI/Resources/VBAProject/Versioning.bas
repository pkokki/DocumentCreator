Attribute VB_Name = "Versioning"
Option Explicit

Private Const VERSION_MAJOR As String = "0"
Private Const VERSION_MINOR As String = "1"
Private Const VERSION_YEAR As Integer = 2020
Private Const VERSION_MONTH As Integer = 4
Private Const VERSION_DAY As Integer = 12

Private Const VERSION_PROPERTY As String = "DocumentCreatorVersion"

Function GetDocumentCreatorVersion()
    Dim cdp As DocumentProperty, cdp_value As String
    cdp_value = "<Not Set>"
    For Each cdp In ActiveWorkbook.CustomDocumentProperties
        If cdp.Name = VERSION_PROPERTY Then
            cdp_value = cdp.Value
            Exit For
        End If
    Next cdp
    GetDocumentCreatorVersion = cdp_value
End Function

Sub UpdateDocumentCreatorVersion()
    Dim cdp As DocumentProperty, cdp_exists As Boolean, cdp_value As String
    cdp_value = VERSION_MAJOR & "." & VERSION_MINOR & "." _
                & DateDiff("s", DateSerial(VERSION_YEAR, VERSION_MONTH, VERSION_DAY), Now())
    
    ' Check if the property exists
    cdp_exists = False
    For Each cdp In ActiveWorkbook.CustomDocumentProperties
        If cdp.Name = VERSION_PROPERTY Then
            cdp_exists = True
        End If
    Next cdp
    
    ' Add or update property
    If cdp_exists Then
        Debug.Print "Updating to version " & cdp_value
        ActiveWorkbook.CustomDocumentProperties(VERSION_PROPERTY).Value = cdp_value
    Else
        Debug.Print "Adding version " & cdp_value
        ActiveWorkbook.CustomDocumentProperties.Add _
            Name:=VERSION_PROPERTY, _
            LinkToContent:=False, _
            Type:=4, _
            Value:=cdp_value
    End If
End Sub


