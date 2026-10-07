Unicode true
SetCompressor /SOLID lzma
SetDatablockOptimize on

;--------------------------------
!define PRODUCT_VERSION "0.1.0"
!define SETUP_NAME "GreatCompany"
!define EXE_NAME "GreatCompany"
!define PRODUCT_NAME "QS: Великая компания"
!define SHORTCUT_NAME "QS Великая компания"
!define MENU_DIR_NAME "Великая компания"
!define APP_DIR_NAME "Великая компания"
!define UNINSTAL_KEY "QSGreatCompany"

; The name of the installer
Name "${PRODUCT_NAME}"

; The file to write
OutFile "${SETUP_NAME}-${PRODUCT_VERSION}.exe"

!include "MUI.nsh"
!include "x64.nsh"
!include "dotnetcore.nsh"

; The default installation directory
InstallDir "$PROGRAMFILES64\${APP_DIR_NAME}"

; Request application privileges for Windows Vista
RequestExecutionLevel admin

;--------------------------------
; Pages

!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!define MUI_FINISHPAGE_RUN "$INSTDIR\${EXE_NAME}.exe"
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

;--------------------------------
;Languages

!insertmacro MUI_LANGUAGE "Russian"

;--------------------------------
; The stuff to install
Section "${PRODUCT_NAME}" SecProgram

  SectionIn RO

  SetOutPath $INSTDIR

  ; Put files there
  File /r "Files\*.*"

  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${UNINSTAL_KEY}" "DisplayName" "${PRODUCT_NAME}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${UNINSTAL_KEY}" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${UNINSTAL_KEY}" "DisplayIcon" '"$INSTDIR\${EXE_NAME}.exe"'
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${UNINSTAL_KEY}" "Publisher" "Quality Solution"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${UNINSTAL_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${UNINSTAL_KEY}" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${UNINSTAL_KEY}" "NoRepair" 1
  WriteUninstaller "uninstall.exe"

  ; Start Menu Shortcuts
  SetShellVarContext all
  CreateDirectory "$SMPROGRAMS\${MENU_DIR_NAME}"
  CreateShortCut "$SMPROGRAMS\${MENU_DIR_NAME}\Удаление.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  CreateShortCut "$SMPROGRAMS\${MENU_DIR_NAME}\${SHORTCUT_NAME}.lnk" "$INSTDIR\${EXE_NAME}.exe" "" "$INSTDIR\${EXE_NAME}.exe" 0

SectionEnd

Section ".NET 10.0 Desktop Runtime" SecDotNet10
  SectionIn RO

  !insertmacro CheckDotNetCore 10.0

SectionEnd

Section "Ярлык на рабочий стол" SecDesktop

  SetShellVarContext all

  SetOutPath $INSTDIR
  CreateShortCut "$DESKTOP\${SHORTCUT_NAME}.lnk" "$INSTDIR\${EXE_NAME}.exe" "" "$INSTDIR\${EXE_NAME}.exe" 0

SectionEnd

;--------------------------------
;Descriptions

  LangString DESC_SecProgram ${LANG_Russian} "Основные файлы программы"
  LangString DESC_SecDotNet10 ${LANG_Russian} ".NET Desktop Runtime, необходимый для работы программы. При необходимости будет выполнена установка через интернет."
  LangString DESC_SecDesktop ${LANG_Russian} "Установит ярлык программы на рабочий стол"

  !insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
    !insertmacro MUI_DESCRIPTION_TEXT ${SecProgram} $(DESC_SecProgram)
    !insertmacro MUI_DESCRIPTION_TEXT ${SecDotNet10} $(DESC_SecDotNet10)
    !insertmacro MUI_DESCRIPTION_TEXT ${SecDesktop} $(DESC_SecDesktop)
  !insertmacro MUI_FUNCTION_DESCRIPTION_END

;--------------------------------
; Uninstaller

Section "Uninstall"

  SetShellVarContext all
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${UNINSTAL_KEY}"

  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\${MENU_DIR_NAME}\*.*"
  Delete "$DESKTOP\${SHORTCUT_NAME}.lnk"
  RMDir "$SMPROGRAMS\${MENU_DIR_NAME}"

  ; Remove files and uninstaller
  RMDir /r "$INSTDIR"

SectionEnd
