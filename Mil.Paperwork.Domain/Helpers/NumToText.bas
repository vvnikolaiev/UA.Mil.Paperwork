Attribute VB_Name = "UkrNumbers"
Option Explicit

' Gender constants
Private Const GM As Integer = 0
Private Const GF As Integer = 1
Private Const GN As Integer = 2

' Builds a Unicode string from character code arguments.
' Avoids encoding issues — the file stays pure ASCII.
Private Function W(ParamArray c() As Variant) As String
    Dim i As Integer, s As String
    For i = 0 To UBound(c): s = s & ChrW(c(i)): Next i
    W = s
End Function

Private Function NC(ByVal n As Long) As Integer
    If n Mod 10 = 1 And n Mod 100 <> 11 Then
        NC = 1
    ElseIf n Mod 100 >= 12 And n Mod 100 <= 14 And n > 20 Then
        NC = 3
    ElseIf n Mod 10 >= 2 And n Mod 10 <= 4 And (n < 10 Or n > 20) Then
        NC = 2
    Else
        NC = 3
    End If
End Function

Private Function UnitM(ByVal n As Long) As String
    Select Case n
        Case 0: UnitM = W(1085,1091,1083,1100)
        Case 1: UnitM = W(1086,1076,1080,1085)
        Case 2: UnitM = W(1076,1074,1072)
        Case 3: UnitM = W(1090,1088,1080)
        Case 4: UnitM = W(1095,1086,1090,1080,1088,1080)
        Case 5: UnitM = W(1087,39,1103,1090,1100)
        Case 6: UnitM = W(1096,1110,1089,1090,1100)
        Case 7: UnitM = W(1089,1110,1084)
        Case 8: UnitM = W(1074,1110,1089,1110,1084)
        Case 9: UnitM = W(1076,1077,1074,39,1103,1090,1100)
    End Select
End Function

Private Function UnitF(ByVal n As Long) As String
    Select Case n
        Case 0: UnitF = W(1085,1091,1083,1100)
        Case 1: UnitF = W(1086,1076,1085,1072)
        Case 2: UnitF = W(1076,1074,1110)
        Case 3: UnitF = W(1090,1088,1080)
        Case 4: UnitF = W(1095,1086,1090,1080,1088,1080)
        Case 5: UnitF = W(1087,39,1103,1090,1100)
        Case 6: UnitF = W(1096,1110,1089,1090,1100)
        Case 7: UnitF = W(1089,1110,1084)
        Case 8: UnitF = W(1074,1110,1089,1110,1084)
        Case 9: UnitF = W(1076,1077,1074,39,1103,1090,1100)
    End Select
End Function

Private Function UnitN(ByVal n As Long) As String
    Select Case n
        Case 0: UnitN = W(1085,1091,1083,1100)
        Case 1: UnitN = W(1086,1076,1085,1077)
        Case 2: UnitN = W(1076,1074,1072)
        Case 3: UnitN = W(1090,1088,1080)
        Case 4: UnitN = W(1095,1086,1090,1080,1088,1080)
        Case 5: UnitN = W(1087,39,1103,1090,1100)
        Case 6: UnitN = W(1096,1110,1089,1090,1100)
        Case 7: UnitN = W(1089,1110,1084)
        Case 8: UnitN = W(1074,1110,1089,1110,1084)
        Case 9: UnitN = W(1076,1077,1074,39,1103,1090,1100)
    End Select
End Function

Private Function GU(ByVal n As Long, ByVal g As Integer) As String
    Select Case g
        Case GF:   GU = UnitF(n)
        Case GN:   GU = UnitN(n)
        Case Else: GU = UnitM(n)
    End Select
End Function

Private Function Teen(ByVal n As Long) As String
    Select Case n
        Case 10: Teen = W(1076,1077,1089,1103,1090,1100)
        Case 11: Teen = W(1086,1076,1080,1085,1072,1076,1094,1103,1090,1100)
        Case 12: Teen = W(1076,1074,1072,1085,1072,1076,1094,1103,1090,1100)
        Case 13: Teen = W(1090,1088,1080,1085,1072,1076,1094,1103,1090,1100)
        Case 14: Teen = W(1095,1086,1090,1080,1088,1085,1072,1076,1094,1103,1090,1100)
        Case 15: Teen = W(1087,39,1103,1090,1085,1072,1076,1094,1103,1090,1100)
        Case 16: Teen = W(1096,1110,1089,1090,1085,1072,1076,1094,1103,1090,1100)
        Case 17: Teen = W(1089,1110,1084,1085,1072,1076,1094,1103,1090,1100)
        Case 18: Teen = W(1074,1110,1089,1110,1084,1085,1072,1076,1094,1103,1090,1100)
        Case 19: Teen = W(1076,1077,1074,39,1103,1090,1085,1072,1076,1094,1103,1090,1100)
    End Select
End Function

Private Function TensW(ByVal n As Long) As String
    Select Case n
        Case 10: TensW = W(1076,1077,1089,1103,1090,1100)
        Case 20: TensW = W(1076,1074,1072,1076,1094,1103,1090,1100)
        Case 30: TensW = W(1090,1088,1080,1076,1094,1103,1090,1100)
        Case 40: TensW = W(1089,1086,1088,1086,1082)
        Case 50: TensW = W(1087,39,1103,1090,1076,1077,1089,1103,1090)
        Case 60: TensW = W(1096,1110,1089,1090,1076,1077,1089,1103,1090)
        Case 70: TensW = W(1089,1110,1084,1076,1077,1089,1103,1090)
        Case 80: TensW = W(1074,1110,1089,1110,1084,1076,1077,1089,1103,1090)
        Case 90: TensW = W(1076,1077,1074,39,1103,1085,1086,1089,1090,1086)
    End Select
End Function

Private Function HundredsW(ByVal n As Long) As String
    Select Case n
        Case 100: HundredsW = W(1089,1090,1086)
        Case 200: HundredsW = W(1076,1074,1110,1089,1090,1110)
        Case 300: HundredsW = W(1090,1088,1080,1089,1090,1072)
        Case 400: HundredsW = W(1095,1086,1090,1080,1088,1080,1089,1090,1072)
        Case 500: HundredsW = W(1087,39,1103,1090,1089,1086,1090)
        Case 600: HundredsW = W(1096,1110,1089,1090,1089,1086,1090)
        Case 700: HundredsW = W(1089,1110,1084,1089,1086,1090)
        Case 800: HundredsW = W(1074,1110,1089,1110,1084,1089,1086,1090)
        Case 900: HundredsW = W(1076,1077,1074,39,1103,1090,1089,1086,1090)
    End Select
End Function

Private Function ThForm(ByVal nc As Integer) As String
    Select Case nc
        Case 1:    ThForm = W(1090,1080,1089,1103,1095,1072)
        Case 2:    ThForm = W(1090,1080,1089,1103,1095,1110)
        Case Else: ThForm = W(1090,1080,1089,1103,1095)
    End Select
End Function

Private Function MiForm(ByVal nc As Integer) As String
    Select Case nc
        Case 1:    MiForm = W(1084,1110,1083,1100,1081,1086,1085)
        Case 2:    MiForm = W(1084,1110,1083,1100,1081,1086,1085,1080)
        Case Else: MiForm = W(1084,1110,1083,1100,1081,1086,1085,1110,1074)
    End Select
End Function

Private Function BiForm(ByVal nc As Integer) As String
    Select Case nc
        Case 1:    BiForm = W(1084,1110,1083,1100,1103,1088,1076)
        Case 2:    BiForm = W(1084,1110,1083,1100,1103,1088,1076,1080)
        Case Else: BiForm = W(1084,1110,1083,1100,1103,1088,1076,1110,1074)
    End Select
End Function

Private Function HF(ByVal c As Integer) As String
    Select Case c
        Case 1:    HF = W(1075,1088,1080,1074,1085,1103)
        Case 2:    HF = W(1075,1088,1080,1074,1085,1110)
        Case Else: HF = W(1075,1088,1080,1074,1077,1085,1100)
    End Select
End Function

Private Function KF(ByVal c As Integer) As String
    Select Case c
        Case 1:    KF = W(1082,1086,1087,1110,1081,1082,1072)
        Case 2:    KF = W(1082,1086,1087,1110,1081,1082,1080)
        Case Else: KF = W(1082,1086,1087,1110,1081,1086,1082)
    End Select
End Function

Private Function FmtNum(ByVal n As Long) As String
    Dim s As String: s = CStr(n)
    Dim result As String: result = ""
    Dim cnt As Integer: cnt = 0
    Dim i As Integer
    For i = Len(s) To 1 Step -1
        If cnt > 0 And cnt Mod 3 = 0 Then result = " " & result
        result = Mid(s, i, 1) & result
        cnt = cnt + 1
    Next i
    FmtNum = result
End Function

Private Function NW(ByVal n As Long, ByVal g As Integer) As String
    Dim q As Long, r As Long, s As String
    If n = 0 Then NW = GU(0, g): Exit Function
    If n < 10 Then
        NW = GU(n, g)
    ElseIf n < 20 Then
        NW = Teen(n)
    ElseIf n < 100 Then
        s = TensW((n \ 10) * 10)
        If n Mod 10 > 0 Then s = s & " " & GU(n Mod 10, g)
        NW = s
    ElseIf n < 1000 Then
        s = HundredsW((n \ 100) * 100)
        If n Mod 100 > 0 Then s = s & " " & NW(n Mod 100, g)
        NW = s
    ElseIf n < 1000000 Then
        q = n \ 1000: r = n Mod 1000
        s = NW(q, GF) & " " & ThForm(NC(q))
        If r > 0 Then s = s & " " & NW(r, g)
        NW = s
    ElseIf n < 1000000000 Then
        q = n \ 1000000: r = n Mod 1000000
        s = NW(q, GM) & " " & MiForm(NC(q))
        If r > 0 Then s = s & " " & NW(r, g)
        NW = s
    Else
        q = n \ 1000000000: r = n Mod 1000000000
        s = NW(q, GM) & " " & BiForm(NC(q))
        If r > 0 Then s = s & " " & NW(r, g)
        NW = s
    End If
End Function

' =========================================================================
'  Public UDF — call from any Excel cell
'
'  =NumToText(A1)              -> number in words, masculine (default)
'  =NumToText(A1,"number_m")   -> masculine:  "сто двадцять три"
'  =NumToText(A1,"number_f")   -> feminine:   "одна тисяча"
'  =NumToText(A1,"number_n")   -> neuter:     "одне"
'  =NumToText(A1,"sum")        -> "123грн. 45коп. (сто двадцять три гривні...)"
'  =NumToText(A1,"sum_words")  -> "сто двадцять три гривні сорок п'ять копійок"
'
'  Supported range: 0..2 147 483 647
'  Negative values and non-numeric input return #VALUE!
' =========================================================================
Public Function NumToText(ByVal v As Variant, Optional ByVal fmt As String = "number") As Variant
    If IsError(v) Or IsNull(v) Or Not IsNumeric(v) Then
        NumToText = CVErr(2015): Exit Function
    End If
    If CDbl(v) < 0 Then
        NumToText = CVErr(2015): Exit Function
    End If
    Dim f As String: f = LCase(Trim(fmt))
    Dim ip As Long, fp As Long, db As Double
    Dim grn As String: grn = W(1075,1088,1085)
    Dim kop As String: kop = W(1082,1086,1087)
    Select Case f
        Case "sum", "amount"
            db = CDbl(v): ip = CLng(Int(db)): fp = CLng(Int((db - Int(db)) * 100 + 0.5))
            NumToText = FmtNum(ip) & grn & ". " & Format(fp, "00") & kop & ". (" & _
                NW(ip, GF) & " " & HF(NC(ip)) & " " & NW(fp, GF) & " " & KF(NC(fp)) & ")"
        Case "sum_words", "amount_words"
            db = CDbl(v): ip = CLng(Int(db)): fp = CLng(Int((db - Int(db)) * 100 + 0.5))
            NumToText = NW(ip, GF) & " " & HF(NC(ip)) & " " & NW(fp, GF) & " " & KF(NC(fp))
        Case "number_f": NumToText = NW(CLng(v), GF)
        Case "number_n": NumToText = NW(CLng(v), GN)
        Case Else:       NumToText = NW(CLng(v), GM)
    End Select
End Function
