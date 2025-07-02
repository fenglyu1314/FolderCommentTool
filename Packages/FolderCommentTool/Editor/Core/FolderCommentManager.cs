using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TATools.FolderCommentTool
{
    /// <summary>
    /// 文件夹注释管理器
    /// </summary>
    [InitializeOnLoad]
    public class FolderCommentManager
    {
        // 单例实例
        private static FolderCommentManager _instance;

        // 旧数据库文件路径（用于数据迁移）
        private static readonly string OldDatabasePath = Path.Combine("Assets", "FolderCommentTool", "Editor", "Data", "FolderComments.asset");

        // 数据库实例
        private IFolderCommentDatabase _database;

        // 是否已初始化
        private bool _initialized = false;

        // 数据库工厂委托，用于创建数据库实例
        private Func<IFolderCommentDatabase> _databaseFactory;

        /// <summary>
        /// 获取单例实例
        /// </summary>
        public static FolderCommentManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FolderCommentManager();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        private FolderCommentManager()
        {
            // 默认使用工厂创建JSON数据库
            _databaseFactory = () => FolderCommentDatabaseFactory.CreateDatabase(FolderCommentDatabaseFactory.DatabaseType.Json);
        }

        /// <summary>
        /// 设置数据库工厂，用于依赖注入和测试
        /// </summary>
        /// <param name="databaseFactory">创建数据库实例的工厂方法</param>
        public void SetDatabaseFactory(Func<IFolderCommentDatabase> databaseFactory)
        {
            if (databaseFactory == null)
                throw new ArgumentNullException(nameof(databaseFactory));

            _databaseFactory = databaseFactory;

            // 如果已初始化，重新加载数据库
            if (_initialized)
            {
                LoadDatabase();
            }
        }

        /// <summary>
        /// 静态构造函数，Unity编辑器启动时自动调用
        /// </summary>
        static FolderCommentManager()
        {
            // 确保实例被创建
            EditorApplication.delayCall += () =>
            {
                Instance.Initialize();
            };
        }

        /// <summary>
        /// 初始化管理器
        /// </summary>
        public void Initialize()
        {
            if (_initialized)
                return;

            // 确保设置已加载
            var settings = FolderCommentSettings.Instance;

            LoadDatabase();
            _initialized = true;
        }

        /// <summary>
        /// 加载数据库
        /// </summary>
        private void LoadDatabase()
        {
            try
            {
                // 创建数据库实例
                _database = _databaseFactory();

                // 尝试加载数据
                if (!_database.Load())
                {
                    // 如果加载失败，检查是否需要从旧格式迁移数据
                    if (AssetDatabase.LoadAssetAtPath<FolderCommentsDatabase>(OldDatabasePath) != null)
                    {
                        // 如果存在旧格式的数据库，进行数据迁移
                        MigrateFromOldDatabase();
                    }
                    else
                    {
                        // 如果没有任何数据库，创建新的
                        Debug.Log("创建新的文件夹注释数据库");
                        _database.Save();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"加载数据库时出错: {e.Message}\n{e.StackTrace}");

                // 尝试恢复 - 创建新的数据库
                try
                {
                    _database = _databaseFactory();
                    _database.Save();
                    Debug.Log("已创建新的文件夹注释数据库");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"创建新数据库时出错: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }

        /// <summary>
        /// 从旧格式的数据库迁移数据
        /// </summary>
        private void MigrateFromOldDatabase()
        {
            try
            {
                Debug.Log("正在从旧格式迁移文件夹注释数据...");

                // 加载旧数据库
                var oldDatabase = AssetDatabase.LoadAssetAtPath<FolderCommentsDatabase>(OldDatabasePath);
                if (oldDatabase != null)
                {
                    // 复制所有注释数据
                    foreach (var comment in oldDatabase.Comments)
                    {
                        _database.AddComment(comment);
                    }

                    // 保存新数据库
                    _database.Save();

                    Debug.Log("文件夹注释数据迁移完成");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"迁移数据库时出错: {e.Message}\n{e.StackTrace}");
                _database.Save();
            }
        }

        /// <summary>
        /// 保存数据库
        /// </summary>
        public void SaveDatabase()
        {
            if (_database == null)
                return;

            try
            {
                _database.Save();
            }
            catch (Exception e)
            {
                Debug.LogError($"保存数据库时出错: {e.Message}\n{e.StackTrace}");
            }
        }

        /// <summary>
        /// 获取文件夹注释数据
        /// </summary>
        /// <param name="guid">文件夹GUID</param>
        /// <returns>注释数据，如果不存在则返回null</returns>
        public FolderCommentData GetFolderComment(string guid)
        {
            if (_database == null || string.IsNullOrEmpty(guid))
                return null;

            return _database.GetComment(guid);
        }

        /// <summary>
        /// 根据资源路径获取注释数据
        /// </summary>
        /// <param name="assetPath">资源路径</param>
        /// <returns>注释数据，如果不存在则返回null</returns>
        public FolderCommentData GetFolderCommentByPath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath) || !AssetDatabase.IsValidFolder(assetPath))
                return null;

            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            return GetFolderComment(guid);
        }

        /// <summary>
        /// 添加或更新文件夹注释
        /// </summary>
        /// <param name="guid">文件夹GUID</param>
        /// <param name="title">注释标题</param>
        /// <param name="comment">详细注释</param>
        /// <param name="color">标题颜色</param>
        public void SetFolderComment(string guid, string title, string comment, Color? color = null)
        {
            if (_database == null || string.IsNullOrEmpty(guid))
            {
                Debug.LogWarning("无法设置文件夹注释：数据库未初始化或GUID为空");
                return;
            }

            try
            {
                FolderCommentData existingData = _database.GetComment(guid);

                if (existingData != null)
                {
                    // 更新现有注释
                    existingData.UpdateComment(title, comment, color);
                    _database.AddComment(existingData);
                }
                else
                {
                    // 创建新注释
                    FolderCommentData newData = new FolderCommentData(guid, title, comment, color);
                    _database.AddComment(newData);
                }

                // 保存数据库
                SaveDatabase();

                // 刷新Project窗口
                EditorApplication.RepaintProjectWindow();
            }
            catch (Exception e)
            {
                Debug.LogError($"设置文件夹注释时出错: {e.Message}\n{e.StackTrace}");
            }
        }

        /// <summary>
        /// 根据资源路径添加或更新注释
        /// </summary>
        /// <param name="assetPath">资源路径</param>
        /// <param name="title">注释标题</param>
        /// <param name="comment">详细注释</param>
        /// <param name="color">标题颜色</param>
        public void SetFolderCommentByPath(string assetPath, string title, string comment, Color? color = null)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogWarning("无法设置文件夹注释：资源路径为空");
                return;
            }

            if (!AssetDatabase.IsValidFolder(assetPath))
            {
                Debug.LogWarning($"无法设置文件夹注释：{assetPath} 不是有效的文件夹");
                return;
            }

            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            SetFolderComment(guid, title, comment, color);
        }

        /// <summary>
        /// 删除文件夹注释
        /// </summary>
        /// <param name="guid">文件夹GUID</param>
        /// <returns>是否成功删除</returns>
        public bool RemoveFolderComment(string guid)
        {
            if (_database == null || string.IsNullOrEmpty(guid))
                return false;

            try
            {
                bool result = _database.RemoveComment(guid);

                if (result)
                {
                    // 保存数据库
                    SaveDatabase();

                    // 刷新Project窗口
                    EditorApplication.RepaintProjectWindow();
                }

                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"删除文件夹注释时出错: {e.Message}\n{e.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// 根据资源路径删除注释
        /// </summary>
        /// <param name="assetPath">资源路径</param>
        /// <returns>是否成功删除</returns>
        public bool RemoveFolderCommentByPath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogWarning("无法删除文件夹注释：资源路径为空");
                return false;
            }

            if (!AssetDatabase.IsValidFolder(assetPath))
            {
                Debug.LogWarning($"无法删除文件夹注释：{assetPath} 不是有效的文件夹");
                return false;
            }

            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            return RemoveFolderComment(guid);
        }

        /// <summary>
        /// 清空所有注释数据
        /// </summary>
        public void ClearAllComments()
        {
            if (_database == null)
                return;

            try
            {
                _database.ClearAllComments();
                SaveDatabase();
                EditorApplication.RepaintProjectWindow();
                Debug.Log("已清空所有文件夹注释");
            }
            catch (Exception e)
            {
                Debug.LogError($"清空所有注释时出错: {e.Message}\n{e.StackTrace}");
            }
        }
    }
}
