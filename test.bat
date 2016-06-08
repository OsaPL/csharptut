@echo off
cd %cd%\csharptut\bin\Debug\
start csharptut.exe 
cd ..
cd ..
cd ..
cd %cd%\client_sharptut\bin\Debug\
copy /y %cd%\client_sharptut.exe %cd%\client_sharptut_copy.exe
start client_sharptut.exe
start client_sharptut_copy.exe
