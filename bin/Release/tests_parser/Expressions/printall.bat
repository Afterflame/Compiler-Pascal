@echo off

for %%t in (*.in) do (
	echo %%t
	echo:
	echo input file:
	echo:
	type %%t
	echo:
	Compiler %%t -p > %%~nt.out
	echo:
	type %%~nt.out
	echo:
	echo:
	set /p input=
)
pause