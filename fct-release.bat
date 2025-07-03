@echo off
setlocal enabledelayedexpansion
chcp 65001 >nul

REM ===== FolderCommentTool 一键发布工具 =====
REM 功能: 自动读取版本号并发布Unity插件到UPM分支
REM 作者: 喵喵Mya
REM ==========================================

echo.
echo ========================================
echo   FolderCommentTool 一键发布工具
echo ========================================
echo 工作目录: %CD%
echo 当前时间: %date% %time%
echo.

REM ===== 配置变量 =====
SET ToolName=upm
SET ToolAssetPath=Packages/FolderCommentTool
SET ToolAssetPathWin=Packages\FolderCommentTool
SET RepoUrl=https://github.com/fenglyu1314/FolderCommentTool.git

REM ===== 环境检查 =====
echo [1/6] 检查环境和文件...

REM 检查Git是否可用
call :CheckCommand "git --version" "Git 未安装或不在PATH中" "请安装Git并确保可以在命令行中使用"

REM 检查是否在Git仓库中
call :CheckCommand "git rev-parse --git-dir" "当前目录不是Git仓库" "请在Git仓库根目录下运行此脚本"

REM 检查package.json文件
echo 📁 检查文件: %ToolAssetPathWin%\package.json
if not exist "%ToolAssetPathWin%\package.json" (
    echo ❌ 错误: 找不到 package.json 文件
    echo 路径: %ToolAssetPathWin%\package.json
    pause
    exit /b 1
)
echo ✅ package.json 文件存在

echo.
echo [2/6] 读取版本号...
call :ReadVersion "%ToolAssetPathWin%\package.json"

if "%ToolVersion%"=="" (
    echo ❌ 错误: 无法读取版本号
    pause
    exit /b 1
)
echo ✅ 检测到版本: %ToolVersion%

echo.
echo [3/6] 检查版本标签...
git tag -l %ToolVersion% | findstr "^%ToolVersion%$" >nul 2>&1
set tag_exists=!errorlevel!
if !tag_exists! equ 0 (
    echo ⚠️  警告: 标签 %ToolVersion% 已存在
    echo 💡 提示: 稍后将询问是否覆盖现有标签
) else (
    echo ✅ 版本标签不存在，可以创建新标签
)

echo.
echo [4/6] 创建UPM分支...

REM 删除现有的UPM分支（如果存在）
call :DeleteBranchIfExists %ToolName%

REM 删除远程UPM分支（如果存在）
git ls-remote --heads origin %ToolName% | findstr %ToolName% >nul 2>&1
if !errorlevel! equ 0 (
    echo 🗑️  删除远程 %ToolName% 分支...
    git push origin --delete %ToolName% >nul 2>&1
    echo ✅ 远程分支已删除
)

echo 🔄 正在使用git subtree split创建 %ToolName% 分支...
git subtree split -P %ToolAssetPath% -b %ToolName%

if !errorlevel! neq 0 (
    echo ❌ 错误: 创建分支失败
    echo 请检查路径是否正确: %ToolAssetPath%
    pause
    exit /b 1
)
echo ✅ UPM分支创建成功

echo.
echo [5/6] 创建版本标签...

REM 使用第3步检查的结果
if !tag_exists! equ 0 (
    echo ⚠️  标签 %ToolVersion% 已存在
    echo.
    echo 选项:
    echo [1] 覆盖现有标签
    echo [2] 取消发布
    echo.
    set /p choice="请选择 (1/2): "

    if "!choice!"=="1" (
        echo 🗑️  删除现有标签...
        git tag -d %ToolVersion% >nul 2>&1
        git push origin :refs/tags/%ToolVersion% >nul 2>&1
        echo ✅ 现有标签已删除
    ) else (
        echo ❌ 发布已取消
        echo 💡 提示: 请更新 package.json 中的版本号后重试
        pause
        exit /b 1
    )
)

echo 🏷️  正在创建标签 %ToolVersion%...

REM 获取UPM分支的最新提交ID
for /f %%i in ('git rev-parse %ToolName%') do set UMP_COMMIT=%%i

REM 在UPM分支的提交上创建标签（不需要切换分支）
git tag %ToolVersion% %UMP_COMMIT%
if !errorlevel! neq 0 (
    echo ❌ 错误: 创建标签失败
    pause
    exit /b 1
)
echo ✅ 版本标签创建成功

echo.
echo [6/6] 推送到远程仓库...
echo 🚀 正在推送 %ToolName% 分支...
call :PushToRemote "git push origin %ToolName%" "分支推送失败"

echo 🚀 正在推送 %ToolVersion% 标签...
call :PushToRemote "git push origin %ToolVersion%" "标签推送失败"

echo.
echo ========================================
echo 🎉 发布完成!
echo ========================================
echo 📦 插件版本: %ToolVersion%
echo 🌿 UPM分支: %ToolName%
echo 🔗 安装地址: %RepoUrl%#%ToolName%
echo.
echo 💡 用户现在可以通过Unity Package Manager安装此版本
pause

REM ===== 函数定义 =====

:ReadVersion
REM 读取package.json中的版本号
setlocal enabledelayedexpansion
set "json_file=%~1"
set "ToolVersion="

REM 逐行读取文件，查找version行
for /f "usebackq tokens=*" %%a in ("%json_file%") do (
    set "line=%%a"
    REM 检查是否包含"version"且不包含"Url"（避免匹配URL中的version）
    echo !line! | findstr /i "version" | findstr /v /i "Url" >nul
    if !errorlevel! equ 0 (
        REM 提取版本号：先按冒号分割，再清理引号和逗号
        for /f "tokens=2 delims=:" %%b in ("!line!") do (
            set "version_raw=%%b"
            REM 移除空格
            set "version_raw=!version_raw: =!"
            REM 移除引号
            set "version_raw=!version_raw:"=!"
            REM 移除逗号
            set "version_raw=!version_raw:,=!"
            set "ToolVersion=!version_raw!"
            goto :version_extracted
        )
    )
)

:version_extracted
endlocal & set "ToolVersion=%ToolVersion%"
goto :eof

:CheckCommand
REM 检查命令是否成功执行
REM 参数: %1=命令, %2=错误信息, %3=建议
%~1 >nul 2>&1
if !errorlevel! neq 0 (
    echo ❌ 错误: %~2
    echo %~3
    pause
    exit /b 1
)
goto :eof

:DeleteBranchIfExists
REM 检查并删除指定分支
REM 参数: %1=分支名
git show-ref --verify --quiet refs/heads/%1
if !errorlevel! equ 0 (
    echo 🗑️  删除现有的 %1 分支...
    git branch -D %1 >nul 2>&1
    if !errorlevel! neq 0 (
        echo ❌ 错误: 无法删除现有分支 %1
        pause
        exit /b 1
    )
    echo ✅ 现有分支已删除
)
goto :eof

:PushToRemote
REM 推送到远程仓库
REM 参数: %1=推送命令, %2=错误信息
%~1 >nul 2>&1
if !errorlevel! neq 0 (
    echo ❌ 错误: %~2
    echo 请检查网络连接和Git权限
    pause
    exit /b 1
)
echo ✅ 推送成功
goto :eof