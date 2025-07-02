# 文件夹注释工具 PRD

## 1. 产品概述

### 1.1 背景
在Unity项目开发过程中，随着项目规模的扩大，资源文件夹数量会不断增加。为了更好地管理和理解各个文件夹的用途，需要一个能够为文件夹添加注释的工具，使团队成员能够快速了解各个文件夹的用途和内容。

### 1.2 目标
开发一个Unity编辑器扩展工具，允许用户为项目中的文件夹添加注释，并在Project窗口中直观地显示这些注释，同时在Inspector面板中提供更详细的注释编辑功能。

## 2. 功能需求

### 2.1 核心功能
1. **文件夹注释显示**
   - 在Project窗口中直接显示文件夹的注释标题
   - 列表模式下：注释右侧对齐显示
   - 图标模式下：注释显示在文件夹图标下方，文件名上方
   - 支持自定义注释文字颜色

2. **注释编辑**
   - 在Inspector面板中编辑文件夹的详细注释
   - 记录注释的创建和修改时间
   - 支持多行注释内容

3. **数据管理**
   - 使用文件夹的GUID进行关联，防止文件夹改名或移动后丢失注释
   - 数据持久化存储，项目重启后保持注释内容

### 2.2 用户界面
1. **Project窗口显示**
   - 注释标题显示在文件夹旁边，不影响原有UI
   - 文字颜色可自定义，默认为浅蓝色
   - 文字大小适中，确保可读性

2. **Inspector面板**
   - 选中文件夹后，在Inspector中显示注释编辑区域
   - 包含标题输入框、颜色选择器、详细注释文本区域
   - 显示注释的创建和最后修改时间

## 3. 技术规格

### 3.1 数据结构
```csharp
// 文件夹注释数据
[Serializable]
public class FolderCommentData
{
    public string guid;           // 文件夹GUID
    public string title;          // 注释标题（显示在Project窗口）
    public string comment;        // 详细注释
    public Color titleColor;      // 标题颜色
    public DateTime createdTime;  // 创建时间
    public DateTime modifiedTime; // 最后修改时间
}

// 数据容器
[CreateAssetMenu(fileName = "FolderComments", menuName = "FolderCommentTool/Comments Database")]
public class FolderCommentsDatabase : ScriptableObject
{
    public List<FolderCommentData> comments = new List<FolderCommentData>();
}
```

### 3.2 存储方式
- 使用ScriptableObject存储所有文件夹注释数据
- 数据文件保存在项目的`ProjectSettings`目录下
- 自动保存机制，确保数据不会丢失

## 4. 代码框架

### 4.1 目录结构
```
Assets/Editor/FolderComment/
├── Core/
│   ├── FolderCommentData.cs         // 数据结构定义
│   ├── FolderCommentsDatabase.cs    // 数据存储容器
│   └── FolderCommentManager.cs      // 核心管理类
├── Editor/
│   ├── FolderCommentDrawer.cs       // Project窗口绘制
│   └── FolderCommentInspector.cs    // Inspector面板绘制
└── Utils/
    ├── FolderCommentStyles.cs       // UI样式定义
    └── FolderCommentUtils.cs        // 工具函数
```

### 4.2 核心类设计

#### FolderCommentManager
- 单例模式，管理所有文件夹注释数据
- 提供添加、修改、删除注释的API
- 负责数据的加载和保存

#### FolderCommentDrawer
- 实现`EditorApplication.projectWindowItemOnGUI`回调
- 根据不同视图模式（列表/图标）绘制注释标题
- 处理文字裁剪和位置计算

#### FolderCommentInspector
- 自定义Inspector，用于编辑文件夹注释
- 提供标题、颜色、详细注释的编辑界面
- 显示创建和修改时间信息

## 5. 实现计划

### 5.1 阶段一：基础框架
- 创建数据结构和存储类
- 实现数据管理器的基本功能
- 搭建Inspector编辑界面

### 5.2 阶段二：UI绘制
- 实现Project窗口中的注释显示
- 处理不同视图模式下的位置计算
- 实现文字裁剪和颜色显示

### 5.3 阶段三：功能完善
- 添加时间记录功能
- 优化UI交互体验
- 完善数据保存和加载机制

### 5.4 阶段四：测试和优化
- 创建测试用例
- 性能优化
- 修复潜在问题

## 6. 注意事项
- 确保工具不影响Unity编辑器的性能
- 注意处理多人协作时的数据同步问题
- 考虑大型项目中可能有大量文件夹的情况
