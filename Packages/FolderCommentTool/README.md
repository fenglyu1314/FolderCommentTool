# Folder Comment Tool

> 版本：v1.3.0
>
> 📖 **说明**：这是插件的使用说明文档。如果您是插件开发者，请查看[工程README](../../README.md)。

一个Unity编辑器扩展工具，允许用户为项目中的文件夹添加注释，并在Project窗口中直观地显示这些注释。

## 功能特点

- 📁 在Project窗口中直接显示文件夹的注释标题
- 🎨 支持列表模式和图标模式下的注释显示
- ✏️ 在Inspector面板中编辑文件夹的详细注释
- 🌈 支持自定义注释文字颜色
- 📝 支持富文本格式（粗体、斜体、颜色、大小等），提供可复制的语法说明
- ⏰ 记录注释的创建和修改时间
- 🎯 使用文件夹的GUID进行关联，防止文件夹改名或移动后丢失注释
- 🔄 支持单文件夹编辑模式，通过右键菜单进入，具有未保存修改保护
- 🔧 提供完整的测试工具

## 安装方法

### 通过Unity Package Manager安装

1. 打开Unity编辑器，选择 Window > Package Manager
2. 点击左上角的 "+" 按钮，选择 "Add package from git URL..."
3. 输入 `https://github.com/fenglyu1314/FolderCommentTool.git#upm`
4. 点击 "Add" 按钮

### 手动安装

1. 下载最新版本的.unitypackage文件
2. 在Unity中选择 Assets > Import Package > Custom Package
3. 选择下载的.unitypackage文件并导入

## 使用方法

1. 在Project窗口中选择一个文件夹
2. 在Inspector面板中右键点击"文件夹注释"标题
3. 选择"开启编辑模式"进入编辑界面
4. 编辑文件夹的注释内容
5. 点击"保存"按钮保存注释
6. 注释会自动显示在Project窗口中

## 设置

可以在 Edit > Project Settings > TATools > Folder Comment Tool 中找到设置面板，可以自定义以下选项：

- 启用/禁用文件夹注释功能
- 调整文字大小和样式
- 自定义UI外观

## 兼容性

- **Unity版本**：2022.3及以上版本
- **平台支持**：Windows、macOS、Linux
- **包格式**：标准Unity Package格式，支持UPM

## 问题反馈

- **GitHub Issues**：https://github.com/fenglyu1314/FolderCommentTool/issues
- **邮箱**：fenglyu@foxmail.com

## 许可证

本项目采用 MIT 许可证，您可以自由使用、修改和分发。详情请参阅 [LICENSE.md](LICENSE.md) 文件。
