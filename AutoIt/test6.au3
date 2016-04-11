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
Sleep(300)
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:23]') ;+
Sleep(300)
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:11]')	;2
Sleep(300)
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:23]') ;+
Sleep(300)
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:16]')	;3
Sleep(300)
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:23]') ;+
Sleep(300)
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:4]')	;4
Sleep(300)
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:23]') ;+
Sleep(300)
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:10]')	;5
Sleep(300)
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:23]') ;+
Sleep(300)
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:15]')	;6
Sleep(300)
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:23]') ;+
Sleep(300)
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:3]')	;7
Sleep(300)
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:23]') ;+
Sleep(300)
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:9]')	;8
Sleep(300)
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:23]') ;+
Sleep(300)
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:14]')	;9
Sleep(300)
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:23]') ;+
Sleep(300)
ControlSend('¤pºâ½L','','','10');10
Sleep(300)
ControlClick('¤pºâ½L', '','[CLASS:Button; INSTANCE:28]') ;=
Sleep(300)

$result = WinGetText($hWnd)
$result = StringReplace($result,@LF, '')

;Check result
If $result== '55' Then
	WriteResult(0)
Else
	WriteResult(1)
EndIf