using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TATools.FolderCommentTool
{
    /// <summary>
    /// 文件夹注释数据容器
    /// </summary>
    [CreateAssetMenu(fileName = "FolderComments", menuName = "FolderCommentTool/Comments Database")]
    public class FolderCommentsDatabase : ScriptableObject, IFolderCommentDatabase
    {
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

            // 标记为脏，确保Unity保存更改
            EditorUtility.SetDirty(this);
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
                EditorUtility.SetDirty(this);
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
            EditorUtility.SetDirty(this);
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <returns>是否加载成功</returns>
        public bool Load()
        {
            // ScriptableObject由Unity自动加载，所以这里总是返回true
            return true;
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <returns>是否保存成功</returns>
        public bool Save()
        {
            try
            {
                // 确保资源被保存
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"保存ScriptableObject数据库时出错: {e.Message}\n{e.StackTrace}");
                return false;
            }
        }
    }
}
