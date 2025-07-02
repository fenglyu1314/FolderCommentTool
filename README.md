# FolderCommentTool - Unity编辑器文件夹注释工具

> **当前版本**: v1.1.0
> **Unity版本要求**: 2022.3+
> **开发者**: 喵喵Mya

一个专业的Unity编辑器扩展工具，为项目文件夹管理提供强大的注释功能，帮助团队更好地组织和理解项目结构。本工程包含完整的插件源码、开发环境和测试工具。

## 🚀 项目概述
![插件效果图](Packages/FolderCommentTool/Documentation~/Images/插件效果图.png)
FolderCommentTool是一个Unity编辑器扩展，允许开发者为项目中的文件夹添加详细注释，并在Project窗口中直观显示。该工具特别适用于大型项目的文件夹管理和团队协作。

## 使用前请先阅读使用文档： [使用文档](https://github.com/fenglyu1314/FolderCommentTool/blob/main/Packages/FolderCommentTool/Documentation%7E/%E4%BD%BF%E7%94%A8%E6%96%87%E6%A1%A3.md)

### 核心特性

- 📁 **直观显示**: 在Project窗口中直接显示文件夹注释标题
- 🎨 **多模式支持**: 支持列表模式和图标模式下的注释显示
- ✏️ **富文本编辑**: 在Inspector面板中编辑详细注释，支持富文本格式
- 🌈 **自定义样式**: 支持自定义颜色，富文本格式（粗体、斜体、大小调整）
- 🔄 **智能关联**: 使用GUID关联，文件夹重命名或移动后注释不丢失
- 🔧 **单文件夹编辑**: 通过右键菜单进入编辑模式，支持未保存修改保护
- ⚙️ **灵活配置**: 完整的设置面板，支持全局开关和样式定制
- 🧪 **完整测试**: 内置测试工具，确保功能稳定性

## 📦 安装方法

### 方式一：Unity Package Manager - Git URL (推荐)

1. 打开Unity编辑器，选择 `Window > Package Manager`
2. 点击左上角的 `+` 按钮，选择 `Add package from git URL...`
3. 输入以下URL：
   ```
   https://github.com/fenglyu1314/FolderCommentTool.git#upm
   ```
4. 点击 `Add` 按钮完成安装

### 方式二：Clone到Packages目录 (开发推荐)

适用于需要修改插件代码或参与开发的情况：

1. 在项目根目录下打开命令行/终端
2. 克隆插件的upm分支到Packages目录：
   ```bash
   git clone -b upm https://github.com/fenglyu1314/FolderCommentTool.git Packages/com.ta.foldercommenttool
   ```
3. Unity会自动检测并加载插件

**优势**：
- ✅ 可以直接修改插件代码
- ✅ 便于调试和开发
- ✅ 支持版本控制
- ✅ 可以提交改进和修复

### 方式三：Clone整个工程 (完整开发)

适用于需要完整开发环境的情况：

1. 在合适的目录下打开命令行/终端
2. 克隆完整工程：
   ```bash
   git clone https://github.com/fenglyu1314/FolderCommentTool.git
   ```
3. 使用Unity打开克隆下来的工程目录
4. 在工程中可以直接修改和测试所有功能

**优势**：
- ✅ 完整的开发环境
- ✅ 包含所有测试用例和示例
- ✅ 可以运行完整的测试流程
- ✅ 适合进行大规模修改或重构

### 方式四：手动安装

1. 下载最新版本的 `.unitypackage` 文件
2. 在Unity中选择 `Assets > Import Package > Custom Package`
3. 选择下载的文件并导入

## 🎯 快速开始

### 基本使用

1. **选择文件夹**：在Project窗口中选择要添加注释的文件夹
2. **进入编辑模式**：在Inspector面板中右键点击"文件夹注释"标题，选择"开启编辑模式"
3. **编辑注释**：输入标题、选择颜色、编写详细注释（支持富文本）
4. **保存注释**：点击"保存"按钮保存并退出编辑模式
5. **查看效果**：注释标题会自动显示在Project窗口的文件夹旁边

### 设置配置

访问 `Edit > Project Settings > TATools > Folder Comment Tool` 进行全局配置：

- 启用/禁用文件夹注释功能
- 调整文字大小和样式
- 自定义UI外观和显示效果

## 🏗️ 工程说明

本工程是FolderCommentTool插件的完整开发环境，包含：

- **插件源码**：位于 `Assets/FolderCommentTool/` 目录，这是实际的UPM包内容
- **开发环境**：完整的Unity工程，用于插件开发和测试
- **测试工具**：内置的测试用例和测试辅助工具
- **文档系统**：完整的开发文档和使用说明
- **发布工具**：自动化的版本发布脚本

### 工程 vs 插件

| 项目 | 用途 | 目标用户 |
|------|------|----------|
| **本工程** | 插件开发、测试、维护 | 插件开发者、贡献者 |
| **UPM包** | 最终用户使用的插件 | Unity项目开发者 |

- **插件README**：[Packages/FolderCommentTool/README.md](Packages/FolderCommentTool/README.md) - 针对插件使用者
- **工程README**：本文件 - 针对插件开发者

## 🛠️ 开发环境

### 项目结构

```
FolderCommentTool/
├── Packages/FolderCommentTool/       # 插件源码 (UPM包)
│   ├── Editor/                       # 编辑器脚本
│   │   ├── Core/                     # 核心功能
│   │   ├── Data/                     # 数据管理
│   │   ├── UI/                       # 用户界面
│   │   ├── Utils/                    # 工具类
│   │   └── Tests/                    # 测试代码
│   ├── Documentation~/               # 文档
│   ├── Tests/                        # 运行时测试
│   ├── package.json                  # UPM包配置
│   ├── README.md                     # 插件说明
│   ├── CHANGELOG.md                  # 版本历史
│   └── LICENSE.md                    # 许可证
├── Assets/TestFolders/               # 测试用文件夹
├── ProjectSettings/                  # 项目设置
│   ├── FolderComments.json          # 注释数据
│   └── FolderCommentSettings.json   # 工具设置
└── fct-release.bat                   # 发布脚本
```

### 技术架构

- **架构模式**: MVC (Model-View-Controller)
- **命名空间**: `TATools.FolderCommentTool`
- **数据存储**: JSON格式，存储在ProjectSettings目录
- **UI框架**: Unity UIElements (IMGUI兼容)
- **测试框架**: Unity Test Framework

## 🔧 开发工具

### 测试工具

通过 `TATools > 文件夹注释工具` 菜单访问：

- **运行所有测试**: 执行完整的功能测试
- **创建测试环境**: 自动创建测试文件夹和注释
- **性能测试**: 测试大量文件夹的性能表现
- **创建测试文件夹**: 快速创建测试用的文件夹结构

### 发布流程

使用项目根目录的 `fct-release.bat` 脚本：

1. 自动从 `package.json` 读取版本号
2. 创建 `upm` 分支并同步插件代码
3. 创建版本标签并推送到远程仓库

```bash
# 运行发布脚本
./fct-release.bat
```

## 📋 版本历史

### v1.1.0 (2025-01-23)
- 📚 **文档优化**：大幅简化使用文档，专注于普通用户需求
- 🎯 **用户体验**：移除开发相关内容，提高文档实用性
- 📝 **结构优化**：重新组织文档章节，逻辑更清晰
- 🧹 **内容精简**：去掉冗余的技术细节和图片说明注释

### v1.0.3 (2025-01-23)
- 🔧 **重大功能改进**：改为单文件夹编辑模式，通过右键菜单进入编辑
- 🛡️ **数据保护**：添加未保存修改保护，切换目标时自动询问是否保存
- 🎨 **UI优化**：移除花哨按钮样式，使用Unity标准样式
- 📝 **富文本简化**：移除自动填写按钮，改为可选择复制的语法说明
- ⚡ **性能优化**：修复内存泄漏，优化GUIStyle缓存，提取常量
- 📚 **文档完善**：全面更新文档，确保与代码功能一致

### v1.0.2 (2025-01-23)
- 🔧 测试工具菜单重组，TATools成为顶层菜单
- 🎯 统一测试工具入口，提升用户体验
- 📁 优化菜单层级结构

### v1.0.1 (2025-01-23)
- 🏗️ 命名空间统一为 `TATools.FolderCommentTool`
- ⚙️ 设置面板重组到TATools分组
- 🧽 代码质量优化，清理未使用变量

### v1.0.0 (2025-01-23)
- 🎉 初始发布
- 📁 完整的文件夹注释功能
- 🎨 富文本支持和自定义样式
- 🔧 完整的测试工具

[查看完整版本历史](Assets/FolderCommentTool/CHANGELOG.md)

## 🤝 贡献指南

### 开发规范

- 遵循MVC架构原则
- 使用TATools命名空间
- 编写单元测试覆盖新功能
- 更新CHANGELOG记录变更

### 提交规范

- `feat:` 新功能
- `fix:` Bug修复
- `docs:` 文档更新
- `refactor:` 代码重构
- `test:` 测试相关

## 📞 联系方式

- **GitHub Issues**: [提交问题](https://github.com/fenglyu1314/FolderCommentTool/issues)
- **邮箱**: fenglyu@foxmail.com

## 📄 许可证

本项目采用专有软件许可协议，需要获得明确授权才可使用。

**重要提示**: 本软件受版权保护，未经授权禁止使用、复制、修改或分发。如需使用请联系授权。

详情请参阅 [LICENSE.md](Packages/FolderCommentTool/LICENSE.md) 文件。

---

*最后更新: 2025-01-23 - v1.1.0 文档优化与用户体验提升*


