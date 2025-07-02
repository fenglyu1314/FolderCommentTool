using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TATools.FolderCommentTool
{
    /// <summary>
    /// 文件夹注释工具设置
    /// </summary>
    [Serializable]
    public class FolderCommentSettings
    {
        // 设置变更事件委托
        public delegate void SettingsChangedHandler();

        // 设置变更事件
        public static event SettingsChangedHandler OnSettingsChanged;

        // 单例实例
        private static FolderCommentSettings _instance;

        // 设置文件路径
        private static readonly string SettingsPath = "ProjectSettings/FolderCommentSettings.json";

        /// <summary>
        /// 是否启用文件夹注释功能
        /// </summary>
        public bool enableFolderComment = true;

        /// <summary>
        /// 列表视图中的文字大小
        /// </summary>
        public int listViewFontSize = 11;

        /// <summary>
        /// 图标视图中的文字大小
        /// </summary>
        public int iconViewFontSize = 11;

        /// <summary>
        /// 是否使用粗体
        /// </summary>
        public bool useBoldFont = true;

        /// <summary>
        /// 是否使用描边
        /// </summary>
        public bool useOutline = false;

        /// <summary>
        /// 描边颜色
        /// </summary>
        public Color outlineColor = new Color(0, 0, 0, 0.5f);

        /// <summary>
        /// 列表视图中的右侧边距
        /// </summary>
        public float listViewRightMargin = 8f;

        /// <summary>
        /// 图标视图中的垂直偏移
        /// </summary>
        public float iconViewVerticalOffset = 2f;

        /// <summary>
        /// 获取单例实例
        /// </summary>
        public static FolderCommentSettings Instance
        {
            get
            {
                // 每次获取实例时都重新加载设置，确保获取最新设置
                if (_instance == null)
                {
                    _instance = Load();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 重新加载设置
        /// </summary>
        public static void ReloadSettings()
        {
            _instance = Load();

            // 清除样式缓存，强制重新创建样式
            FolderCommentStyles.ClearCache();

            // 刷新Project窗口
            EditorApplication.RepaintProjectWindow();
        }

        /// <summary>
        /// 加载设置
        /// </summary>
        /// <returns>设置实例</returns>
        private static FolderCommentSettings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    string json = File.ReadAllText(SettingsPath);
                    FolderCommentSettings settings = JsonUtility.FromJson<FolderCommentSettings>(json);
                    if (settings != null)
                    {
                        return settings;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"加载文件夹注释设置时出错: {e.Message}");
            }

            // 如果加载失败或文件不存在，返回默认设置
            return new FolderCommentSettings();
        }

        /// <summary>
        /// 保存设置
        /// </summary>
        public void Save()
        {
            try
            {
                string json = JsonUtility.ToJson(this, true);
                File.WriteAllText(SettingsPath, json);

                // 重新加载设置，确保使用最新设置
                ReloadSettings();

                // 触发设置变更事件
                OnSettingsChanged?.Invoke();

                // 刷新所有Inspector窗口
                RefreshInspectorWindows();
            }
            catch (Exception e)
            {
                Debug.LogError($"保存文件夹注释设置时出错: {e.Message}");
            }
        }

        /// <summary>
        /// 刷新所有Inspector窗口
        /// </summary>
        private static void RefreshInspectorWindows()
        {
            foreach (var window in Resources.FindObjectsOfTypeAll<EditorWindow>())
            {
                if (window.titleContent.text == "Inspector")
                {
                    window.Repaint();
                }
            }
        }
    }

    /// <summary>
    /// 文件夹注释设置提供者
    /// </summary>
    internal class FolderCommentSettingsProvider : SettingsProvider
    {
        // 设置实例
        private SerializedObject _serializedSettings;

        // 设置对象
        private FolderCommentSettings _settings;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path">设置路径</param>
        /// <param name="scopes">设置范围</param>
        public FolderCommentSettingsProvider(string path, SettingsScope scopes) : base(path, scopes)
        {
            _settings = FolderCommentSettings.Instance;
        }

        /// <summary>
        /// 绘制设置界面
        /// </summary>
        /// <param name="searchContext">搜索上下文</param>
        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("文件夹注释工具设置", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            // 检查是否有任何设置更改
            EditorGUI.BeginChangeCheck();

            // 基本设置
            EditorGUILayout.LabelField("基本设置", EditorStyles.boldLabel);
            _settings.enableFolderComment = EditorGUILayout.Toggle("启用文件夹注释", _settings.enableFolderComment);
            EditorGUILayout.Space(10);

            // 文字样式设置
            EditorGUILayout.LabelField("文字样式", EditorStyles.boldLabel);
            _settings.listViewFontSize = EditorGUILayout.IntSlider("列表视图文字大小", _settings.listViewFontSize, 8, 16);
            _settings.iconViewFontSize = EditorGUILayout.IntSlider("图标视图文字大小", _settings.iconViewFontSize, 8, 16);
            _settings.useBoldFont = EditorGUILayout.Toggle("使用粗体", _settings.useBoldFont);
            EditorGUILayout.Space(10);

            // 描边设置
            EditorGUILayout.LabelField("描边设置", EditorStyles.boldLabel);
            _settings.useOutline = EditorGUILayout.Toggle("使用描边", _settings.useOutline);
            if (_settings.useOutline)
            {
                EditorGUI.indentLevel++;
                _settings.outlineColor = EditorGUILayout.ColorField("描边颜色", _settings.outlineColor);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space(10);

            // 位置设置
            EditorGUILayout.LabelField("位置设置", EditorStyles.boldLabel);
            _settings.listViewRightMargin = EditorGUILayout.Slider("列表视图右侧边距", _settings.listViewRightMargin, 0f, 20f);
            _settings.iconViewVerticalOffset = EditorGUILayout.Slider("图标视图垂直偏移", _settings.iconViewVerticalOffset, -10f, 10f);

            // 如果有任何设置更改，立即保存并应用
            if (EditorGUI.EndChangeCheck())
            {
                _settings.Save();
            }
            EditorGUILayout.Space(10);

            // 恢复默认按钮
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("恢复默认", GUILayout.Width(120)))
            {
                _settings = new FolderCommentSettings();
                _settings.Save();
                AssetDatabase.Refresh();
                EditorApplication.RepaintProjectWindow();
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 创建设置提供者
        /// </summary>
        /// <returns>设置提供者</returns>
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new FolderCommentSettingsProvider("Project/TATools/Folder Comment Tool", SettingsScope.Project);
        }
    }
}
