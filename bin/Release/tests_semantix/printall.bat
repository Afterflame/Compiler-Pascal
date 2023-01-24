@echo off

for %%t in (*.in) do (
	echo %%t
	echo:
	echo input file:
	echo:
	type %%t
	echo:
	Compiler %%t -s > %%~nt.out
	echo:
	type %%~nt.out
	echo:
	echo:
)
pause