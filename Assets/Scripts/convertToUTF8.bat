@echo off
chcp 65001 >nul
echo 正在将Scripts文件夹下的.cs文件转换为UTF-8编码...
echo.

for /r "S:\UnityProjects\GameJam\Assets\Scripts" %%f in (*.cs) do (
    echo 转换: %%f
    powershell -Command "$content = Get-Content '%%f' -Encoding Default; $Utf8NoBomEncoding = New-Object System.Text.UTF8Encoding $False; [System.IO.File]::WriteAllLines('%%f', $content, $Utf8NoBomEncoding)"
)

echo.
echo 转换完成！
pause