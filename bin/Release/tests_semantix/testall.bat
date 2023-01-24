@echo off

set right=0
set total=0
for %%t in (*.in) do (
	set /a total=total+1
	echo %%t
	echo input file:
	type %%t
	echo:
	echo expexted output:
	type %%~nt.ans
	echo:
	Compiler %%t -s > %%~nt.out
	set /a right=right+1
	set /a isok=1
	fc %%~nt.out %%~nt.ans > fc.out && echo OK ||  (set /a right=right-1 && echo actual output: && type %%~nt.out && echo WA)
	echo:
	echo:
)
echo %right% / %total% OK
pause