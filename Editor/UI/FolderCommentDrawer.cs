using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TATools.FolderCommentTool
{
    /// <summary>
    /// 文件夹注释绘制器
    /// </summary>
    [InitializeOnLoad]
    public class FolderCommentDrawer
    {
        // 样式缓存字典，按颜色缓存样式
        private static readonly Dictionary<Color, GUIStyle> _labelStyleCache = new Dictionary<Color, GUIStyle>();
        private static readonly Dictionary<Color, GUIStyle> _outlineStyleCache = new Dictionary<Color, GUIStyle>();

        // 上次使用的设置值，用于检测设置变化
        private static Color _lastOutlineColor;
        private static bool _lastUseBoldFont;
        private static int _lastListViewFontSize;
        private static int _lastIconViewFontSize;
        private static bool _styleNeedsUpdate = true;

        // 性能优化：缓存GUIContent对象
        private static readonly GUIContent _tempContent = new GUIContent();

        /// <summary>
        /// 静态构造函数，Unity编辑器启动时自动调用
        /// </summary>
        static FolderCommentDrawer()
        {
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;

            // 监听编辑器设置变化，刷新样式缓存
            FolderCommentSettings.OnSettingsChanged += () => {
                _styleNeedsUpdate = true;
                ClearStyleCache();
            };
        }

        /// <summary>
        /// 清除样式缓存
        /// </summary>
        private static void ClearStyleCache()
        {
            _labelStyleCache.Clear();
            _outlineStyleCache.Clear();
        }

        /// <summary>
        /// 获取标签样式
        /// </summary>
        /// <param name="color">文本颜色</param>
        /// <param name="isListView">是否为列表视图</param>
        /// <returns>标签样式</returns>
        private static GUIStyle GetLabelStyle(Color color, bool isListView)
        {
            // 检查缓存中是否已有该颜色的样式
            if (_labelStyleCache.TryGetValue(color, out GUIStyle style))
                return style;

            // 获取基础样式
            GUIStyle baseStyle = isListView
                ? FolderCommentStyles.ListViewLabelStyle
                : FolderCommentStyles.IconViewLabelStyle;

            // 创建新样式
            style = new GUIStyle(baseStyle)
            {
                normal = { textColor = color },
                hover = { textColor = color },
                active = { textColor = color }
            };

            // 缓存样式
            _labelStyleCache[color] = style;
            return style;
        }

        /// <summary>
        /// 获取描边样式
        /// </summary>
        /// <param name="color">描边颜色</param>
        /// <param name="isListView">是否为列表视图</param>
        /// <returns>描边样式</returns>
        private static GUIStyle GetOutlineStyle(Color color, bool isListView)
        {
            // 检查缓存中是否已有该颜色的样式
            if (_outlineStyleCache.TryGetValue(color, out GUIStyle style))
                return style;

            // 获取基础样式
            GUIStyle baseStyle = isListView
                ? FolderCommentStyles.ListViewLabelStyle
                : FolderCommentStyles.IconViewLabelStyle;

            // 创建新样式
            style = new GUIStyle(baseStyle)
            {
                normal = { textColor = color },
                hover = { textColor = color },
                active = { textColor = color }
            };

            // 缓存样式
            _outlineStyleCache[color] = style;
            return style;
        }

        /// <summary>
        /// Project窗口项目绘制回调
        /// </summary>
        /// <param name="guid">资源GUID</param>
        /// <param name="selectionRect">选择区域</param>
        private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            // 获取设置
            var settings = FolderCommentSettings.Instance;

            // 如果功能被禁用，直接返回
            if (!settings.enableFolderComment)
                return;

            // 获取文件夹注释数据
            FolderCommentData commentData = FolderCommentManager.Instance.GetFolderComment(guid);

            // 如果没有注释数据或标题为空，不显示
            if (commentData == null || string.IsNullOrEmpty(commentData.title))
                return;

            // 判断是否为列表视图
            bool isListView = FolderCommentUtils.IsListView(selectionRect);

            // 检查设置是否变化
            if (_styleNeedsUpdate ||
                _lastOutlineColor != settings.outlineColor ||
                _lastUseBoldFont != settings.useBoldFont ||
                _lastListViewFontSize != settings.listViewFontSize ||
                _lastIconViewFontSize != settings.iconViewFontSize)
            {
                // 更新缓存状态
                _lastOutlineColor = settings.outlineColor;
                _lastUseBoldFont = settings.useBoldFont;
                _lastListViewFontSize = settings.listViewFontSize;
                _lastIconViewFontSize = settings.iconViewFontSize;
                _styleNeedsUpdate = false;

                // 清除样式缓存
                ClearStyleCache();
            }

            // 获取样式
            GUIStyle labelStyle = GetLabelStyle(commentData.titleColor, isListView);

            // 设置临时内容
            _tempContent.text = commentData.title;

            // 计算标题尺寸
            Vector2 labelSize = labelStyle.CalcSize(_tempContent);

            // 计算标题位置
            Rect labelRect = CalculateLabelRect(selectionRect, labelSize, isListView, guid);

            // 裁剪文本以适应宽度
            float maxWidth = isListView ? labelRect.width : selectionRect.width;
            _tempContent.text = FolderCommentUtils.CropText(labelStyle, commentData.title, maxWidth);

            // 如果启用了描边
            if (settings.useOutline)
            {
                // 获取描边样式
                GUIStyle outlineStyle = GetOutlineStyle(settings.outlineColor, isListView);

                // 保存原始位置
                Rect originalRect = labelRect;

                // 绘制描边（优化：减少绘制次数）
                labelRect.x += 1;
                EditorGUI.LabelField(labelRect, _tempContent, outlineStyle);
                labelRect.x -= 2;
                EditorGUI.LabelField(labelRect, _tempContent, outlineStyle);

                // 恢复原始位置
                labelRect = originalRect;
            }

            // 绘制主文本
            EditorGUI.LabelField(labelRect, _tempContent, labelStyle);
        }

        /// <summary>
        /// 计算标签矩形区域
        /// </summary>
        /// <param name="selectionRect">选择区域</param>
        /// <param name="labelSize">标签尺寸</param>
        /// <param name="isListView">是否为列表视图</param>
        /// <param name="guid">资源GUID</param>
        /// <returns>标签矩形区域</returns>
        private static Rect CalculateLabelRect(Rect selectionRect, Vector2 labelSize, bool isListView, string guid)
        {
            // 获取设置（性能优化：缓存设置实例）
            var settings = FolderCommentSettings.Instance;

            if (isListView)
            {
                // 列表视图：右侧对齐
                float rightMargin = settings.listViewRightMargin;
                return new Rect(
                    selectionRect.xMax - labelSize.x - rightMargin,
                    selectionRect.y + (selectionRect.height - labelSize.y) * 0.5f, // 垂直居中
                    labelSize.x,
                    labelSize.y);
            }
            else
            {
                // 图标视图：文件夹图标下方，文件名上方
                bool isSmallIcon = FolderCommentUtils.IsSmallIcon(ref selectionRect);

                // 性能优化：减少Selection.objects的使用，只在必要时检查选中状态
                float yOffset = 0f;

                // 计算标签位置 - 调整位置使其不与图标重叠，并在文件名上方保持合适距离
                float yPosition = selectionRect.yMax - labelSize.y - 2f + settings.iconViewVerticalOffset + yOffset;

                return new Rect(
                    selectionRect.x + (selectionRect.width - labelSize.x) * 0.5f,
                    yPosition,
                    labelSize.x,
                    labelSize.y);
            }
        }

        /// <summary>
        /// 检查GUID是否在当前选中对象中
        /// </summary>
        /// <param name="guid">要检查的GUID</param>
        /// <returns>是否选中</returns>
        private static bool IsGuidSelected(string guid)
        {
            if (Selection.objects == null || Selection.objects.Length == 0)
                return false;

            foreach (Object obj in Selection.objects)
            {
                if (AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj)) == guid)
                    return true;
            }

            return false;
        }
    }
}
