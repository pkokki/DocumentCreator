Option Explicit On
Private Const FIELD_PARENT_COLUMN As Integer = 3
Private Const FIELD_CONTENT_COLUMN As Integer = 6
Private Const MAPPINGS_RANGE As String = "B3:J200"
Private Const FIELD_ID_RANGE As String = "B1:B200"
Private Const SOURCES_RANGE As String = "P3:Q10"
Private Const SOURCE_NAMES_RANGE As String = "P3:P10"
Private Const SOURCE_VALUES_RANGE As String = "Q3:Q10"
Private Const TEMPLATE_NAME_CELL As String = "Q15"
Private Const TEST_MAPPING_URL_CELL As String = "Q17"
Private Const TEST_RESULTS_COLUMN As String = "M"
Private Const TEST_CHECK_COLUMN As String = "N"
Private Const EXPRESSION_COLUMN As String = "J"
Private Const TEST_EXPR_TOTAL As String = "Q22"
Private Const TEST_EXPR_ERRORS As String = "Q23"
Private m_source_cache As New Dictionary
Private m_parent_sources(100) As Collection
' ----

Public Sub OnTestMapping()
    Dim test_mapping_url As String, obj_http As Object, test_mapping_req As String, row_index As Integer,
        response_json As Object, results_collection As Collection, json_result As Object,
        expression_addr As String, result_addr As String

    test_mapping_url = Range(TEST_MAPPING_URL_CELL).Text
    test_mapping_req = PrepareTestJson()
    Debug.Print "--------- REQUEST -------------------------"
    Debug.Print test_mapping_req
    Debug.Print("--------------------------------------------")
    Set obj_http = CreateObject("MSXML2.XMLHTTP.6.0")
    
    On Error GoTo err_handler
    With obj_http
        .Open "POST", test_mapping_url, False
        .SetRequestHeader "Content-type", "application/json"
        .SetRequestHeader "Accept", "application/json"
        .Send test_mapping_req

        If .Status = "200" Then
            Debug.Print "--------- RESPONSE -------------------------"
            Debug.Print.ResponseText
            Debug.Print "--------------------------------------------"
            Set response_json = JsonConverter.ParseJson(.ResponseText)
            If TypeName(response_json("results")) = "Collection" Then
                row_index = 3
                
                
                Set results_collection = response_json("results")
                For Each json_result In results_collection
                    ' Update result column
                    Range(TEST_RESULTS_COLUMN & row_index).NumberFormat = "@"
                    If VBA.IsNull(json_result("error")) Then
                        Range(TEST_RESULTS_COLUMN & row_index).Value = json_result("text")
                    Else
                        Range(TEST_RESULTS_COLUMN & row_index).Value = json_result("error")
                    End If
                    ' Update check column
                    expression_addr = EXPRESSION_COLUMN & row_index
                    result_addr = TEST_RESULTS_COLUMN & row_index
                    ' =IF(ISNA(FORMULATEXT(J3));"";IF(J3=M3;1;IF(J3=IFNA(VALUE(M3);M3);1;2)))
                    Range(TEST_CHECK_COLUMN & row_index).NumberFormat = "General"
                    Range(TEST_CHECK_COLUMN & row_index).Value = ""
                    Range(TEST_CHECK_COLUMN & row_index).Formula =
                        "=IF(" &
                            "ISNA(FORMULATEXT(" & expression_addr & "))," &
                            """""," &
                            "IF(" &
                                expression_addr & "=" & result_addr & "," &
                                "1," &
                                "IF(" & expression_addr & "=IFNA(VALUE(" & result_addr & ")," & result_addr & ")," &
                                    "1," &
                                    "2)" &
                            ")" &
                        ")"
                    row_index = row_index + 1
                Next json_result
            End If
            Range(TEST_EXPR_TOTAL).Value = response_json("total")
            Range(TEST_EXPR_ERRORS).Value = response_json("errors")
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
    Debug.Print test_mapping_url & " " & CStr(Err.Number) & ": " & Err.Description
    Debug.Print "--------------------------------------------"
    MsgBox test_mapping_url & vbCrLf & Err.Description, vbCritical, CStr(Err.Number)
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
    ' Add mappings B=1, C=2, D=3, E=4, F=5, G=7, I=8, J=9
    Set rng = Range(MAPPINGS_RANGE)
    For Each rng_row In rng.Rows
        If rng_row.Cells(1, 1).Value <> "" Then
            Set json_object = New Dictionary
            json_object("name") = rng_row.Cells(1, 1).Value
            json_object("cell") = rng_row.Cells(1, 9).Address(0, 0)
            json_object("expression") = rng_row.Cells(1, 9).Formula
            If rng_row.Cells(1, 3).Value = "" Or VBA.IsNull(rng_row.Cells(1, 3).Value) Then
                json_object("isCollection") = False
            Else
                json_object("isCollection") = rng_row.Cells(1, 3).Value
            End If
            json_object("parent") = rng_row.Cells(1, 2).Value
            json_object("content") = rng_row.Cells(1, 4).Value
            Call json_payload("expressions").Add(json_object)
        End If
    Next rng_row
    
    ' Add test sources
    Set rng = Range(SOURCES_RANGE)
    For Each rng_row In rng.Rows
        Set json_object = New Dictionary
        If rng_row.Cells(1, 1).Value <> "" Then
            json_object("name") = rng_row.Cells(1, 1).Value
            Set json_object("payload") = JsonConverter.ParseJson(rng_row.Cells(1, 2).Value)
            Call json_payload("sources").Add(json_object)
        End If
    Next rng_row

    PrepareTestJson = JsonConverter.ConvertToJson(json_payload)
End Function

Private Function GetSourceJson(name As String) As Dictionary
    Dim json_cache_item As Dictionary, source_row As Long, source_value As String
    
    Set json_cache_item = Nothing
    If m_source_cache.Exists(name) Then
        Set json_cache_item = m_source_cache.Item(name)
    Else
        On Error Resume Next
        source_row = Application.WorksheetFunction.Match(name, Range(SOURCE_NAMES_RANGE), 0)
        On Error GoTo 0
        If source_row > 0 Then
            source_value = Range(SOURCE_VALUES_RANGE).Cells(source_row, 1).Text
            On Error Resume Next
            Set json_cache_item = JsonConverter.ParseJson(source_value)
            On Error GoTo 0
            If json_cache_item Is Nothing Or TypeName(json_cache_item) <> "Dictionary" Then
                MsgBox "Error parsing json for source " & name
                Debug.Print "Error parsing json for source " & name
            Else
                Set m_source_cache(name) = json_cache_item
            End If
        End If
    End If
    Set GetSourceJson = json_cache_item
End Function

Public Function CONTENT(is_visible As Boolean) As String
    If is_visible Then
        If TypeName(Application.Caller) = "Range" Then
            CONTENT = Application.Caller.Worksheet.Cells(Application.Caller.Row, FIELD_CONTENT_COLUMN).Text
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

Public Function RQL(source_path As String) As Variant
    RQL = SOURCE("RQ", "LogHeader." & source_path)
End Function

Public Function RQD(source_path As String) As Variant
    RQD = SOURCE("RQ", "RequestData." & source_path)
End Function

Public Function RQR(source_path As String) As Variant
    Dim parent_id As Variant, parent_row As Long, result_buffer As String, child_value As Variant,
        source_collection As VBA.Collection, source_obj As Object, path_tokens() As String

    If TypeName(Application.Caller) = "Range" Then
        parent_id = Application.Caller.Worksheet.Cells(Application.Caller.Row, FIELD_PARENT_COLUMN).Value
        On Error Resume Next
        parent_row = Application.WorksheetFunction.Match(parent_id, Range(FIELD_ID_RANGE), 0)
        On Error GoTo 0
        If parent_row > 0 Then
            If TypeName(m_parent_sources(parent_row)) = "Collection" Then
                Set source_collection = m_parent_sources(parent_row)
                If source_collection.Count > 0 Then
                    ' build a JArray
                    result_buffer = ""
                    For Each source_obj In source_collection
                        If TypeName(source_obj) = "Dictionary" Then
                            path_tokens = Split(source_path, ".")
                            child_value = GetJsonCellValue(GetJsonNode(source_obj, path_tokens), path_tokens(UBound(path_tokens)))
                        Else
                            If VBA.IsMissing(source_path) Then
                                child_value = source_collection.Item(1)
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

Public Function SOURCE(source_name As String, source_path As String) As Variant
    Dim parent_node As Dictionary, source_tokens() As String, node_name As String,
        cell_value As Variant, source_json As Dictionary
    
    Set source_json = GetSourceJson(source_name)
    If source_json Is Nothing Then
        cell_value = "#MissingSourceName:" & source_name & "#"
    Else
        source_tokens = Split(source_path, ".")
        node_name = source_tokens(UBound(source_tokens))
        Set parent_node = GetJsonNode(source_json, source_tokens)
        cell_value = GetJsonCellValue(parent_node, node_name)
        Set m_parent_sources(Application.Caller.Row) = Nothing
        If cell_value = "[]" And Not parent_node Is Nothing Then
            If parent_node.Exists(node_name) Then Set m_parent_sources(Application.Caller.Row) = parent_node.Item(node_name)
        End If
    End If
    SOURCE = cell_value
End Function


Private Function GetJsonNode(json_object As Dictionary, path_tokens() As String) As Dictionary
    Dim current_node As Dictionary, i As Integer, node_name As String, child_node As Object
    
    Set current_node = json_object
    If (UBound(path_tokens) - LBound(path_tokens) + 1 > 1) Then
        For i = LBound(path_tokens) To UBound(path_tokens) - 1
            node_name = path_tokens(i)
            If current_node.Exists(node_name) Then
                Set child_node = Nothing
                On Error Resume Next
                Set child_node = current_node.Item(node_name)
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

Private Function GetJsonCellValue(parent_node As Dictionary, node_name As String) As Variant
    Dim cell_value As Variant, value_type As String

    If (parent_node Is Nothing) Then
        cell_value = "#Missingparent_node:" & node_name & "#"
    ElseIf Not parent_node.Exists(node_name) Then
        cell_value = "#MissingNode:" & node_name & "#"
    Else
        value_type = TypeName(parent_node.Item(node_name))
        If VBA.IsNull(parent_node.Item(node_name)) Then
            cell_value = ""
        ElseIf value_type = "Collection" Then
            cell_value = "[]"
        ElseIf value_type = "Dictionary" Then
            cell_value = "{}"
        ElseIf VBA.IsError(parent_node.Item(node_name)) Then
            cell_value = "VBA.Error"
        Else
            cell_value = parent_node.Item(node_name)
        End If
    End If
    GetJsonCellValue = cell_value
End Function

