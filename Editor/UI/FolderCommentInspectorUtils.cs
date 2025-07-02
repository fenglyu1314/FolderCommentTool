using UnityEditor;
using UnityEngine;

namespace TATools.FolderCommentTool
{
    /// <summary>
    /// 文件夹注释Inspector面板工具类
    /// </summary>
    public partial class FolderCommentInspector
    {
        /// <summary>
        /// 获取标题样式（带缓存）
        /// </summary>
        private GUIStyle GetTitleStyle(Color color)
        {
            if (_cachedTitleStyle == null || _cachedTitleColor != color)
            {
                _cachedTitleStyle = new GUIStyle(EditorStyles.boldLabel);
                _cachedTitleStyle.fontSize = 14;
                _cachedTitleStyle.normal.textColor = color;
                _cachedTitleStyle.richText = true;
                _cachedTitleColor = color;
            }
            return _cachedTitleStyle;
        }

        /// <summary>
        /// 获取注释样式（带缓存）
        /// </summary>
        private GUIStyle GetCommentStyle()
        {
            if (_cachedCommentStyle == null)
            {
                _cachedCommentStyle = new GUIStyle(EditorStyles.wordWrappedLabel);
                _cachedCommentStyle.richText = true;
                _cachedCommentStyle.wordWrap = true;
                _cachedCommentStyle.padding = new RectOffset(10, 10, 0, 0);
            }
            return _cachedCommentStyle;
        }

        /// <summary>
        /// 获取斜体样式（带缓存）
        /// </summary>
        private GUIStyle GetItalicStyle()
        {
            if (_cachedItalicStyle == null)
            {
                _cachedItalicStyle = new GUIStyle(EditorStyles.label);
                _cachedItalicStyle.fontStyle = FontStyle.Italic;
            }
            return _cachedItalicStyle;
        }

        /// <summary>
        /// 显示右键菜单
        /// </summary>
        private void ShowContextMenu()
        {
            GenericMenu menu = new GenericMenu();

            if (!_isInEditMode)
            {
                menu.AddItem(new GUIContent("开启编辑模式"), false, EnterEditMode);
            }
            else
            {
                menu.AddItem(new GUIContent("退出编辑模式"), false, () => {
                    if (_hasUnsavedChanges)
                    {
                        if (EditorUtility.DisplayDialog("退出编辑模式", "当前有未保存的修改，是否保存？", "保存", "丢弃"))
                        {
                            SaveChanges();
                        }
                    }
                    ExitEditMode();
                });
            }

            menu.ShowAsContext();
        }

        /// <summary>
        /// 进入编辑模式
        /// </summary>
        private void EnterEditMode()
        {
            _isInEditMode = true;
            _hasUnsavedChanges = false;

            // 初始化临时编辑数据
            _tempTitle = _title;
            _tempComment = _comment;
            _tempTitleColor = _titleColor;

            Repaint();
        }

        /// <summary>
        /// 退出编辑模式
        /// </summary>
        private void ExitEditMode()
        {
            _isInEditMode = false;
            _hasUnsavedChanges = false;
            Repaint();
        }

        /// <summary>
        /// 标记为已修改
        /// </summary>
        private void MarkAsModified()
        {
            _hasUnsavedChanges = true;
            Repaint();
        }

        /// <summary>
        /// 保存修改
        /// </summary>
        private void SaveChanges()
        {
            // 将临时数据保存到实际数据
            _title = _tempTitle;
            _comment = _tempComment;
            _titleColor = _tempTitleColor;

            // 应用更改到所有选中的文件夹
            for (int i = 0; i < _folderPaths.Count; i++)
            {
                string guid = AssetDatabase.AssetPathToGUID(_folderPaths[i]);
                FolderCommentManager.Instance.SetFolderComment(guid, _title, _comment, _titleColor);
            }

            // 标记为已修改
            _isModified = true;

            // 标记目标为脏，确保Unity保存更改
            EditorUtility.SetDirty(target);

            // 刷新Project窗口，显示更新后的注释
            EditorApplication.RepaintProjectWindow();

            _hasUnsavedChanges = false;
        }

        /// <summary>
        /// 取消修改
        /// </summary>
        private void CancelChanges()
        {
            // 恢复临时数据到原始状态
            _tempTitle = _title;
            _tempComment = _comment;
            _tempTitleColor = _titleColor;

            _hasUnsavedChanges = false;
        }

        /// <summary>
        /// 删除注释
        /// </summary>
        private void DeleteComment()
        {
            for (int i = 0; i < _folderPaths.Count; i++)
            {
                string guid = AssetDatabase.AssetPathToGUID(_folderPaths[i]);
                FolderCommentManager.Instance.RemoveFolderComment(guid);
            }

            _title = string.Empty;
            _comment = string.Empty;
            _titleColor = DefaultTitleColor;
            _currentData = null;

            // 重置临时数据
            _tempTitle = _title;
            _tempComment = _comment;
            _tempTitleColor = _titleColor;

            _isModified = true;
            EditorUtility.SetDirty(target);
            EditorApplication.RepaintProjectWindow();
        }
    }
}
