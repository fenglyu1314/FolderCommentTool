using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TATools.FolderCommentTool
{
    /// <summary>
    /// 文件夹注释Inspector面板
    /// </summary>
    [CanEditMultipleObjects, CustomEditor(typeof(DefaultAsset))]
    public partial class FolderCommentInspector : UnityEditor.Editor
    {
        // 常量定义
        private static readonly Color DefaultTitleColor = new Color(0.4f, 0.8f, 1f);
        private static readonly Color SeparatorColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        private const string PreviewPrefsKey = "FolderCommentTool_ShowPreview";
        private const string CommentTextAreaControlName = "CommentTextArea";
        private const int CommentTextAreaMinHeight = 100;
        // 文件夹路径列表
        private List<string> _folderPaths = new List<string>();

        // 注释标题
        private string _title = string.Empty;

        // 详细注释
        private string _comment = string.Empty;

        // 标题颜色
        private Color _titleColor = DefaultTitleColor;

        // 是否已修改
        private bool _isModified = false;

        // 当前选中的文件夹GUID
        private string _currentGuid = string.Empty;

        // 当前注释数据
        private FolderCommentData _currentData = null;

        // 是否显示预览
        private bool _showPreview = true;

        // 单文件夹编辑模式状态
        private bool _isInEditMode = false;

        // 临时编辑数据（未保存的修改）
        private string _tempTitle = string.Empty;
        private string _tempComment = string.Empty;
        private Color _tempTitleColor = DefaultTitleColor;

        // 是否有未保存的修改
        private bool _hasUnsavedChanges = false;

        // 缓存的样式，避免每帧重复创建
        private GUIStyle _cachedTitleStyle;
        private Color _cachedTitleColor;
        private GUIStyle _cachedCommentStyle;
        private GUIStyle _cachedItalicStyle;

        /// <summary>
        /// Inspector被启用时调用
        /// </summary>
        private void OnEnable()
        {
            // 检查是否切换了目标，如果是则退出编辑模式
            string newGuid = string.Empty;
            if (targets.Length > 0)
            {
                string assetPath = AssetDatabase.GetAssetPath(targets[0]);
                if (AssetDatabase.IsValidFolder(assetPath))
                {
                    newGuid = AssetDatabase.AssetPathToGUID(assetPath);
                }
            }

            // 如果GUID发生变化，说明切换了目标，自动退出编辑模式
            if (!string.IsNullOrEmpty(_currentGuid) && _currentGuid != newGuid && _isInEditMode)
            {
                if (_hasUnsavedChanges)
                {
                    // 如果有未保存的修改，询问用户是否保存
                    if (EditorUtility.DisplayDialog("切换目标", "当前有未保存的修改，是否保存？", "保存", "丢弃"))
                    {
                        SaveChanges();
                    }
                }
                ExitEditMode();
            }

            _folderPaths.Clear();

            // 收集所有选中的文件夹路径
            for (int i = 0; i < targets.Length; i++)
            {
                string assetPath = AssetDatabase.GetAssetPath(targets[i]);
                if (!AssetDatabase.IsValidFolder(assetPath))
                    continue;

                _folderPaths.Add(assetPath);
            }

            // 如果有选中的文件夹，加载第一个文件夹的注释
            if (_folderPaths.Count > 0)
            {
                string path = _folderPaths[0];
                _currentGuid = AssetDatabase.AssetPathToGUID(path);
                _currentData = FolderCommentManager.Instance.GetFolderComment(_currentGuid);

                if (_currentData != null)
                {
                    _title = _currentData.title;
                    _comment = _currentData.comment;
                    _titleColor = _currentData.titleColor;
                }
                else
                {
                    _title = string.Empty;
                    _comment = string.Empty;
                    _titleColor = DefaultTitleColor;
                }
            }

            // 从EditorPrefs加载预览开关状态
            _showPreview = EditorPrefs.GetBool(PreviewPrefsKey, true);

            // 初始化编辑模式状态（如果不在编辑模式）
            if (!_isInEditMode)
            {
                _hasUnsavedChanges = false;
            }

            // 初始化临时编辑数据
            _tempTitle = _title;
            _tempComment = _comment;
            _tempTitleColor = _titleColor;
        }

        /// <summary>
        /// Inspector被销毁时调用
        /// </summary>
        private void OnDestroy()
        {
            // 如果有修改，确保数据被保存
            if (_isModified)
            {
                FolderCommentManager.Instance.SaveDatabase();
            }

            // 如果在编辑模式且有未保存修改，询问是否保存
            if (_isInEditMode && _hasUnsavedChanges)
            {
                if (EditorUtility.DisplayDialog("Inspector关闭", "当前有未保存的修改，是否保存？", "保存", "丢弃"))
                {
                    SaveChanges();
                }
            }
        }

        /// <summary>
        /// 绘制Inspector界面
        /// </summary>
        public override void OnInspectorGUI()
        {
            // 先调用默认的Inspector绘制
            base.OnInspectorGUI();

            // 如果没有选中文件夹，不显示注释编辑界面
            if (_folderPaths.Count == 0)
                return;

            bool enabled = GUI.enabled;
            GUI.enabled = true;

            EditorGUILayout.Space(10);

            // 绘制标题和右键菜单检测区域
            Rect headerRect = EditorGUILayout.GetControlRect();
            EditorGUI.LabelField(headerRect, "文件夹注释", FolderCommentStyles.HeaderLabelStyle);

            // 检测右键点击
            if (Event.current.type == EventType.ContextClick && headerRect.Contains(Event.current.mousePosition))
            {
                ShowContextMenu();
                Event.current.Use();
            }

            EditorGUILayout.Space(5);

            DrawFolderCommentEditor();

            GUI.enabled = enabled;
        }

        /// <summary>
        /// 绘制文件夹注释编辑器
        /// </summary>
        private void DrawFolderCommentEditor()
        {
            // 根据单文件夹编辑模式状态显示不同的界面
            if (_isInEditMode)
            {
                // 编辑模式：显示编辑界面
                DrawEditModeUI();
            }
            else
            {
                // 查看模式：只有当注释不为空时才显示
                if (!string.IsNullOrEmpty(_title) || !string.IsNullOrEmpty(_comment))
                {
                    DrawViewModeUI();
                }
                // 如果标题和注释都为空，不显示任何内容
            }
        }

        /// <summary>
        /// 绘制编辑模式的UI
        /// </summary>
        private void DrawEditModeUI()
        {
            EditorGUI.BeginChangeCheck();

            // 标题输入
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("标题");
            _tempTitle = EditorGUILayout.TextField(_tempTitle, FolderCommentStyles.TitleFieldStyle);
            EditorGUILayout.EndHorizontal();

            // 颜色选择
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("颜色");

            _tempTitleColor = EditorGUILayout.ColorField(_tempTitleColor);
            EditorGUILayout.EndHorizontal();

            // 详细注释
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("注释", EditorStyles.boldLabel);

            // 注释输入框（满行显示）
            GUI.SetNextControlName(CommentTextAreaControlName); // 设置控件名称，用于后续获取焦点
            _tempComment = EditorGUILayout.TextArea(_tempComment, FolderCommentStyles.CommentTextAreaStyle, GUILayout.MinHeight(CommentTextAreaMinHeight));

            // 显示富文本语法说明
            EditorGUILayout.Space(5);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("富文本语法说明", EditorStyles.boldLabel);

            // 使用可选择的文本字段显示语法说明
            EditorGUILayout.SelectableLabel("• <b>文本</b> - 粗体", EditorStyles.label, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            EditorGUILayout.SelectableLabel("• <i>文本</i> - 斜体", EditorStyles.label, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            EditorGUILayout.SelectableLabel("• <size=14>文本</size> - 字体大小", EditorStyles.label, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            EditorGUILayout.SelectableLabel("• <color=#ff0000>文本</color> - 字体颜色", EditorStyles.label, GUILayout.Height(EditorGUIUtility.singleLineHeight));

            EditorGUILayout.EndVertical();

            // 检测是否有修改
            if (EditorGUI.EndChangeCheck())
            {
                MarkAsModified();
            }

            // 添加预览区域
            EditorGUILayout.Space(10);

            // 预览标题和切换开关
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("注释效果预览", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();

            // 检查预览开关状态是否变化
            bool oldShowPreview = _showPreview;
            _showPreview = EditorGUILayout.Toggle("显示预览", _showPreview);

            // 如果状态变化，保存到EditorPrefs
            if (oldShowPreview != _showPreview)
            {
                EditorPrefs.SetBool(PreviewPrefsKey, _showPreview);
            }

            EditorGUILayout.EndHorizontal();

            // 根据开关状态显示预览
            if (_showPreview)
            {
                // 开始带边框的预览区域
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.Space(5);

                // 标题预览
                if (!string.IsNullOrEmpty(_tempTitle))
                {
                    // 绘制标题
                    EditorGUILayout.LabelField(_tempTitle, GetTitleStyle(_tempTitleColor));

                    // 如果有标题和注释，添加一条分隔线
                    if (!string.IsNullOrEmpty(_tempComment))
                    {
                        EditorGUILayout.Space(5);
                        Rect rect = EditorGUILayout.GetControlRect(false, 1);
                        EditorGUI.DrawRect(rect, SeparatorColor);
                        EditorGUILayout.Space(5);
                    }
                }

                // 注释预览
                if (!string.IsNullOrEmpty(_tempComment))
                {
                    // 绘制注释内容
                    EditorGUILayout.LabelField(_tempComment, GetCommentStyle());
                }
                else
                {
                    EditorGUILayout.LabelField("(无注释内容)", GetItalicStyle());
                }

                EditorGUILayout.Space(5);
                EditorGUILayout.EndVertical();

                // 添加提示
                EditorGUILayout.HelpBox("上方预览区域显示了文件夹注释的实际显示效果。", MessageType.Info);
            }

            // 显示时间信息
            if (_currentData != null)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField($"创建时间: {FolderCommentUtils.FormatDateTime(_currentData.CreatedTime)}", FolderCommentStyles.TimeInfoStyle);
                EditorGUILayout.LabelField($"修改时间: {FolderCommentUtils.FormatDateTime(_currentData.ModifiedTime)}", FolderCommentStyles.TimeInfoStyle);
            }

            // 添加保存和取消按钮
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();

            // 保存按钮 - 使用Unity标准样式
            if (GUILayout.Button("保存"))
            {
                SaveChanges();
                ExitEditMode();
            }

            // 取消按钮 - 使用Unity标准样式
            if (GUILayout.Button("取消"))
            {
                if (!_hasUnsavedChanges || EditorUtility.DisplayDialog("取消编辑", "确定要取消编辑吗？未保存的修改将丢失。", "确定", "继续编辑"))
                {
                    CancelChanges();
                    ExitEditMode();
                }
            }

            EditorGUILayout.EndHorizontal();

            // 删除按钮 - 使用Unity标准样式
            EditorGUILayout.Space(5);
            if (GUILayout.Button("删除注释"))
            {
                if (EditorUtility.DisplayDialog("删除注释", "确定要删除这个文件夹的注释吗？", "确定", "取消"))
                {
                    DeleteComment();
                    ExitEditMode();
                }
            }
        }

        /// <summary>
        /// 绘制查看模式的UI
        /// </summary>
        private void DrawViewModeUI()
        {
            // 开始带边框的区域，包裹标题和注释内容
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.Space(5);

            // 标题显示（使用标题样式）
            if (!string.IsNullOrEmpty(_title))
            {
                // 绘制标题
                EditorGUILayout.LabelField(_title, GetTitleStyle(_titleColor));
                EditorGUILayout.Space(5);

                // 如果有标题和注释，添加一条分隔线
                if (!string.IsNullOrEmpty(_comment))
                {
                    Rect rect = EditorGUILayout.GetControlRect(false, 1);
                    EditorGUI.DrawRect(rect, SeparatorColor);
                    EditorGUILayout.Space(5);
                }
            }

            // 注释显示（使用富文本）
            if (!string.IsNullOrEmpty(_comment))
            {
                // 绘制注释内容
                EditorGUILayout.LabelField(_comment, GetCommentStyle());
            }

            EditorGUILayout.Space(5);
            EditorGUILayout.EndVertical();

            // 在非编辑模式下不显示预览区域，但仍然保存预览开关状态
            // 这样在切换回编辑模式时可以恢复用户的偏好设置

            // 显示时间信息
            if (_currentData != null)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField($"创建时间: {FolderCommentUtils.FormatDateTime(_currentData.CreatedTime)}", FolderCommentStyles.TimeInfoStyle);
                EditorGUILayout.LabelField($"修改时间: {FolderCommentUtils.FormatDateTime(_currentData.ModifiedTime)}", FolderCommentStyles.TimeInfoStyle);
            }
        }
    }
}
