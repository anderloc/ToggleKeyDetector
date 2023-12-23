;--------------------------------
;Include Modern UI

!include "MUI2.nsh"

;--------------------------------
;General

!define PUBLISH_X86_DIR "..\ToggleKeyDetector\bin\Release\net6.0-windows\publish\win-x86"
!define APP_EXECUTABLE "ToggleKeyDetector.exe"
!define APP_NAME "Toggle Key Detector"

;Name and file
Name "${APP_NAME}"
OutFile "ToggleKeyDetector_Installer.exe"

;Default installation folder
InstallDir "$PROGRAMFILES\ToggleKeyDetector"

;Request application privileges for Windows Vista
RequestExecutionLevel admin

; Define path to Executable
!define MUI_FINISHPAGE_RUN "$INSTDIR\${APP_EXECUTABLE}"

!macro CheckIfAppIsRunning
  System::Call 'kernel32::OpenMutex(i 0x100000, i 0, t "ToggleKeyDetectorMutex")p.R0'
  IntPtrCmp $R0 0 notRunning
    System::Call 'kernel32::CloseHandle(p $R0)'
    MessageBox MB_OK|MB_ICONSTOP "${APP_NAME} is running. Please close it first" /SD IDOK
    Abort
  notRunning:
    # Continue
!macroend

;--------------------------------
;Pages

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH

;--------------------------------
;Languages

!insertmacro MUI_LANGUAGE "English"

;--------------------------------
;Installer Sections

Section "Dummy Section" SecDummy
  !insertmacro CheckIfAppIsRunning

  SetOutPath "$INSTDIR"

  File "${PUBLISH_X86_DIR}\D3DCompiler_47_cor3.dll"
  File "${PUBLISH_X86_DIR}\PenImc_cor3.dll"
  File "${PUBLISH_X86_DIR}\PresentationNative_cor3.dll"
  File "${PUBLISH_X86_DIR}\ToggleKeyDetector.exe"
  File "${PUBLISH_X86_DIR}\vcruntime140_cor3.dll"
  File "${PUBLISH_X86_DIR}\wpfgfx_cor3.dll"

  CreateDirectory "$INSTDIR\Icons"
  SetOutPath "$INSTDIR\Icons"

  File "${PUBLISH_X86_DIR}\Icons\Lowercase.ico"
  File "${PUBLISH_X86_DIR}\Icons\Uppercase.ico"
  File "${PUBLISH_X86_DIR}\Icons\NumLock.ico"
  File "${PUBLISH_X86_DIR}\Icons\ScrollLock.ico"
  
  ;Create uninstaller
  WriteUninstaller "$INSTDIR\Uninstall.exe"

  ;Store installation folder
  WriteRegStr HKCU "Software\${APP_NAME}" "" $INSTDIR

  ; Register App to run on startup
  SetOutPath "$INSTDIR"
  CreateShortCut "$SMPROGRAMS\Startup\${APP_EXECUTABLE}.lnk" "$INSTDIR\${APP_EXECUTABLE}"

  CreateDirectory '$SMPROGRAMS\${APP_NAME}'
  CreateShortCut '$SMPROGRAMS\${APP_NAME}\${APP_NAME}.lnk' '$INSTDIR\${APP_EXECUTABLE}' "" '$INSTDIR\${APP_EXECUTABLE}' 0
  CreateShortCut '$SMPROGRAMS\${APP_NAME}\Uninstall ${APP_NAME}.lnk' '$INSTDIR\Uninstall.exe' "" '$INSTDIR\Uninstall.exe' 0

  ; Uninstall program
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}" "DisplayName" "${APP_NAME}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}" "UninstallString" "$INSTDIR\Uninstall.exe"

SectionEnd

;--------------------------------
;Uninstaller Section

Section "Uninstall"

  !insertmacro CheckIfAppIsRunning

  Delete "$INSTDIR\D3DCompiler_47_cor3.dll"
  Delete "$INSTDIR\PenImc_cor3.dll"
  Delete "$INSTDIR\PresentationNative_cor3.dll"
  Delete "$INSTDIR\ToggleKeyDetector.exe"
  Delete "$INSTDIR\vcruntime140_cor3.dll"
  Delete "$INSTDIR\wpfgfx_cor3.dll"

  Delete "$INSTDIR\Icons\Lowercase.ico"
  Delete "$INSTDIR\Icons\Uppercase.ico"
  Delete "$INSTDIR\Icons\NumLock.ico"
  Delete "$INSTDIR\Icons\ScrollLock.ico"
  RMDir "$INSTDIR\Icons"


  Delete "$INSTDIR\Uninstall.exe"
  Delete "$SMPROGRAMS\Startup\${APP_EXECUTABLE}.lnk"

  Delete '$SMPROGRAMS\${APP_NAME}\${APP_NAME}.lnk'
  Delete '$SMPROGRAMS\${APP_NAME}\Uninstall ${APP_NAME}.lnk'
  RMDir '$SMPROGRAMS\${APP_NAME}'

  RMDir "$INSTDIR"

  DeleteRegKey /ifempty HKCU "Software\${APP_NAME}"
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}"

SectionEnd
