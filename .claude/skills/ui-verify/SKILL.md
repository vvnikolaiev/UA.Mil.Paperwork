---
name: ui-verify
description: Runtime-verify Avalonia UI changes in Mil.Paperwork.UI on Windows — launch the app, take screenshots, drive it via UIAutomation/mouse/keyboard, then stop it. Use after implementing UI features or when asked to test the app's behavior live.
---

# UI runtime verification (Avalonia, Windows)

## 1. Launch

```powershell
dotnet run --project Mil.Paperwork.UI
```
Run with `run_in_background: true`. Wait ~6 seconds before interacting.

## 2. Bring the app to the foreground

`SetForegroundWindow` P/Invoke is blocked by Windows — clicks will hit whatever window is actually in front. Use instead:

```powershell
$proc = Get-Process -Name 'Mil.Paperwork.UI'
(New-Object -ComObject WScript.Shell).AppActivate($proc.Id) | Out-Null
Start-Sleep -Milliseconds 500
```

## 3. Screenshot, then Read the file to see it

```powershell
Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing
$b = [System.Windows.Forms.Screen]::PrimaryScreen.Bounds
$bmp = New-Object System.Drawing.Bitmap($b.Width, $b.Height)
[System.Drawing.Graphics]::FromImage($bmp).CopyFromScreen($b.Location, [System.Drawing.Point]::Empty, $b.Size)
$bmp.Save('C:\Temp\ss.png')
```

## 4. Find the window and controls (UIAutomation)

```powershell
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
$desktop = [System.Windows.Automation.AutomationElement]::RootElement
$cond = New-Object System.Windows.Automation.PropertyCondition(
    [System.Windows.Automation.AutomationElement]::NameProperty, 'Mil.Paperwork.UI')
$window = $desktop.FindFirst([System.Windows.Automation.TreeScope]::Children, $cond)
```

**Prefer AutomationId over Name/coordinates.** Most interactive controls in the app now carry a stable `AutomationProperties.AutomationId` (convention: `{ViewName}_{Purpose}`, e.g. `ProductsDictionary_AddButton`, `MessageBox_OkButton`). Find them with:

```powershell
$idCond = New-Object System.Windows.Automation.PropertyCondition(
    [System.Windows.Automation.AutomationElement]::AutomationIdProperty, 'ProductsDictionary_AddButton')
$button = $window.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $idCond)
```

This avoids the garbled-Name and Cyrillic-text-matching issues below. If a control has no `AutomationId` yet (e.g. DataGrid cells inside template columns, dynamically-generated import-preview columns), fall back to ControlType/Name search or coordinates as described next — and when you add or edit a View, add `AutomationId` to its interactive controls too (see project CLAUDE.md / memory on this convention) rather than relying on the fallback.

Find descendants by ControlType (Button, MenuItem, ListItem, ComboBox...) or Name. Button names may come back garbled in PowerShell output, but element order is stable — cross-check counts against a screenshot.

## 5. Interact — known quirks (hard-won)

- **Buttons**: `InvokePattern` works → `$el.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern).Invoke()`.
- **Avalonia MenuItem does NOT support InvokePattern** ("Unsupported Pattern"). Read `$el.Current.BoundingRectangle` and mouse-click its center instead.
- **Dialogs are children of MainWindow**, not top-level windows. Search MainWindow descendants. The custom MessageBoxWindow's buttons have stable AutomationIds (`MessageBox_OkButton`, `MessageBox_YesButton`, `MessageBox_NoButton`, `MessageBox_CancelButton`) — prefer those over text matching. Their visible Content is still Cyrillic («ОК», «Так», «Ні») if you ever need a Name-based fallback.
- **ComboBox**: raw coordinate clicks are unreliable. Use `ExpandCollapsePattern.Expand()` on the ComboBox, then `SelectionItemPattern.Select()` on the target ListItem.
- **Flyouts/menus close between separate PowerShell invocations** — do every multi-step interaction (open flyout → click item → handle dialog) inside ONE script with `Start-Sleep -Milliseconds 400-600` between steps.

Mouse click at screen coordinates:

```powershell
Add-Type @'
using System; using System.Runtime.InteropServices;
public class M {
  [DllImport("user32.dll")] public static extern bool SetCursorPos(int x, int y);
  [DllImport("user32.dll")] public static extern void mouse_event(int f, int x, int y, int c, int e);
  public static void Click(int x, int y) {
    SetCursorPos(x,y); System.Threading.Thread.Sleep(80);
    mouse_event(2,x,y,0,0); System.Threading.Thread.Sleep(50); mouse_event(4,x,y,0,0);
  }
}
'@
[M]::Click(380, 303)
```

DataGrid row center: `grid.Top + ~35 (header) + rowIndex * ~38 + 19`.

Keyboard:

```powershell
Add-Type -AssemblyName System.Windows.Forms
[System.Windows.Forms.SendKeys]::SendWait('{TAB}')   # also +{TAB}, {ENTER}, {ESC}, {UP}, {DOWN}, plain chars
```
Sleep 400–600 ms after each key before screenshotting.

## 6. Test data caveats

- Don't hand-edit `Data/History/index.json` with PowerShell JSON cmdlets — PS 5.1 `ConvertTo-Json` wraps arrays in `{value, Count}` and corrupts the file. If the index gets corrupted, just delete it; the repository rebuilds it from `Data/History/Entries/*.json`.
- String `.Replace()` on JSON files hits every occurrence — prefer creating proper entry files.

## 7. Stop

```powershell
Get-Process -Name 'Mil.Paperwork.UI' -ErrorAction SilentlyContinue | Stop-Process -Force
```

Always stop the app when verification is done.
