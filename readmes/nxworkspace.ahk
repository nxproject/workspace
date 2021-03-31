#CommentFlag // 

#s::
payload := GetPayload()
packet := {}
packet.ts := A_TickCount
packet.value := GetActiveField()
packet.command := "Search"
SetPayload(payload, packet)
return

// Gets the NXProject payload, if any, form the clipboard
GetPayload() {
	ret := clipboard
	if (SubStr(ret, 1, 14) == "@@nxproject@@") {
		ret := SubStr(ret, 15)
	} else {
		ret := ""
	}
	return ret
}

// Saves the cmmand object (src), along with any previous payload into the clipboard
SetPayload(payload, src) {
	ret := ""
	if(IsObject(src)) {
		if (src.command=="Reset") {
			src := ""
			payload :=""
		} else {
			src := SerDes(src)
		}
	}
	len := StrLen(payload)
	if (len > 0) {
		ret := payload "," src
	} else {
		ret := src
	}
	if (StrLen(ret) > 0) {
		ret := "@@nxproject@@" + ret
	}
	clipboard := ret
	SoundBeep 
}

// Returns the text in the active field
GetActiveField() {
	Send ^a^c
	ClipWait
	clipboard := clipboard
	return clipboard
}

// WIN CALLS

// Maximizes active window
MaximizeActiveWindow() {
	WinGetActiveTitle, title
	WinMaximize, %title%
}

// Minimizes active window
MinimizeActiveWindow() {
	WinGetActiveTitle, title
	WinMinimize, %title%
}

// Restores active window
RestoreActiveWindow() {
	WinGetActiveTitle, title
	WinRestore, %title%
}

// Returns the title of the active window
GetActiveWindow() {
	WinGetActiveTitle, title
	return title
}

// Activates a window according to its title
ActivateWindow(title) {
	WinActivate, %title%
}

// Compares two fields to see if they are in the same "row" or "col"
InSameRow(a, b) {
	ans := 0
	if (b.y >= a.y AND b.y < (a.y + a.h))
	{ 
		ans := 1
	}
	else
	{
		bottom := b.y+b.h
		if (bottom > a.y AND bottom <= (a.y + a.h))
		{
			ans := 1
		}
	}
	return ans
}
InSameCol(a, b) {
	ans := 0
	if (b.x >= a.x AND b.x < (a.x + a.w))
	{ 
		ans := 1
	}
	else
	{
		right := b.x+b.w
		if (right > a.x AND right <= (a.x + a.w))
		{
			ans := 1
		}
	}
	return ans
}

// Returns an array of all fields (map) in the active window
MapActiveWindow() {
	SetTitleMatchMode, 3
	SetTitleMatchMode, slow
	WinGetActiveTitle, cWin
	WinGet, cList, ControlList, %cWin%
	// Map all of the fields
	fields := []
	Loop, Parse, cList, `n, `r
	{
		ControlGetText, cText, %A_LoopField%, A
		sn := StrSplit(A_LoopField, ".")
		if (sn.Length() > 2) 
		{
			ctl := {}
			ctl.c := A_LoopField
			ctl.n := sn[2]
			ctl.t := cText
			ControlGetPos cX, cY, cW, cH, %A_LoopField%, A
			ctl.x := cX
			ctl.y := cY
			ctl.h := cH
			ctl.w := cW
			ctl.done := 0
			fields.push(ctl)
		}
	}
	return fields
}

// Returns the next field to the right
FieldToTheRight(map, index, c := 1) {
	ans := -1
	at := map[index]
	For k, v In map
	{
		if (k > index)
		{
			if (InSameRow(at, v))
			{
				ans := k
				c := c - 1
				if(c < = 0)
				{
					break
				}
			}
		}
	}
	return ans
}

// Returns the next field to the right
FieldBelow(map, index, c := 1) {
	ans := -1
	at := map[index]
	For k, v In map
	{
		if (k > index)
		{
			if (InSameCol(at, v))
			{
				ans := k
				c := c - 1
				if(c < = 0)
				{
					break
				}
			}
		}
	}
	return ans
}

// Returns the index position of the active field in a window from a map.  -1 if none found
FindActive(map) {
	ans := -1
	ControlGetFocus, OutputVar
	For k, v In map
	{
		if (v.c = OutputVar)
		{
			ans := k
			Break
		}
	}
	return ans
}

// Returns the index position of a field with the given text in a window from a map.  -1 if none found
FindText(map, text) {
	ans := -1
	For k, v In map
	{
		if (v.t = text)
		{
			ans := k
			Break
		}
	}
	return ans
}

// Returns the index position of a label with the given text in a window from a map.  -1 if none found
FindLabel(map, text) {
	ans := -1
	For k, v In map
	{
		if (v.t = text and v.n = "STATIC")
		{
			ans := k
			Break
		}
	}
	return ans
}

// Returns the text field after a label
GetFieldAfterLabel(map, text, c := 1) {
	ans := ""
	pos := FindLabel(map, text)
	if (pos >= 0) 
	{
		pos := FieldToTheRight(map, pos, c)
		if (pos >= 0)
		{
			ans := GetText(map, pos)
		}
	}
	return ans
}

// Returns the index position of a button with the given text in a window from a map.  -1 if none found
FindButton(map, text) {
	ans := -1
	For k, v In map
	{
		if (v.t = text and v.n = "BUTTON")
		{
			ans := k
			Break
		}
	}
	return ans
}

// Click on button
FindAndClickButton(map, text) {
	For k, v In map
	{
		if (v.t = text and v.n = "BUTTON")
		{
			ctl := v.n
			ControlClick,%ctl%
			Break
		}
	}
}

// Returns the text of a field with the given index in a window from a map.  -1 if none found
ClickButton(map, index) {
	if (index > 0 and index < map.Length())
	{
		ControlClick, %cNN%
	}
}

// Returns the text of a field with the given index in a window from a map.  -1 if none found
GetText(map, index) {
	ans := ""
	if (index > 0 and index < map.Length())
	{
		ans := map[index].t
	}
	return ans
}

// Returns the text of a field with the given index in a window from a map.  -1 if none found
SetText(map, index, value) {
	if (index > 0 and index < map.Length())
	{
		ControlClick, %cNN%, %value%
	}
}

// Select a menu from the active window
MenuSelect(m1, m2 := "", m3:= ""){
	if (m2 = "") {
		WinMenuSelectItem,,,m1
	} else {
		if (m3 = "") {
			WinMenuSelectItem,,,m1,m2
		} else {
			WinMenuSelectItem,,,m1,m2,m3
		}
	}
}

// BROWSER CALLS

// Moves to the fields n tabs away
BrowserGetField(offset) {
	SetKeyDelay, 100
	While (offset > 0) {
		Send {TAB}
		offset := offset - 1
	}
	Send {HOME}
	return GetActiveField()
}

// Moves the mouse
BrowserMouseMove(x, y, r := 1) {
	if(r = 1) {
		MouseMove %x%, %y%, 0, R
	} else {
		MouseMove %x%, %y%, 0
	}
}

// Does a mouse click
BrowserMouseClick() {
	MouseClick
}

#CommentFlag ;

; Serializes/Deserializes a JSON object to/from a string
; From: https://github.com/cocobelgica/AutoHotkey-SerDes/blob/master/SerDes.ahk
SerDes(src, out:="", indent:="") {
	if IsObject(src) {
		ret := _SerDes(src, indent)
		if (out == "")
			return ret
		if !(f := FileOpen(out, "w"))
			throw "Failed to open file: '" out "' for writing."
		bytes := f.Write(ret), f.Close()
		return bytes ;// return bytes written when dumping to file
	}
	if FileExist(src) {
		if !(f := FileOpen(src, "r"))
			throw "Failed to open file: '" src "' for reading."
		src := f.Read(), f.Close()
	}
	;// Begin de-serialization routine
	static is_v2 := (A_AhkVersion >= "2"), q := Chr(34) ;// Double quote
	     , push  := Func(is_v2 ? "ObjPush"     : "ObjInsert")
	     , ins   := Func(is_v2 ? "ObjInsertAt" : "ObjInsert")
	     , set   := Func(is_v2 ? "ObjRawSet"   : "ObjInsert")
	     , pop   := Func(is_v2 ? "ObjPop"      : "ObjRemove")
	     , del   := Func(is_v2 ? "ObjRemoveAt" : "ObjRemove")
	static esc_seq := { ;// AHK escape sequences
	(Join Q C
		"``": "``",  ;// accent
		(q):  q,     ;// double quote
		"n":  "`n",  ;// newline
		"r":  "`r",  ;// carriage return
		"b":  "`b",  ;// backspace
		"t":  "`t",  ;// tab
		"v":  "`v",  ;// vertical tab
		"a":  "`a",  ;// alert (bell)
		"f":  "`f"   ;// formfeed
	)}
	;// Extract string literals
	strings := [], i := 0, end := 0-is_v2 ;// v1.1=0, v2.0-a=-1 -> SubStr()
	while (i := InStr(src, q,, i+1)) {
		j := i
		while (j := InStr(src, q,, j+1))
			if (SubStr(str := SubStr(src, i+1, j-i-1), end) != "``")
				break
		if !j
			throw "Missing close quote(s)."
		src := SubStr(src, 1, i) . SubStr(src, j+1)
		k := 0
		while (k := InStr(str, "``",, k+1)) {
			if InStr(q "``nrbtvaf", ch := SubStr(str, k+1, 1))
				str := SubStr(str, 1, k-1) . esc_seq[ch] . SubStr(str, k+2)
			else throw "Invalid escape sequence: '``" . ch . "'" 
		}
		%push%(strings, str) ;// strings.Insert(str) / strings.Push(str)
	}
	;// Begin recursive descent to parse markup
	pos := 0
	, is_key := false ;// if true, active data is to be used as associative array key
	, refs := [], kobj := [] ;// refs=object references, kobj=objects as keys
	, stack := [tree := []]
	, is_arr := Object(tree, 1)
	, next := q "{[01234567890-" ;// chars considered valid when encountered
	while ((ch := SubStr(src, ++pos, 1)) != "") {
		if InStr(" `t`n`r", ch)
			continue
		if !InStr(next, ch) ;// validate current char
			throw "Unexpected char: '" ch "'"
		is_array := is_arr[_obj := stack[1]] ;// active container object
		;// Associative/Linear array opening
		if InStr("{[", ch) {
			val := {}, is_arr[val] := ch == "[", %push%(refs, &val)
			if is_key
				%ins%(kobj, 1, val), key := val
			is_array? %push%(_obj, val) : %set%(_obj, key, is_key ? 0 : val)
			, %ins%(stack, 1, val), is_key := ch == "{"
			, next := q "{[0123456789-$" (is_key ? "}" : "]") ;// Chr(NumGet(ch, "Char")+2)
		}
		;// Associative/Linear array closing
		else if InStr("}]", ch) {
			next := is_arr[stack[2]] ? "]," : "},"
			if (kobj[1] == %del%(stack, 1))
				key := %del%(kobj, 1), next := ":"
		}
		;// Token
		else if InStr(",:", ch) {
			if (_obj == tree)
				throw "Unexpected char: '" ch "' -> there is no container object."
			next := q "{[0123456789-$", is_key := (!is_array && ch == ",")
		}
		;// String | Number | Object reference
		else {
			if (ch == q) { ;// string
				val := %del%(strings, 1)
			} else { ;// number / object reference
				if (is_ref := (ch == "$")) ;// object reference token
					pos += 1
				val := SubStr(src, pos, (SubStr(src, pos) ~= "[\]\}:,\s]|$")-1)
				if (Abs(val) == "")
					throw "Invalid number: " val
				pos += StrLen(val)-1, val += 0
				if is_ref {
					if !ObjHasKey(refs, val)
						throw "Invalid object reference: $" val
					val := Object(refs[val]), is_ref := false
				}
			}
			if is_key
				key := val, next := ":"
			else
				is_array? %push%(_obj, val) : %set%(_obj, key, val)
				, next := is_array ? "]," : "},"
		}
	}
	return tree[1]
}
;// Helper function, serialize object to string -> internal use only
_SerDes(obj, indent:="", lvl:=1, refs:=false) { ;// lvl,refs=internal parameters
	static q := Chr(34) ;// Double quote, for v1.1 & v2.0-a compatibility
	
	if IsObject(obj) {
		/* In v2, an exception is thrown when using ObjGetCapacity() on a
		 * non-standard AHK object (e.g. COM, Func, RegExMatch, File)
		 */
		if (ObjGetCapacity(obj) == "")
			throw "SerDes(): Only standard AHK objects are supported." ; v1.1
		if !refs
			refs := {}
		if ObjHasKey(refs, obj) ;// Object references, includes circular
			return "$" refs[obj] ;// return notation = $(index_of_object)
		refs[obj] := NumGet(&refs + 4*A_PtrSize)+1

		for k in obj
			is_array := k == A_Index
		until !is_array

		if (Abs(indent) != "") {
			spaces := Abs(indent), indent := ""
			Loop % spaces
				indent .= " "
		}
		indt := ""
		Loop % indent ? lvl : 0
			indt .= indent

		lvl += 1, out := "" ;// , len := NumGet(&obj+4*A_PtrSize) -> unreliable
		for k, v in obj {
			if !is_array
				out .= _SerDes(k,,, refs) . ( indent ? ": " : ":" ) ;// object(s) used as keys are not indented
			out .= _SerDes(v, indent, lvl, refs) . ( indent ? ",`n" . indt : "," )
		}
		if (out != "") {
			out := Trim(out, ",`n" . indent)
			if (indent != "")
				out := "`n" . indt . out . "`n" . SubStr(indt, 1, -StrLen(indent)) ;// trim 1 level of indentation
		}
		return is_array ? "[" out "]" : "{" out "}"
	}
	
	else if (ObjGetCapacity([obj], 1) == "")
		return obj
	
	static esc_seq := { ;// AHK escape sequences
	(Join Q C
		(q):  "``" . q,  ;// double quote
		"`n": "``n",     ;// newline
		"`r": "``r",     ;// carriage return
		"`b": "``b",     ;// backspace
		"`t": "``t",     ;// tab
		"`v": "``v",     ;// vertical tab
		"`a": "``a",     ;// alert (bell)
		"`f": "``f"      ;// formfeed
	)}
	i := -1
	while (i := InStr(obj, "``",, i+2))
		obj := SubStr(obj, 1, i-1) . "````" . SubStr(obj, i+1)
	for k, v in esc_seq {
		/* StringReplace/StrReplace workaround routine for v1.1 and v2.0-a
		 * compatibility. TODO: Compare w/ RegExReplace(), use RegExReplace()??
		 */
		i := -1
		while (i := InStr(obj, k,, i+2))
			obj := SubStr(obj, 1, i-1) . v . SubStr(obj, i+1)
	}
	return q . obj . q
}