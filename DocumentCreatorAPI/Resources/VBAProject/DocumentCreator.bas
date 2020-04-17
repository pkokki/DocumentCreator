Attribute VB_Name = "DocumentCreator"
Option Explicit
Private Const ROW_OFFSET As Integer = 2 ' We start at row 3
Private Const FIELD_PARENT_COLUMN As Integer = 2
Private Const FIELD_CONTENT_COLUMN As Integer = 4
Private Const MAPPINGS_RANGE As String = "A3:K200"
Private Const FIELD_ID_RANGE As String = "A3:A200"
Private Const SOURCES_RANGE As String = "M3:N11"
Private Const SOURCE_NAMES_RANGE As String = "M3:M11"
Private Const SOURCE_VALUES_RANGE As String = "N3:N11"
Private Const TEMPLATE_NAME_CELL As String = "N15"
Private Const TEST_MAPPING_URL_CELL As String = "N17"
Private Const TEST_RESULTS_COLUMN As String = "J"
Private Const TEST_EXPR_TOTAL As String = "N22"
Private Const TEST_EXPR_ERRORS As String = "N23"
Private m_source_cache As New Dictionary
Private m_parent_sources(100) As Collection
' ----
Public Function REFSOURCE(ByVal ref_range As Range) As Variant
    Dim ref_row As Range, source_dict As Dictionary, source_text As String
    Application.EnableEvents = False
    Set source_dict = New Dictionary
    For Each ref_row In ref_range.Rows
        If ref_row.Cells(1, 1).value <> "" Then
            source_dict(ref_row.Cells(1, 1).value) = ref_row.Cells(1, 2).value
        End If
    Next
    source_text = JsonConverter.ConvertToJson(source_dict)
    REFSOURCE = source_text
    Application.EnableEvents = True
End Function
Public Function MAPVALUE(ByVal source_cell As Range, ByVal source_path As String, Optional ByVal transform_cell As Range = Nothing) As Variant
    Dim parent_node As Dictionary, source_tokens() As String, node_name As String, _
        source_name As String, cell_value As Variant, source_json As Dictionary
    
    source_path = CStr(source_path)
    source_name = source_cell.Offset(0, -1).value
    Set source_json = GetSourceJson(source_name)
    If source_json Is Nothing Then
        cell_value = "#MissingSourceName:" & source_name & "#"
    Else
        source_tokens = Split(source_path, ".")
        node_name = source_tokens(UBound(source_tokens))
        Set parent_node = GetJsonNode(source_json, source_tokens)
        cell_value = GetJsonCellValue(parent_node, node_name)
        Set m_parent_sources(Application.Caller.row) = Nothing
        If cell_value = "[]" And Not parent_node Is Nothing Then
            If parent_node.Exists(node_name) Then
                Set m_parent_sources(Application.Caller.row) = parent_node.item(node_name)
                ' Fill values
            End If
        End If
        If Not transform_cell Is Nothing Then
            cell_value = MAPVALUE(transform_cell, CStr(cell_value))
        End If
    End If
    MAPVALUE = cell_value
End Function
Public Function MAPITEM(ByVal parent_cell As Range, ByVal source_path As String) As Variant
    Dim parent_row As Long, result_buffer As String, child_value As Variant, _
        source_collection As VBA.Collection, source_obj As Object, path_tokens() As String
  
    parent_row = parent_cell.row
    If TypeName(m_parent_sources(parent_row)) = "Collection" Then
        Set source_collection = m_parent_sources(parent_row)
        If source_collection.count > 0 Then
            ' build a JArray
            result_buffer = ""
            For Each source_obj In source_collection
                If TypeName(source_obj) = "Dictionary" Then
                    path_tokens = Split(source_path, ".")
                    child_value = GetJsonCellValue(GetJsonNode(source_obj, path_tokens), path_tokens(UBound(path_tokens)))
                Else
                    If VBA.IsMissing(source_path) Then
                        child_value = source_collection.item(1)
                    Else
                        MAPITEM = "MAPITEM:ItemNotObject#"
                        Exit For
                    End If
                End If
                If result_buffer <> "" Then result_buffer = result_buffer & ","
                result_buffer = result_buffer & "'" & CStr(child_value) & "'"
            Next source_obj
            If MAPITEM <> "MAPITEM:ItemNotObject#" Then
                MAPITEM = "[" & result_buffer & "]"
            End If
        Else
            MAPITEM = "MAPITEM:CollectionIsEmpty#"
        End If
    Else
        MAPITEM = "MAPITEM:ParentNotCollection#"
    End If
End Function
Public Function GETLIST(ByVal parent_cell As Range, ByVal source_path As String) As Variant()
    Dim parent_row As Long, child_value As Variant, i As Integer, _
        source_collection As VBA.Collection, source_obj As Object, path_tokens() As String
  
    Dim empty_array(0) As Variant
    parent_row = parent_cell.row
    If TypeName(m_parent_sources(parent_row)) = "Collection" Then
        Set source_collection = m_parent_sources(parent_row)
        If source_collection.count > 0 Then
            ReDim result_array(source_collection.count - 1) As Variant
            i = 0
            For Each source_obj In source_collection
                If TypeName(source_obj) = "Dictionary" Then
                    path_tokens = Split(source_path, ".")
                    child_value = GetJsonCellValue(GetJsonNode(source_obj, path_tokens), path_tokens(UBound(path_tokens)))
                    result_array(i) = child_value
                Else
                    If VBA.IsMissing(source_path) Then
                        child_value = source_collection.item(i)
                        result_array(i) = child_value
                    Else
                        Debug.Print "WARNING GETLIST: ItemNotObject#"
                        result_array = empty_array
                        Exit For
                    End If
                End If
                i = i + 1
            Next source_obj
            GETLIST = result_array
        Else
            Debug.Print "WARNING GETLIST: EmptyCollection"
            GETLIST = empty_array
        End If
    Else
        Debug.Print "WARNING GETLIST: CollectionNotFound"
        GETLIST = empty_array
    End If
End Function
Public Function GETITEM(ByVal parent_cell As Range, ByVal source_path As String, _
                        Optional ByVal transform_cell As Range = Nothing, Optional ByVal target_index As Integer = 1) As Variant
    Dim parent_row As Long, child_value As Variant, _
        source_collection As VBA.Collection, source_obj As Object, path_tokens() As String
  
    child_value = Null
    parent_row = parent_cell.row
    If TypeName(m_parent_sources(parent_row)) = "Collection" Then
        Set source_collection = m_parent_sources(parent_row)
        If source_collection.count > 0 Then
            If target_index > 0 And target_index <= source_collection.count Then
                If TypeName(source_collection.item(target_index)) = "Dictionary" Then
                    path_tokens = Split(source_path, ".")
                    child_value = GetJsonCellValue(GetJsonNode(source_collection.item(target_index), path_tokens), path_tokens(UBound(path_tokens)))
                Else
                    If VBA.IsMissing(source_path) Then
                        child_value = source_collection.item(target_index)
                    Else
                        GETITEM = "GETITEM:ItemNotObject#"
                    End If
                End If
            Else
                GETITEM = "GETITEM:IndexOutOfRange#"
            End If
        Else
            GETITEM = "GETITEM:CollectionIsEmpty#"
        End If
    Else
        GETITEM = "GETITEM:ParentNotCollection#"
    End If
    If Not IsNull(child_value) Then
        If Not transform_cell Is Nothing Then
            child_value = MAPVALUE(transform_cell, CStr(child_value))
        End If
        GETITEM = child_value
    End If
End Function


Public Sub OnWorksheetActivated()
    Dim rng As Range, rng_row As Range, expression_addr As String, result_addr As String, row_index As Integer
    Set rng = Range(MAPPINGS_RANGE)
    ' A=1, B=2, C=3, D=4, E=5, F=6, G=7, H=8, I=9, J=10, K=11
    For Each rng_row In rng.Rows
        If rng_row.Cells(1, 1).value <> "" Then
            ' Update check column
            expression_addr = rng_row.Cells(1, 6).Address(0, 0)
            result_addr = rng_row.Cells(1, 10).Address(0, 0)
            ' =IFNA(FORMULATEXT(F3);"")
            If rng_row.Cells(1, 9).Formula = "" Then
                With rng_row.Cells(1, 9)
                    .NumberFormat = "General"
                    .value = ""
                    .Formula = "=IFNA(FORMULATEXT(" & expression_addr & "),"""")"
                End With
            End If
            ' =IF(ISNA(FORMULATEXT(F3));"";IF(F3=J3;1;IF(F3=IFNA(VALUE(J3);J3);1;2)))
            If rng_row.Cells(1, 11).Formula = "" Then
                With rng_row.Cells(1, 11)
                    .NumberFormat = "General"
                    .value = ""
                    .Formula = "=IF(" & _
                                    "ISNA(FORMULATEXT(" & expression_addr & "))," & _
                                    """""," & _
                                    "IF(" & _
                                        expression_addr & "=" & result_addr & "," & _
                                        "1," & _
                                        "IF(" & expression_addr & "=IFNA(VALUE(" & result_addr & ")," & result_addr & ")," & _
                                            "1," & _
                                            "2)" & _
                                    ")" & _
                                ")"
                End With
            End If
        End If
    Next
End Sub

Public Sub OnTestMapping()
    Dim test_mapping_url As String, obj_http As Object, test_mapping_req As String, row_index As Integer, _
        response_json As Object, results_collection As Collection, json_result As Object
            
    test_mapping_url = Range(TEST_MAPPING_URL_CELL).Text
    test_mapping_req = PrepareTestJson()
    Debug.Print "--------- REQUEST -------------------------"
    Debug.Print test_mapping_req
    Debug.Print ("--------------------------------------------")
    Set obj_http = CreateObject("MSXML2.XMLHTTP.6.0")
    
    On Error GoTo err_handler
    With obj_http
        .Open "POST", test_mapping_url, False
        .SetRequestHeader "Content-type", "application/json"
        .SetRequestHeader "Accept", "application/json"
        .Send test_mapping_req

        If .Status = "200" Then
            Debug.Print "--------- RESPONSE -------------------------"
            Debug.Print .ResponseText
            Debug.Print "--------------------------------------------"
            Set response_json = JsonConverter.ParseJson(.ResponseText)
            If TypeName(response_json("results")) = "Collection" Then
                row_index = ROW_OFFSET + 1
                
                
                Set results_collection = response_json("results")
                For Each json_result In results_collection
                    ' Update result column
                    Range(TEST_RESULTS_COLUMN & row_index).NumberFormat = "@"
                    If VBA.IsNull(json_result("error")) Then
                        Range(TEST_RESULTS_COLUMN & row_index).value = json_result("text")
                    Else
                        Range(TEST_RESULTS_COLUMN & row_index).value = json_result("error")
                    End If
                    row_index = row_index + 1
                Next json_result
            End If
            Range(TEST_EXPR_TOTAL).value = response_json("total")
            Range(TEST_EXPR_ERRORS).value = response_json("errors")
        Else
            Debug.Print "--------- ERROR -------------------------"
            Debug.Print "ERROR: " & .Status & vbCrLf & .ResponseText
            Debug.Print "--------------------------------------------"
            MsgBox "ERROR: " & .Status & vbCrLf & .ResponseText
        End If
    End With
    Exit Sub

err_handler:
    Debug.Print "--------- ERROR @ err_handler -------------------------"
    Debug.Print test_mapping_url & " " & CStr(Err.number) & ": " & Err.Description
    Debug.Print "--------------------------------------------"
    MsgBox test_mapping_url & vbCrLf & Err.Description, vbCritical, CStr(Err.number)
End Sub

Public Sub OnCellChanged(ByVal target_cell As Range)
    If Not Application.Intersect(Range(SOURCES_RANGE), target_cell) Is Nothing Then
        Call m_source_cache.RemoveAll
        target_cell.Worksheet.EnableCalculation = False
        target_cell.Worksheet.EnableCalculation = True
    End If

End Sub

Private Function PrepareTestJson()
    Dim json_payload As Dictionary, rng As Range, rng_row As Range, json_object As Dictionary
    
    Set json_payload = JsonConverter.ParseJson("{'expressions':[], 'sources':[]}")
    json_payload("templateName") = Range(TEMPLATE_NAME_CELL).Text
    ' Add mappings A=1, B=2, C=3, D=4, E=5, F=6
    Set rng = Range(MAPPINGS_RANGE)
    For Each rng_row In rng.Rows
        If rng_row.Cells(1, 1).value <> "" Then
            Set json_object = New Dictionary
            json_object("name") = rng_row.Cells(1, 1).value
            json_object("cell") = rng_row.Cells(1, 6).Address(0, 0)
            json_object("expression") = rng_row.Cells(1, 6).Formula
            If rng_row.Cells(1, 3).value = "" Or VBA.IsNull(rng_row.Cells(1, 3).value) Then
                json_object("isCollection") = False
            Else
                json_object("isCollection") = rng_row.Cells(1, 3).value
            End If
            json_object("parent") = rng_row.Cells(1, 2).value
            json_object("content") = rng_row.Cells(1, 4).value
            Call json_payload("expressions").Add(json_object)
        End If
    Next rng_row
    
    ' Add test sources
    Set rng = Range(SOURCES_RANGE)
    For Each rng_row In rng.Rows
        Set json_object = New Dictionary
        If rng_row.Cells(1, 1).value <> "" Then
            json_object("name") = rng_row.Cells(1, 1).value
            json_object("cell") = rng_row.Cells(1, 2).Address(0, 0)
            Set json_object("payload") = JsonConverter.ParseJson(rng_row.Cells(1, 2).value)
            Call json_payload("sources").Add(json_object)
        End If
    Next rng_row

    PrepareTestJson = JsonConverter.ConvertToJson(json_payload)
End Function

Private Function GetSourceJson(ByVal source_name As String) As Dictionary
    Dim json_cache_item As Dictionary, source_row As Long, source_value As String
    
    Set json_cache_item = Nothing
    If m_source_cache.Exists(source_name) Then
        Set json_cache_item = m_source_cache.item(source_name)
    Else
        On Error Resume Next
        source_row = Application.WorksheetFunction.Match(source_name, Range(SOURCE_NAMES_RANGE), 0)
        On Error GoTo 0
        If source_row > 0 Then
            source_value = Range(SOURCE_VALUES_RANGE).Cells(source_row, 1).Text
            On Error Resume Next
            Set json_cache_item = JsonConverter.ParseJson(source_value)
            On Error GoTo 0
            If json_cache_item Is Nothing Or TypeName(json_cache_item) <> "Dictionary" Then
                MsgBox "Error parsing json for source " & source_name
                Debug.Print "Error parsing json for source " & source_name
            Else
                Set m_source_cache(source_name) = json_cache_item
            End If
        End If
    End If
    Set GetSourceJson = json_cache_item
End Function

Public Function CONTENT(ByVal is_visible As Boolean) As String
    If is_visible Then
        If TypeName(Application.Caller) = "Range" Then
            CONTENT = Application.Caller.Worksheet.Cells(Application.Caller.row, FIELD_CONTENT_COLUMN).Text
        Else
            CONTENT = "#CONTENT:UnsupportedCaller#"
        End If
    Else
        CONTENT = ""
    End If
End Function

Public Function SYSDATE() As String
    SYSDATE = CStr(Date)
End Function

Public Function RQL(ByVal source_path As String) As Variant
    RQL = SOURCE("RQ", "LogHeader." & source_path)
End Function

Public Function RQD(ByVal source_path As String) As Variant
    RQD = SOURCE("RQ", "RequestData." & source_path)
End Function

Public Function RQR(ByVal source_path As String) As Variant
    Dim parent_id As Variant, parent_row As Long, result_buffer As String, child_value As Variant, _
        source_collection As VBA.Collection, source_obj As Object, path_tokens() As String
                        
    If TypeName(Application.Caller) = "Range" Then
        parent_id = Application.Caller.Worksheet.Cells(Application.Caller.row, FIELD_PARENT_COLUMN).value
        On Error Resume Next
        parent_row = Application.WorksheetFunction.Match(parent_id, Range(FIELD_ID_RANGE), 0) + ROW_OFFSET
        On Error GoTo 0
        If parent_row > 0 Then
            If TypeName(m_parent_sources(parent_row)) = "Collection" Then
                Set source_collection = m_parent_sources(parent_row)
                If source_collection.count > 0 Then
                    ' build a JArray
                    result_buffer = ""
                    For Each source_obj In source_collection
                        If TypeName(source_obj) = "Dictionary" Then
                            path_tokens = Split(source_path, ".")
                            child_value = GetJsonCellValue(GetJsonNode(source_obj, path_tokens), path_tokens(UBound(path_tokens)))
                        Else
                            If VBA.IsMissing(source_path) Then
                                child_value = source_collection.item(1)
                            Else
                                RQR = "#RQR:ItemNotObject#"
                                Exit For
                            End If
                        End If
                        If result_buffer <> "" Then result_buffer = result_buffer & ","
                        result_buffer = result_buffer & "'" & CStr(child_value) & "'"
                    Next source_obj
                    If RQR <> "#RQR:ItemNotObject#" Then
                        RQR = "[" & result_buffer & "]"
                    End If
                Else
                    RQR = "#RQR:ArrayIsEmpty#"
                End If
            Else
                RQR = "#RQR:ArrayNotFound#"
            End If
        Else
            RQR = "#RQR:ParentNotFound#"
        End If
    Else
        RQR = "RQR:UnsupportedCaller#"
    End If
End Function

Public Function SOURCE(ByVal source_name As String, ByVal source_path As String) As Variant
    Dim parent_node As Dictionary, source_tokens() As String, node_name As String, _
        cell_value As Variant, source_json As Dictionary
    
    Set source_json = GetSourceJson(source_name)
    If source_json Is Nothing Then
        cell_value = "#MissingSourceName:" & source_name & "#"
    Else
        source_tokens = Split(source_path, ".")
        node_name = source_tokens(UBound(source_tokens))
        Set parent_node = GetJsonNode(source_json, source_tokens)
        cell_value = GetJsonCellValue(parent_node, node_name)
        Set m_parent_sources(Application.Caller.row) = Nothing
        If cell_value = "[]" And Not parent_node Is Nothing Then
            If parent_node.Exists(node_name) Then Set m_parent_sources(Application.Caller.row) = parent_node.item(node_name)
        End If
    End If
    SOURCE = cell_value
End Function


Private Function GetJsonNode(ByVal json_object As Dictionary, ByRef path_tokens() As String) As Dictionary
    Dim current_node As Dictionary, i As Integer, node_name As String, child_node As Object
    
    Set current_node = json_object
    If (UBound(path_tokens) - LBound(path_tokens) + 1 > 1) Then
        For i = LBound(path_tokens) To UBound(path_tokens) - 1
            node_name = path_tokens(i)
            If current_node.Exists(node_name) Then
                Set child_node = Nothing
                On Error Resume Next
                Set child_node = current_node.item(node_name)
                On Error GoTo 0
                Set current_node = child_node
                If child_node Is Nothing Then
                    Exit For
                End If
            Else
                Set current_node = Nothing
                Exit For
            End If
        Next
    End If
    Set GetJsonNode = current_node
End Function

Private Function GetJsonCellValue(ByVal parent_node As Dictionary, ByVal node_name As String) As Variant
    Dim cell_value As Variant, value_type As String

    If (parent_node Is Nothing) Then
        cell_value = "#Missingparent_node:" & node_name & "#"
    ElseIf Not parent_node.Exists(node_name) Then
        cell_value = "#MissingNode:" & node_name & "#"
    Else
        value_type = TypeName(parent_node.item(node_name))
        If VBA.IsNull(parent_node.item(node_name)) Then
            cell_value = ""
        ElseIf value_type = "Collection" Then
            cell_value = "[]"
        ElseIf value_type = "Dictionary" Then
            cell_value = "{}"
        ElseIf VBA.IsError(parent_node.item(node_name)) Then
            cell_value = "VBA.Error"
        Else
            cell_value = parent_node.item(node_name)
        End If
    End If
    GetJsonCellValue = cell_value
End Function






