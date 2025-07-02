using System;
using UnityEngine;
using UnityEditor;

namespace TATools.FolderCommentTool
{
    /// <summary>
    /// 文件夹注释数据库工厂类，用于创建不同类型的数据库实例
    /// </summary>
    public static class FolderCommentDatabaseFactory
    {
        /// <summary>
        /// 数据库类型枚举
        /// </summary>
        public enum DatabaseType
        {
            /// <summary>
            /// JSON文件数据库（默认）
            /// </summary>
            Json,

            /// <summary>
            /// ScriptableObject数据库（旧版）
            /// </summary>
            ScriptableObject
        }

        /// <summary>
        /// 创建数据库实例
        /// </summary>
        /// <param name="type">数据库类型</param>
        /// <returns>数据库实例</returns>
        public static IFolderCommentDatabase CreateDatabase(DatabaseType type = DatabaseType.Json)
        {
            switch (type)
            {
                case DatabaseType.Json:
                    return new FolderCommentsJsonDatabase();

                case DatabaseType.ScriptableObject:
                    // 旧版ScriptableObject数据库路径
                    string databasePath = System.IO.Path.Combine("Assets", "FolderCommentTool", "Editor", "Data", "FolderComments.asset");

                    // 尝试加载现有数据库
                    var database = AssetDatabase.LoadAssetAtPath<FolderCommentsDatabase>(databasePath);

                    // 如果不存在，创建新的
                    if (database == null)
                    {
                        try
                        {
                            // 确保目录存在
                            string directory = System.IO.Path.GetDirectoryName(databasePath);
                            if (!System.IO.Directory.Exists(directory))
                            {
                                System.IO.Directory.CreateDirectory(directory);
                            }

                            // 创建新的数据库
                            database = ScriptableObject.CreateInstance<FolderCommentsDatabase>();
                            AssetDatabase.CreateAsset(database, databasePath);
                            AssetDatabase.SaveAssets();
                            Debug.Log($"已创建新的ScriptableObject数据库: {databasePath}");
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"创建ScriptableObject数据库时出错: {e.Message}\n{e.StackTrace}");
                            return new FolderCommentsJsonDatabase(); // 失败时回退到JSON数据库
                        }
                    }

                    return database;

                default:
                    Debug.LogWarning($"未知的数据库类型: {type}，使用默认的JSON数据库");
                    return new FolderCommentsJsonDatabase();
            }
        }
    }
}
