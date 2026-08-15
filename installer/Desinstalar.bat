@echo off
rem sem "enabledelayedexpansion": ele faz o cmd comer o "!" das mensagens
setlocal
title Desinstalador - Traducao PT-BR - Path of Idle: Old Gods Rising

echo ============================================================
echo   Remover a Traducao PT-BR - Path of Idle: Old Gods Rising
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
echo Verifique o caminho e rode o desinstalador novamente.
echo.
pause
exit /b 1

:found
echo Jogo encontrado em: %GAMEDIR%
echo.
echo Isto vai remover APENAS os arquivos da traducao PT-BR:
echo   - Mods\PtBrTranslation.dll
echo   - UserData\ptbr_translation.json
echo   - UserData\en_fallback.json
echo   - UserData\missing_strings.json (se existir^)
echo   - UserData\last_notified_version.txt (se existir^)
echo.
echo O MelonLoader e seus outros mods NAO serao tocados.
echo Seu save do jogo NAO e afetado.
echo.
set /p CONFIRMA="Digite S para confirmar (ou qualquer outra tecla para cancelar): "
if /i not "%CONFIRMA%"=="S" goto :cancelado

echo.
echo Removendo...

if exist "%GAMEDIR%\Mods\PtBrTranslation.dll" del /q "%GAMEDIR%\Mods\PtBrTranslation.dll"
if exist "%GAMEDIR%\UserData\ptbr_translation.json" del /q "%GAMEDIR%\UserData\ptbr_translation.json"
if exist "%GAMEDIR%\UserData\en_fallback.json" del /q "%GAMEDIR%\UserData\en_fallback.json"
if exist "%GAMEDIR%\UserData\missing_strings.json" del /q "%GAMEDIR%\UserData\missing_strings.json"
if exist "%GAMEDIR%\UserData\last_notified_version.txt" del /q "%GAMEDIR%\UserData\last_notified_version.txt"

if exist "%GAMEDIR%\Mods\PtBrTranslation.dll" goto :erro

echo.
echo ============================================================
echo   Traducao removida!
echo ============================================================
echo.
echo O jogo volta a ficar em ingles na proxima vez que abrir.
echo.
echo Obrigado por ter usado a traducao. Se removeu por causa de
echo algum problema, conta pra gente:
echo https://github.com/xXSirius/pathofidle-ptbr/issues
echo.
pause
exit /b 0

:cancelado
echo.
echo Cancelado, nada foi removido.
echo.
pause
exit /b 0

:erro
echo.
echo ERRO: nao consegui remover o arquivo do mod.
echo Feche o jogo (se estiver aberto^) e rode este desinstalador de novo.
echo.
pause
exit /b 1
