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

Sleep(2000)
Run('calc')
$hWnd = WinWaitActive('小算盤','')
WriteResult(0)