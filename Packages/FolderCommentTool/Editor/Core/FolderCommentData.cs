using System;
using UnityEngine;
using UnityEditor;

namespace TATools.FolderCommentTool
{
    /// <summary>
    /// 文件夹注释数据结构
    /// </summary>
    [Serializable]
    public class FolderCommentData
    {
        /// <summary>
        /// 文件夹的GUID
        /// </summary>
        public string guid;

        /// <summary>
        /// 注释标题（显示在Project窗口）
        /// </summary>
        public string title;

        /// <summary>
        /// 详细注释内容
        /// </summary>
        public string comment;

        /// <summary>
        /// 标题颜色
        /// </summary>
        public Color titleColor = new Color(0.4f, 0.8f, 1f); // 默认浅蓝色

        /// <summary>
        /// 创建时间
        /// </summary>
        public long createdTimeStamp;

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public long modifiedTimeStamp;

        /// <summary>
        /// 创建时间（DateTime格式，不序列化）
        /// </summary>
        public DateTime CreatedTime
        {
            get => DateTimeOffset.FromUnixTimeMilliseconds(createdTimeStamp).DateTime;
            set => createdTimeStamp = new DateTimeOffset(value).ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// 最后修改时间（DateTime格式，不序列化）
        /// </summary>
        public DateTime ModifiedTime
        {
            get => DateTimeOffset.FromUnixTimeMilliseconds(modifiedTimeStamp).DateTime;
            set => modifiedTimeStamp = new DateTimeOffset(value).ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// 创建新的文件夹注释数据
        /// </summary>
        /// <param name="guid">文件夹GUID</param>
        /// <param name="title">注释标题</param>
        /// <param name="comment">详细注释</param>
        /// <param name="color">标题颜色</param>
        public FolderCommentData(string guid, string title, string comment, Color? color = null)
        {
            this.guid = guid;
            this.title = title;
            this.comment = comment;
            this.titleColor = color.HasValue ? color.Value : new Color(0.4f, 0.8f, 1f);

            DateTime now = DateTime.Now;
            this.createdTimeStamp = new DateTimeOffset(now).ToUnixTimeMilliseconds();
            this.modifiedTimeStamp = new DateTimeOffset(now).ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// 更新注释内容
        /// </summary>
        /// <param name="title">新标题</param>
        /// <param name="comment">新注释</param>
        /// <param name="color">新颜色</param>
        public void UpdateComment(string title, string comment, Color? color = null)
        {
            this.title = title;
            this.comment = comment;

            if (color.HasValue)
            {
                this.titleColor = color.Value;
            }

            ModifiedTime = DateTime.Now;
        }
    }
}
