using System.Collections.Generic;

namespace TATools.FolderCommentTool
{
    /// <summary>
    /// 文件夹注释数据库接口
    /// </summary>
    public interface IFolderCommentDatabase
    {
        /// <summary>
        /// 获取所有注释数据
        /// </summary>
        List<FolderCommentData> Comments { get; }

        /// <summary>
        /// 添加或更新注释数据
        /// </summary>
        /// <param name="commentData">注释数据</param>
        void AddComment(FolderCommentData commentData);

        /// <summary>
        /// 根据GUID获取注释数据
        /// </summary>
        /// <param name="guid">文件夹GUID</param>
        /// <returns>注释数据，如果不存在则返回null</returns>
        FolderCommentData GetComment(string guid);

        /// <summary>
        /// 根据GUID删除注释数据
        /// </summary>
        /// <param name="guid">文件夹GUID</param>
        /// <returns>是否成功删除</returns>
        bool RemoveComment(string guid);

        /// <summary>
        /// 清空所有注释数据
        /// </summary>
        void ClearAllComments();

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <returns>是否加载成功</returns>
        bool Load();

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <returns>是否保存成功</returns>
        bool Save();
    }
}
