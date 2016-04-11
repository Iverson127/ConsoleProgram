#include <Array.au3>
#include <File.au3>
#include <MsgBoxConstants.au3>

HotKeySet('{F9}','ReadPosPixel')
;~ HotKeySet('{F8}','FindPixel')
HotKeySet('{F3}','Terminate')
HotKeySet('{F4}','ExitECAT')

;MsgBox(0,"Debug", "test")
Func ReadPosPixel()
	$pos = MouseGetPos()
	$color = PixelGetColor( $pos[0], $pos[1])
	MsgBox(0,'Infomation',"pos = ("&$pos[0]&", "&$pos[1]&")"&@CRLF&"color = "&$color)
EndFunc

Func FindPixel(ByRef $var1, $findColor)
$var1 = PixelSearch(1160,135,1160,1000, $findColor)
If Not @error Then
	MouseMove($var1[0],$var1[1]+12)
	$var1[1] = $var1[1]+12
EndIf
EndFunc

Func Terminate()
	Exit
EndFunc

Func SelectSlaveTab($tabNo)
	Switch $tabNo
		Case 1
			ControlClick('ECAT Builder', '','[NAME:tabControl]','left',1,30,15)
			WinWaitActive('ECAT Builder','General')
		Case 2
			ControlClick('ECAT Builder', '','[NAME:tabControl]','left',1,100,15)
			WinWaitActive('ECAT Builder','PDO Mapping')
		Case 3
			ControlClick('ECAT Builder', '','[NAME:tabControl]','left',1,185,15)
			WinWaitActive('ECAT Builder','I/O Mapping')
		Case 4
			ControlClick('ECAT Builder', '','[NAME:tabControl]','left',1,280,15)
			WinWaitActive('ECAT Builder','Init Commands')
		Case 5
			ControlClick('ECAT Builder', '','[NAME:tabControl]','left',1,360,15)
			WinWaitActive('ECAT Builder','Parameter')
		Case 6
			ControlClick('ECAT Builder', '','[NAME:tabControl]','left',1,450,15)
			WinWaitActive('ECAT Builder','Distributed Clocks')
		Case 7
			ControlClick('ECAT Builder', '','[NAME:tabControl]','left',1,580,15)
			WinWaitActive('ECAT Builder','Advanced Options')
		Case Else
			MsgBox(0,'error','no this tab')
	EndSwitch
EndFunc

Func ExitECAT()
	Local $rst = WinClose('ECAT Builder')
	Sleep(200)
	If $rst <> 0 Then
		$rst = WinExists('Confirm','')
		If $rst = 1 Then
			ControlClick('Confirm','No', '[INSTANCE:2]')
		EndIf
	EndIf
	Terminate()
EndFunc

Func EcatInit()
	AutoItSetOption('MouseCoordMode',0)
	Run('D:\frankho\Project\EIPBuilder\bin\Debug\ECAT Builder.exe')
	WinWait('Warning','',3)
	WinActivate('Warning')
	ControlClick('Warning','確定',2)
	$hWnd = WinWait('ECAT Builder', 'Product List',30);timeout 30sec
	Sleep(100)
	WinMove('ECAT Builder', '',-8,-8,1382,744)
	Sleep(100)
	Local $pos[2]
	FindPixel($pos, 13622766)
	Sleep(100)
	MouseClickDrag('left',$pos[0],$pos[1], 1160, 350)
	Sleep(100)
	MouseMove(1160,135)
	MouseDown('left')
	MouseMove(1160,250)
	MouseUp('left')
EndFunc

EcatInit()
Sleep(300)
ControlClick('ECAT Builder','','[NAME:treeView_ProdList]', 'left',2,53,27)
Sleep(300)
ControlClick('ECAT Builder','','[NAME:treeView_ProdList]', 'left',2,85,45)
Sleep(3000)

;click first slave
Local $aPos = ControlGetPos('ECAT Builder','diagram1','[INSTANCE:8]')
MouseClick('left',$aPos[0]+300,$aPos[1]+180)

$result = WinWaitActive('ECAT Builder','General',2)
If $result = 0 Then
	MsgBox(0,'error','click first slave fail')
	Terminate()
EndIf
Sleep(500)
;~ For $i = 1 To 100
;~ 	SelectSlaveTab(Mod($i,7)+1)
;~ 	Sleep(200)
;~ Next
;~ Terminate()
SelectSlaveTab(2)
;~ Sleep(500)
;~ ;select 1st tx pdo
;~ ;MouseClick('left',330,490,1,50)
;~ Local $aPos = ControlGetPos('ECAT Builder','','[NAME:cellMergeTreeList_Input]')
;~ MouseClick('left',$aPos[0]+80,$aPos[1]+80)
ControlClick('ECAT Builder','','[NAME:cellMergeTreeList_Input]','left',2,$aPos[0]+80,$aPos[1]+80)
Send('{SPACE}')

;~ ;click delete
;~ ControlClick('ECAT Builder', 'Delete', '[NAME:btnPdoMappingDelete]')
;~ WinWait('ECAT Builder','Yes',2)
;~ Sleep(300)
;~ ControlClick('ECAT Builder', 'Yes', '[INSTANCE:1]')

;~ ;edit 2rd PDO
;~ MouseClick('left',$aPos[0]+80,$aPos[1]+80)
;~ Sleep(300)
;~ ControlClick('ECAT Builder', 'Edit', '[NAME:btnPdoMappingEdit]')
;~ WinWait('Edit PDO','',2)

;~ ;modify PDO name
;~ ControlClick('Edit PDO','','[NAME:textBoxPdoName]')
;~ Sleep(300)
;~ ControlSend('Edit PDO','','[NAME:textBoxPdoName]','{END}(123)')

;~ Sleep(300)
;~ ControlClick('Edit PDO', 'OK', '[NAME:btnOK]')
;~ Sleep(300)

;~ ;edit 3rd PDO
;~ MouseClick('left',$aPos[0]+80,$aPos[1]+120)
;~ Sleep(300)
;~ ControlClick('ECAT Builder', 'Edit', '[NAME:btnPdoMappingEdit]')
;~ WinWait('Edit PDO','',2)

;~ ;modify PDO name
;~ ControlClick('Edit PDO','','[NAME:textBoxPdoName]')
;~ Sleep(300)
;~ ControlSend('Edit PDO','','[NAME:textBoxPdoName]','{END}(123)')

;~ ;add the last one entry
;~ ControlClick('Edit PDO','Add','[NAME:btnAdd]')
;~ WinWait('Add Pdo Entry','',5)
;~ Local $bPos = ControlGetPos('Add Pdo Entry','','[NAME:treeListCoEObjDic]')
;~ MouseMove($bPos[0]+80,$bPos[1]+80)
;~ MouseClick('left')
;~ For $i = 0 To 50
;~ 	Send('{PGDN}')
;~ Next
;~ MouseClick('left',$bPos[0]+80,$bPos[1]+$bPos[3]+20,10)
;~ Sleep(300)
;~ ControlClick('Add Pdo Entry', 'Ok', '[NAME:btnOk]')

;~ ;add the second to last entry
;~ ControlClick('Edit PDO','Add','[NAME:btnAdd]')
;~ WinWait('Add Pdo Entry','',5)
;~ Local $bPos = ControlGetPos('Add Pdo Entry','','[NAME:treeListCoEObjDic]')
;~ MouseMove($bPos[0]+80,$bPos[1]+80)
;~ MouseClick('left')
;~ For $i = 0 To 50
;~ 	Send('{PGDN}')
;~ Next
;~ MouseClick('left',$bPos[0]+80,$bPos[1]+$bPos[3],10)
;~ Sleep(300)
;~ ControlClick('Add Pdo Entry', 'Ok', '[NAME:btnOk]')
;~ ;WinActivate('Edit PDO')
;~ Sleep(300)
;~ ControlClick('Edit PDO', 'OK', '[NAME:btnOK]')

;~ ;Edit 4th PDO
;~ ControlFocus('ECAT Builder','','[NAME:cellMergeTreeList_Input]')
;~ For $i = 0 To 5
;~ 	Send('{PGDN}')
;~ Next
;~ MouseClick('left',$aPos[0]+80,$aPos[1]+160,100)
;~ Sleep(300)
;~ ControlClick('ECAT Builder', 'Edit', '[NAME:btnPdoMappingEdit]')
;~ WinWaitActive('Edit PDO', '')

;~ ;Delete last 2 entries of 4th PDO
;~ ControlFocus('Edit PDO','','[NAME:treeList_Entries]')
;~ Sleep(300)
;~ Send('{PGDN}')
;~ Sleep(300)
;~ ControlClick('Edit PDO', 'Delete', '[NAME:btnDelete]')
;~ Sleep(300)
;~ ControlClick('ECAT Builder', 'Yes', '[INSTANCE:1]')
;~ ControlFocus('Edit PDO','','[NAME:treeList_Entries]')
;~ Sleep(300)
;~ Send('{PGDN}')
;~ Sleep(300)
;~ ControlClick('Edit PDO', 'Delete', '[NAME:btnDelete]')
;~ Sleep(300)
;~ ControlClick('ECAT Builder', 'Yes', '[INSTANCE:1]')
;~ Sleep(300)
;~ ControlClick('Edit PDO', 'OK', '[NAME:btnOK]')

SelectSlaveTab(3)
ControlClick('ECAT Builder', 'Auto Fill Symbol', '[NAME:btnAutoFillSymbol]')
Sleep(1500)
ControlClick('ECAT Builder', 'Clear Symbol', '[NAME:btnClearSymbol]')
Sleep(1500)

SelectSlaveTab(4)
;add Init Cmd
ControlClick('ECAT Builder', 'Add', '[NAME:btnInitCmdNew]')
WinWaitActive('Add Init Commands')
For $i = 0 To 50
	Send('{PGDN}')
Next
Local $cPos = ControlGetPos('Add Init Commands','','[NAME:treeList_CoEObjectDic]')
MouseMove($cPos[0]+76,$cPos[1]+290)
MouseClick('left')
ControlClick('Add Init Commands','','[NAME:tB_Data]')
Sleep(300)
ControlSend('Add Init Commands','','[NAME:tB_Data]','{END}123')
ControlClick('Add Init Commands','', '[NAME:cB_PS]')
Sleep(300)
ControlClick('Add Init Commands','', '[NAME:cB_IP]')
Sleep(300)
ControlClick('Add Init Commands', '', '[NAME:btnOk]')
Sleep(300)

;add Init Cmd
ControlClick('ECAT Builder', 'Add', '[NAME:btnInitCmdNew]')
WinWaitActive('Add Init Commands')
For $i = 0 To 50
	Send('{PGDN}')
Next
MouseMove($cPos[0]+76,$cPos[1]+270)
MouseClick('left')
ControlClick('Add Init Commands','', '[NAME:cB_PS]')
Sleep(300)
ControlSend('Add Init Commands','','[NAME:tB_Data]','{END}123')
Sleep(300)
ControlClick('Add Init Commands', "", "[NAME:cB_CompleteAccess]")
Sleep(300)
MouseMove($cPos[0]+577,$cPos[1]-275)
MouseClick('left')
Sleep(500)
MouseMove($cPos[0]+530,$cPos[1]-275)
MouseClick('left')
Sleep(500)
ControlClick('Add Init Commands', '', '[NAME:btnOk]')
Sleep(300)


;Delete the second Init Cmd
ControlClick('ECAT Builder', "Delete", '[NAME:btnInitCmdDelete]')
WinWait('Warning!','',5)
ControlClick('Warning!', "OK", '[CLASS:WindowsForms10.Window.b.app.0.2bf8098_r13_ad1; INSTANCE:1]')
Sleep(300)

;Edit Init Cmd
ControlClick('ECAT Builder', "Edit", '[NAME:btnInitCmdEdit]')
WinWaitActive('Edit Init Commands')
ControlClick('Edit Init Commands','', '[NAME:tB_Data]')
ControlSend('Edit Init Commands','','[NAME:tB_Data]','{END}123')
Sleep(300)
ControlClick('Edit Init Commands', '', '[NAME:btnOk]')
Sleep(300)

Sleep(300)
ControlClick('ECAT Builder','','[NAME:treeView_ProdList]', 'left',2,85,45)
Sleep(3000)

;Copy
ControlClick('ECAT Builder', "Copy", '[NAME:btnInitCmdCopy]')
WinWaitActive('Copy Init Command')
;Copy Init Cmd
ControlClick('Copy Init Command', '', '[NAME:checkBox_All]')
Sleep(300)
ControlClick('Copy Init Command', '', '[NAME:btnOk]')
Sleep(300)


;delete first slave
Local $ePos = ControlGetPos('ECAT Builder','diagram1','[INSTANCE:8]')
MouseClick('Right',$ePos[0]+300,$ePos[1]+180)
Sleep(300)
MouseMove($ePos[0]+310,$ePos[1]+190)
MouseClick('left')
WinWaitActive('Confirm')
ControlClick('Confirm', '', '[CLASS:WindowsForms10.Window.b.app.0.2bf8098_r13_ad1; INSTANCE:1]')
Sleep(300)

;click first slave
Local $aPos = ControlGetPos('ECAT Builder','diagram1','[INSTANCE:8]')
MouseClick('left',$aPos[0]+300,$aPos[1]+180)

$result = WinWaitActive('ECAT Builder','General',2)
If $result = 0 Then
	MsgBox(0,'error','click first slave fail')
	Terminate()
EndIf
Sleep(500)

ControlClick('ECAT Builder', '1002', '[CLASS:WindowsForms10.EDIT.app.0.2bf8098_r13_ad1; INSTANCE:1]')
Send('{BACKSPACE}')
ControlSend('ECAT Builder','1002','[CLASS:WindowsForms10.EDIT.app.0.2bf8098_r13_ad1; INSTANCE:1]','{END}1')
Send('{NUMPADENTER}')
Sleep(300)

Local $aFileList = _FileListToArray('C:\ProgramData\Delta Industrial Automation\ISPSoft\Project\Untitled0_ECAT')
;_ArrayDisplay($aFileList, "$aFileList")
;Sleep(300)

#CS ;Click Export ENI button
; ;ControlFocus('ECAT Builder', '', '[INSTANCE:46]')
; ;$tset = ControlClick('ECAT Builder', '', '[INSTANCE:46]', 'left', 1, 25,15)
;
; ;click Export ENI button
; WinActivate('ECAT Builder','')
; ;Opt("MouseCoordMode", 0)
; ;ControlClick('ECAT Builder','', '[INSTANCE:46]','left',1,21,15)
;
; MouseClick('left',260,65)
; ;MsgBox(0,"Debug", "test1")
; WinWaitActive('另存新檔','',5)
; Sleep(500)
; ControlSend('另存新檔','','','testENI1')
; Sleep(300)
; MouseClick('left',785,508)
; $rst = WinWaitActive('確認另存新檔','',3)
;
; $rst = WinExists('確認另存新檔','')
; If $rst = 1 Then
; 	Sleep(500)
; 	WinActivate('確認另存新檔','')
; 	Send('{LEFT}')
; 	Send('{ENTER}')
; EndIf
;
; While 1
; 	$rst = WinExists('確認另存新檔','')
; 	If $rst = 0 Then ExitLoop
; 	Sleep(300)
; WEnd
 #CE

Local $path1 = 'C:\ProgramData\Delta Industrial Automation\ISPSoft\Project\Untitled0_ECAT\' & string($aFileList[1])
Local $path2 = 'C:\ProgramData\Delta Industrial Automation\ISPSoft\Project\Untitled0_ECAT\' & string($aFileList[2])
;MsgBox('','',$path1)
;MsgBox('','',$path2)

Local $command1 = '"1" "' & $path1 & '" "' & $path2 & '"'

;Sleep(1500)
Local $toolRst = ShellExecuteWait ('C:\Users\franknc.ho\Desktop\何南瑾\小工具\AutoIt\MyAutoItTool.exe', $command1)

If $toolRst = 0 Then
	MsgBox('','Test Result', 'Success')
Else
	MsgBox('','Test Result', 'Failure')
EndIf

;exit
ExitECAT()

While 1
	Sleep(300)
WEnd