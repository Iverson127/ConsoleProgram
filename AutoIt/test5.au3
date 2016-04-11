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
$hWnd = WinWaitActive('¤pºâ½L','')
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:5]')	;1
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:23]') ;+
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:11]')	;2
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:23]') ;+
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:16]')	;3
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:23]') ;+
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:4]')	;4
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:23]') ;+
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:10]')	;5
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:23]') ;+
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:15]')	;6
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:23]') ;+
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:3]')	;7
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:23]') ;+
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:9]')	;8
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:23]') ;+
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:14]')	;9
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:23]') ;+
ControlSend('¤pºâ½L','','','10');10
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:28]') ;=

$result = WinGetText($hWnd)
$result = StringReplace($result,@LF, '')

;Check result
If $result== '55'Then
	WriteResult(0)
Else
	WriteResult(1)
EndIf