using UnityEditor;
using UnityEngine;

namespace TATools.FolderCommentTool
{
    /// <summary>
    /// 文件夹注释工具的UI样式定义
    /// </summary>
    public static class FolderCommentStyles
    {
        // 列表视图中的标签样式
        private static GUIStyle _listViewLabelStyle;

        // 图标视图中的标签样式
        private static GUIStyle _iconViewLabelStyle;

        /// <summary>
        /// 清除样式缓存，强制重新创建样式
        /// </summary>
        public static void ClearCache()
        {
            _listViewLabelStyle = null;
            _iconViewLabelStyle = null;
        }

        /// <summary>
        /// 列表视图中的标签样式
        /// </summary>
        public static GUIStyle ListViewLabelStyle
        {
            get
            {
                if (_listViewLabelStyle == null)
                {
                    var settings = FolderCommentSettings.Instance;

                    _listViewLabelStyle = new GUIStyle(settings.useBoldFont ? EditorStyles.boldLabel : EditorStyles.label)
                    {
                        fontSize = settings.listViewFontSize,
                        fontStyle = settings.useBoldFont ? FontStyle.Bold : FontStyle.Normal,
                        alignment = TextAnchor.MiddleRight,
                        wordWrap = false,
                        clipping = TextClipping.Clip,
                        richText = true // 始终启用富文本
                    };
                }
                return _listViewLabelStyle;
            }
        }

        /// <summary>
        /// 图标视图中的标签样式
        /// </summary>
        public static GUIStyle IconViewLabelStyle
        {
            get
            {
                if (_iconViewLabelStyle == null)
                {
                    var settings = FolderCommentSettings.Instance;

                    _iconViewLabelStyle = new GUIStyle(settings.useBoldFont ? EditorStyles.boldLabel : EditorStyles.label)
                    {
                        fontSize = settings.iconViewFontSize,
                        fontStyle = settings.useBoldFont ? FontStyle.Bold : FontStyle.Normal,
                        alignment = TextAnchor.UpperCenter,
                        wordWrap = false,
                        clipping = TextClipping.Clip,
                        richText = true // 始终启用富文本
                    };
                }
                return _iconViewLabelStyle;
            }
        }

        /// <summary>
        /// 标题输入框样式
        /// </summary>
        public static GUIStyle TitleFieldStyle => EditorStyles.textField;

        /// <summary>
        /// 注释文本区域样式
        /// </summary>
        public static GUIStyle CommentTextAreaStyle => EditorStyles.textArea;

        /// <summary>
        /// 时间标签样式
        /// </summary>
        public static GUIStyle TimeInfoStyle
        {
            get
            {
                GUIStyle style = new GUIStyle(EditorStyles.miniLabel)
                {
                    fontSize = 9,
                    normal = { textColor = new Color(0.5f, 0.5f, 0.5f) }
                };
                return style;
            }
        }

        /// <summary>
        /// 标题标签样式
        /// </summary>
        public static GUIStyle HeaderLabelStyle
        {
            get
            {
                GUIStyle style = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 12
                };
                return style;
            }
        }
    }
}
