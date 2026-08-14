@echo off
setlocal enabledelayedexpansion
title Instalador - Traducao PT-BR - Path of Idle: Old Gods Rising

echo ============================================================
echo   Traducao PT-BR - Path of Idle: Old Gods Rising
echo   (traducao de fa, nao oficial)
echo ============================================================
echo.

set "DEFAULT_PATH=C:\Program Files (x86)\Steam\steamapps\common\PathOfIdle"
set "GAMEDIR=%DEFAULT_PATH%"

if exist "%GAMEDIR%\PathOfIdle.exe" goto :found
echo Nao encontrei o jogo no local padrao do Steam.
echo.
set /p GAMEDIR="Cole aqui o caminho da pasta do jogo (onde fica PathOfIdle.exe): "
if exist "%GAMEDIR%\PathOfIdle.exe" goto :found
echo.
echo ERRO: PathOfIdle.exe nao foi encontrado em "%GAMEDIR%"
echo Verifique o caminho e rode o instalador novamente.
echo.
pause
exit /b 1

:found
echo Jogo encontrado em: %GAMEDIR%
echo.

if exist "%GAMEDIR%\Mods" goto :melonok
echo ============================================================
echo   ATENCAO: o MelonLoader ainda nao esta instalado neste jogo
echo ============================================================
echo.
echo Esta traducao precisa do MelonLoader (framework de mods,
echo gratuito e de codigo aberto^) para funcionar. Antes de continuar:
echo.
echo   1. Baixe o instalador oficial em https://melonwiki.xyz/
echo   2. Abra o instalador e aponte para:
echo      %GAMEDIR%\PathOfIdle.exe
echo   3. Clique em Install
echo   4. Abra o jogo uma vez e feche em seguida
echo      (isso cria as pastas Mods e UserData^)
echo   5. Rode este instalador de novo
echo.
pause
exit /b 1

:melonok
echo Instalando arquivos de traducao em:
echo   %GAMEDIR%
echo.

copy /Y "%~dp0Mods\PtBrTranslation.dll" "%GAMEDIR%\Mods\PtBrTranslation.dll" >nul
if errorlevel 1 goto :erro

copy /Y "%~dp0UserData\ptbr_translation.json" "%GAMEDIR%\UserData\ptbr_translation.json" >nul
if errorlevel 1 goto :erro

copy /Y "%~dp0UserData\en_fallback.json" "%GAMEDIR%\UserData\en_fallback.json" >nul
if errorlevel 1 goto :erro

if exist "%GAMEDIR%\UserData\Loader.cfg" goto :skiploadercfg
copy /Y "%~dp0UserData\Loader.cfg" "%GAMEDIR%\UserData\Loader.cfg" >nul
:skiploadercfg

echo.
echo ============================================================
echo   Instalacao concluida!
echo ============================================================
echo.
echo Proximo passo:
echo   1. Abra o jogo
echo   2. Va em Configuracoes e deixe o idioma em "English"
echo      (esse e o idioma que a traducao PT-BR substitui^)
echo.
echo Bom jogo!
echo.
pause
exit /b 0

:erro
echo.
echo ERRO ao copiar os arquivos. Feche o jogo (se estiver aberto^)
echo e tente rodar o instalador novamente.
echo.
pause
exit /b 1
