Func WriteResult($result)
	$fileName = StringReplace(@ScriptFullPath, '.au3','.txt')
	FileDelete($fileName)
	If $result = 0 Then
		$result = 'Pass'
	Else
		$result = 'Fail'
	EndIf
	;FileWriteLine($fileName, 'Yes')
	FileWriteLine($fileName, $result)
EndFunc

Run('calc')
$hWnd = WinWaitActive('�p��L','')
ControlClick('�p��L', '','[CLASS:Button; INSTANCE:5]')	;1
ControlClick('�p��L', '','[CLASS:Button; INSTANCE:23]') ;+
ControlClick('�p��L', '','[CLASS:Button; INSTANCE:11]')	;2
ControlClick('�p��L', '','[CLASS:Button; INSTANCE:23]') ;+
ControlClick('�p��L', '','[CLASS:Button; INSTANCE:16]')	;3
ControlClick('�p��L', '','[CLASS:Button; INSTANCE:23]') ;+
ControlClick('�p��L', '','[CLASS:Button; INSTANCE:4]')	;4
ControlClick('�p��L', '','[CLASS:Button; INSTANCE:23]') ;+
ControlClick('�p��L', '','[CLASS:Button; INSTANCE:10]')	;5
ControlClick('�p��L', '','[CLASS:Button; INSTANCE:23]') ;+
ControlClick('�p��L', '','[CLASS:Button; INSTANCE:15]')	;6
ControlClick('�p��L', '','[CLASS:Button; INSTANCE:23]') ;+
ControlClick('�p��L', '','[CLASS:Button; INSTANCE:3]')	;7
ControlClick('�p��L', '','[CLASS:Button; INSTANCE:23]') ;+
ControlClick('�p��L', '','[CLASS:Button; INSTANCE:9]')	;8
ControlClick('�p��L', '','[CLASS:Button; INSTANCE:23]') ;+
ControlClick('�p��L', '','[CLASS:Button; INSTANCE:14]')	;9
ControlClick('�p��L', '','[CLASS:Button; INSTANCE:23]') ;+
ControlSend('�p��L','','','10');10
ControlClick('�p��L', '','[CLASS:Button; INSTANCE:28]') ;=

$result = WinGetText($hWnd)
$result = StringReplace($result,@LF, '')

;Check result
If $result== '55'Then
	WriteResult(0)
Else
	WriteResult(1)
EndIf