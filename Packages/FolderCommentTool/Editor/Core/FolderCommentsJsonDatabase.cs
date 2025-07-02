using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace TATools.FolderCommentTool
{
    /// <summary>
    /// 文件夹注释JSON数据容器
    /// </summary>
    [Serializable]
    public class FolderCommentsJsonDatabase : IFolderCommentDatabase
    {
        // 数据库文件名
        private const string DatabaseFileName = "FolderComments.json";

        // 数据库保存路径
        private static readonly string DatabasePath = Path.Combine("ProjectSettings", DatabaseFileName);

        /// <summary>
        /// 所有文件夹注释数据列表
        /// </summary>
        public List<FolderCommentData> comments = new List<FolderCommentData>();

        /// <summary>
        /// 获取所有注释数据
        /// </summary>
        public List<FolderCommentData> Comments => comments;

        /// <summary>
        /// 添加新的注释数据
        /// </summary>
        /// <param name="commentData">注释数据</param>
        public void AddComment(FolderCommentData commentData)
        {
            if (commentData == null || string.IsNullOrEmpty(commentData.guid))
            {
                Debug.LogWarning("尝试添加无效的注释数据");
                return;
            }

            // 检查是否已存在相同GUID的注释
            int existingIndex = comments.FindIndex(c => c.guid == commentData.guid);

            if (existingIndex >= 0)
            {
                // 更新已存在的注释
                comments[existingIndex] = commentData;
            }
            else
            {
                // 添加新注释
                comments.Add(commentData);
            }
        }

        /// <summary>
        /// 根据GUID获取注释数据
        /// </summary>
        /// <param name="guid">文件夹GUID</param>
        /// <returns>注释数据，如果不存在则返回null</returns>
        public FolderCommentData GetComment(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return null;

            return comments.Find(c => c.guid == guid);
        }

        /// <summary>
        /// 根据GUID删除注释数据
        /// </summary>
        /// <param name="guid">文件夹GUID</param>
        /// <returns>是否成功删除</returns>
        public bool RemoveComment(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return false;

            int index = comments.FindIndex(c => c.guid == guid);

            if (index >= 0)
            {
                comments.RemoveAt(index);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 清空所有注释数据
        /// </summary>
        public void ClearAllComments()
        {
            comments.Clear();
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <returns>是否加载成功</returns>
        public bool Load()
        {
            try
            {
                if (!File.Exists(DatabasePath))
                {
                    Debug.Log($"数据库文件不存在: {DatabasePath}，将创建新的数据库");
                    return false;
                }

                string json = File.ReadAllText(DatabasePath);
                var loadedData = JsonUtility.FromJson<FolderCommentsJsonDatabase>(json);

                if (loadedData == null || loadedData.comments == null)
                {
                    Debug.LogError("数据库加载失败: 解析JSON数据出错");
                    return false;
                }

                // 复制加载的数据
                comments = loadedData.comments;
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"加载数据库时出错: {e.Message}\n{e.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <returns>是否保存成功</returns>
        public bool Save()
        {
            try
            {
                string json = JsonUtility.ToJson(this, true);
                File.WriteAllText(DatabasePath, json);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"保存数据库时出错: {e.Message}\n{e.StackTrace}");
                return false;
            }
        }
    }
}
