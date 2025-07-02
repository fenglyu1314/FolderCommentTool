using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace TATools.FolderCommentTool
{
    /// <summary>
    /// 文件夹注释工具的工具函数
    /// </summary>
    public static class FolderCommentUtils
    {
        // 用于获取文本宽度的反射方法
        private static MethodInfo _getNumCharactersThatFitWithinWidth;

        // 文本宽度缓存，避免重复计算
        private static readonly Dictionary<TextWidthCacheKey, int> _textWidthCache = new Dictionary<TextWidthCacheKey, int>(new TextWidthCacheKeyComparer());

        // 缓存大小限制，防止内存泄漏
        private const int MaxCacheSize = 500;

        // 缓存命中计数，用于性能监控
        private static int _cacheHits = 0;
        private static int _cacheMisses = 0;

        /// <summary>
        /// 静态构造函数，初始化反射方法
        /// </summary>
        static FolderCommentUtils()
        {
            // 初始化反射方法
            _getNumCharactersThatFitWithinWidth = typeof(GUIStyle).GetMethod(
                "GetNumCharactersThatFitWithinWidth",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (_getNumCharactersThatFitWithinWidth == null)
            {
                Debug.LogWarning("无法获取GUIStyle.GetNumCharactersThatFitWithinWidth方法，文本裁剪功能可能受限");
            }

            // 监听编辑器设置变化，清除缓存
            FolderCommentSettings.OnSettingsChanged += ClearTextWidthCache;
        }

        /// <summary>
        /// 清除文本宽度缓存
        /// </summary>
        public static void ClearTextWidthCache()
        {
            _textWidthCache.Clear();
            _cacheHits = 0;
            _cacheMisses = 0;
        }

        /// <summary>
        /// 判断是否为列表视图
        /// </summary>
        /// <param name="rect">项目矩形区域</param>
        /// <returns>是否为列表视图</returns>
        public static bool IsListView(Rect rect)
        {
            return rect.height <= 21f;
        }

        /// <summary>
        /// 判断图标是否为小图标
        /// </summary>
        /// <param name="rect">项目矩形区域</param>
        /// <returns>是否为小图标</returns>
        public static bool IsSmallIcon(ref Rect rect)
        {
            bool isSmall = rect.width > rect.height;

            if (isSmall)
                rect.width = rect.height;
            else
                rect.height = rect.width;

            return isSmall;
        }

        /// <summary>
        /// 裁剪文本以适应指定宽度
        /// </summary>
        /// <param name="style">GUI样式</param>
        /// <param name="text">原始文本</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="symbol">省略符号</param>
        /// <returns>裁剪后的文本</returns>
        public static string CropText(GUIStyle style, string text, float maxWidth, string symbol = "…")
        {
            if (string.IsNullOrEmpty(text))
                return text;

            // 如果宽度足够大，直接返回原文本
            if (style.CalcSize(new GUIContent(text)).x <= maxWidth)
                return text;

            // 创建缓存键
            var cacheKey = new TextWidthCacheKey(text, style.fontSize, maxWidth);

            // 尝试从缓存获取字符数
            int charCount;
            if (_textWidthCache.TryGetValue(cacheKey, out charCount))
            {
                _cacheHits++;
            }
            else
            {
                _cacheMisses++;

                // 缓存未命中，计算字符数
                if (_getNumCharactersThatFitWithinWidth != null)
                {
                    try
                    {
                        charCount = (int)_getNumCharactersThatFitWithinWidth.Invoke(style, new object[] { text, maxWidth });

                        // 添加到缓存
                        if (_textWidthCache.Count >= MaxCacheSize)
                        {
                            // 缓存已满，清除一半
                            ClearHalfCache();
                        }

                        _textWidthCache[cacheKey] = charCount;
                    }
                    catch (Exception)
                    {
                        // 反射失败，使用二分法估算
                        charCount = EstimateCharCount(style, text, maxWidth);
                    }
                }
                else
                {
                    // 反射方法不可用，使用二分法估算
                    charCount = EstimateCharCount(style, text, maxWidth);
                }
            }

            // 根据字符数裁剪文本
            if (charCount == -1 || charCount >= text.Length)
                return text;

            if (charCount <= 1)
                return string.Empty;

            return text.Substring(0, charCount - 1) + symbol;
        }

        /// <summary>
        /// 使用二分法估算适合宽度的字符数
        /// </summary>
        /// <param name="style">GUI样式</param>
        /// <param name="text">原始文本</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <returns>估算的字符数</returns>
        private static int EstimateCharCount(GUIStyle style, string text, float maxWidth)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            // 如果整个文本宽度小于最大宽度，直接返回文本长度
            if (style.CalcSize(new GUIContent(text)).x <= maxWidth)
                return text.Length;

            // 二分法查找合适的字符数
            int low = 0;
            int high = text.Length;

            while (low < high)
            {
                int mid = (low + high + 1) / 2;
                string subText = text.Substring(0, mid);

                if (style.CalcSize(new GUIContent(subText)).x <= maxWidth)
                {
                    low = mid;
                }
                else
                {
                    high = mid - 1;
                }
            }

            return low;
        }

        /// <summary>
        /// 清除一半的缓存
        /// </summary>
        private static void ClearHalfCache()
        {
            // 简单策略：直接清除所有缓存
            // 更复杂的策略可以实现LRU（最近最少使用）算法
            _textWidthCache.Clear();
        }

        /// <summary>
        /// 获取缓存统计信息
        /// </summary>
        /// <returns>缓存统计信息</returns>
        public static string GetCacheStats()
        {
            int total = _cacheHits + _cacheMisses;
            float hitRate = total > 0 ? (float)_cacheHits / total * 100 : 0;

            return $"缓存大小: {_textWidthCache.Count}/{MaxCacheSize}, 命中率: {hitRate:F1}% ({_cacheHits}/{total})";
        }

        /// <summary>
        /// 格式化日期时间
        /// </summary>
        /// <param name="dateTime">日期时间</param>
        /// <returns>格式化后的字符串</returns>
        public static string FormatDateTime(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 检查路径是否为有效的文件夹
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <returns>是否为有效文件夹</returns>
        public static bool IsValidFolder(string path)
        {
            return !string.IsNullOrEmpty(path) && AssetDatabase.IsValidFolder(path);
        }

        /// <summary>
        /// 获取文件夹的GUID
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <returns>GUID，如果无效则返回空字符串</returns>
        public static string GetFolderGuid(string path)
        {
            if (!IsValidFolder(path))
                return string.Empty;

            return AssetDatabase.AssetPathToGUID(path);
        }
    }

    /// <summary>
    /// 文本宽度缓存键
    /// </summary>
    internal struct TextWidthCacheKey
    {
        public readonly string Text;
        public readonly int FontSize;
        public readonly float MaxWidth;

        public TextWidthCacheKey(string text, int fontSize, float maxWidth)
        {
            Text = text;
            FontSize = fontSize;
            MaxWidth = maxWidth;
        }
    }

    /// <summary>
    /// 文本宽度缓存键比较器
    /// </summary>
    internal class TextWidthCacheKeyComparer : IEqualityComparer<TextWidthCacheKey>
    {
        public bool Equals(TextWidthCacheKey x, TextWidthCacheKey y)
        {
            return x.Text == y.Text &&
                   x.FontSize == y.FontSize &&
                   Math.Abs(x.MaxWidth - y.MaxWidth) < 0.01f;
        }

        public int GetHashCode(TextWidthCacheKey obj)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (obj.Text?.GetHashCode() ?? 0);
                hash = hash * 23 + obj.FontSize.GetHashCode();
                hash = hash * 23 + ((int)(obj.MaxWidth * 100)).GetHashCode();
                return hash;
            }
        }
    }
}
