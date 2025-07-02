using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace TATools.FolderCommentTool
{
    /// <summary>
    /// 文件夹注释测试辅助工具
    /// </summary>
    public static class FolderCommentTestHelper
    {
        // 测试文件夹路径
        private static readonly string TestFoldersRoot = "Assets/TestFolders";

        // 性能测试文件夹路径
        private static readonly string PerformanceTestFoldersRoot = "Assets/PerformanceTestFolders";

        // 性能测试文件夹数量
        private const int PerformanceFolderCount = 100;

        // 测试文件夹注释模板
        private static readonly Dictionary<string, TestFolderTemplate> TestFolderTemplates = new Dictionary<string, TestFolderTemplate>
        {
            { "Scripts", new TestFolderTemplate("脚本文件", "存放项目中的所有脚本文件，包括游戏逻辑、UI控制等", new Color(0.4f, 0.8f, 1f)) },
            { "Prefabs", new TestFolderTemplate("预制体", "存放项目中的所有预制体资源", new Color(0.4f, 1f, 0.4f)) },
            { "3DModels", new TestFolderTemplate("模型资源", "存放项目中的所有3D模型资源", new Color(1f, 0.4f, 0.4f)) },
            { "Art/Textures", new TestFolderTemplate("纹理资源", "存放项目中的所有纹理和图片资源", new Color(1f, 1f, 0.4f)) },
            { "EmptyFolder", new TestFolderTemplate("空文件夹", "这是一个空文件夹，用于测试", new Color(0.7f, 0.7f, 0.7f)) },
            { "Art", new TestFolderTemplate("美术资源", "存放项目中的所有美术相关资源", new Color(1f, 0.6f, 0.8f)) },
            { "Nested/Level1/Level2", new TestFolderTemplate("嵌套文件夹", "这是一个多层嵌套的文件夹，用于测试多层级目录", new Color(0.5f, 0.5f, 1f)) },
            { "RichText", new TestFolderTemplate("<b>富文本</b>标题", "<color=#FF0000>这是红色文本</color>\n<b>这是粗体文本</b>\n<i>这是斜体文本</i>\n<size=14>这是大号文本</size>", new Color(1f, 0.5f, 0.5f)) },
            { "LongComment", new TestFolderTemplate("长注释", "这是一个非常长的注释，用于测试注释的换行和显示效果。这是一个非常长的注释，用于测试注释的换行和显示效果。这是一个非常长的注释，用于测试注释的换行和显示效果。这是一个非常长的注释，用于测试注释的换行和显示效果。这是一个非常长的注释，用于测试注释的换行和显示效果。", new Color(0.5f, 0.5f, 0.5f)) }
        };

        /// <summary>
        /// 创建测试文件夹
        /// </summary>
        [MenuItem("TATools/文件夹注释工具/创建测试文件夹")]
        public static void CreateTestFolders()
        {
            int createdCount = 0;

            // 确保根目录存在
            if (!AssetDatabase.IsValidFolder(TestFoldersRoot))
            {
                string parentFolder = Path.GetDirectoryName(TestFoldersRoot);
                string folderName = Path.GetFileName(TestFoldersRoot);
                AssetDatabase.CreateFolder(parentFolder, folderName);
                createdCount++;
                Debug.Log($"已创建测试根目录: {TestFoldersRoot}");
            }

            // 创建测试文件夹
            foreach (var template in TestFolderTemplates)
            {
                string relativePath = template.Key;
                string fullPath = Path.Combine(TestFoldersRoot, relativePath);

                // 如果文件夹已存在，跳过
                if (AssetDatabase.IsValidFolder(fullPath))
                {
                    Debug.Log($"测试文件夹已存在: {fullPath}");
                    continue;
                }

                // 处理嵌套文件夹
                string[] pathParts = relativePath.Split('/');
                string currentPath = TestFoldersRoot;

                for (int i = 0; i < pathParts.Length; i++)
                {
                    string nextPath = Path.Combine(currentPath, pathParts[i]);

                    if (!AssetDatabase.IsValidFolder(nextPath))
                    {
                        AssetDatabase.CreateFolder(currentPath, pathParts[i]);
                        createdCount++;
                        Debug.Log($"已创建文件夹: {nextPath}");
                    }

                    currentPath = nextPath;
                }
            }

            // 刷新资源数据库
            AssetDatabase.Refresh();

            // 显示结果
            EditorUtility.DisplayDialog(
                "创建测试文件夹",
                $"已创建 {createdCount} 个测试文件夹。",
                "确定"
            );
        }

        /// <summary>
        /// 为测试文件夹添加注释模板
        /// </summary>
        [MenuItem("TATools/文件夹注释工具/添加测试注释模板")]
        public static void ApplyTestFolderTemplates()
        {
            int successCount = 0;
            int failCount = 0;

            foreach (var template in TestFolderTemplates)
            {
                string folderPath = Path.Combine(TestFoldersRoot, template.Key);

                // 检查文件夹是否存在
                if (!AssetDatabase.IsValidFolder(folderPath))
                {
                    Debug.LogWarning($"测试文件夹不存在: {folderPath}");
                    failCount++;
                    continue;
                }

                // 获取文件夹GUID
                string guid = AssetDatabase.AssetPathToGUID(folderPath);

                // 添加注释
                FolderCommentManager.Instance.SetFolderComment(
                    guid,
                    template.Value.Title,
                    template.Value.Comment,
                    template.Value.Color
                );

                successCount++;
                Debug.Log($"已为文件夹 {folderPath} 添加注释: {template.Value.Title}");
            }

            // 刷新Project窗口
            EditorApplication.RepaintProjectWindow();

            // 显示结果
            EditorUtility.DisplayDialog(
                "添加测试注释模板",
                $"成功添加 {successCount} 个注释，失败 {failCount} 个。",
                "确定"
            );
        }

        /// <summary>
        /// 清除所有测试文件夹的注释
        /// </summary>
        [MenuItem("TATools/文件夹注释工具/清除测试注释")]
        public static void ClearTestFolderComments()
        {
            int removedCount = 0;

            foreach (var template in TestFolderTemplates)
            {
                string folderPath = Path.Combine(TestFoldersRoot, template.Key);

                // 检查文件夹是否存在
                if (!AssetDatabase.IsValidFolder(folderPath))
                {
                    continue;
                }

                // 获取文件夹GUID
                string guid = AssetDatabase.AssetPathToGUID(folderPath);

                // 删除注释
                if (FolderCommentManager.Instance.RemoveFolderComment(guid))
                {
                    removedCount++;
                    Debug.Log($"已清除文件夹 {folderPath} 的注释");
                }
            }

            // 刷新Project窗口
            EditorApplication.RepaintProjectWindow();

            // 显示结果
            EditorUtility.DisplayDialog(
                "清除测试注释",
                $"已清除 {removedCount} 个测试文件夹的注释。",
                "确定"
            );
        }

        /// <summary>
        /// 删除测试文件夹
        /// </summary>
        [MenuItem("TATools/文件夹注释工具/删除测试文件夹")]
        public static void DeleteTestFolders()
        {
            // 先清除注释
            ClearTestFolderComments();

            // 检查根目录是否存在
            if (!AssetDatabase.IsValidFolder(TestFoldersRoot))
            {
                EditorUtility.DisplayDialog(
                    "删除测试文件夹",
                    $"测试根目录不存在: {TestFoldersRoot}",
                    "确定"
                );
                return;
            }

            // 删除根目录
            bool success = AssetDatabase.DeleteAsset(TestFoldersRoot);

            // 刷新资源数据库
            AssetDatabase.Refresh();

            // 显示结果
            if (success)
            {
                EditorUtility.DisplayDialog(
                    "删除测试文件夹",
                    $"已成功删除测试文件夹: {TestFoldersRoot}",
                    "确定"
                );
            }
            else
            {
                EditorUtility.DisplayDialog(
                    "删除测试文件夹",
                    $"删除测试文件夹失败: {TestFoldersRoot}",
                    "确定"
                );
            }
        }

        /// <summary>
        /// 创建性能测试文件夹
        /// </summary>
        [MenuItem("TATools/文件夹注释工具/性能测试/创建性能测试文件夹")]
        public static void CreatePerformanceTestFolders()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int createdCount = 0;

            // 确保根目录存在
            if (!AssetDatabase.IsValidFolder(PerformanceTestFoldersRoot))
            {
                string parentFolder = Path.GetDirectoryName(PerformanceTestFoldersRoot);
                string folderName = Path.GetFileName(PerformanceTestFoldersRoot);
                AssetDatabase.CreateFolder(parentFolder, folderName);
                createdCount++;
                Debug.Log($"已创建性能测试根目录: {PerformanceTestFoldersRoot}");
            }

            // 创建多个测试文件夹
            for (int i = 0; i < PerformanceFolderCount; i++)
            {
                string folderName = $"TestFolder_{i:D3}";
                string folderPath = Path.Combine(PerformanceTestFoldersRoot, folderName);

                if (!AssetDatabase.IsValidFolder(folderPath))
                {
                    AssetDatabase.CreateFolder(PerformanceTestFoldersRoot, folderName);
                    createdCount++;

                    // 每创建10个文件夹输出一次日志
                    if (i % 10 == 0)
                    {
                        Debug.Log($"已创建 {i} 个性能测试文件夹...");
                    }
                }
            }

            // 刷新资源数据库
            AssetDatabase.Refresh();

            stopwatch.Stop();

            // 显示结果
            EditorUtility.DisplayDialog(
                "创建性能测试文件夹",
                $"已创建 {createdCount} 个性能测试文件夹，耗时 {stopwatch.ElapsedMilliseconds} 毫秒。",
                "确定"
            );
        }

        /// <summary>
        /// 为性能测试文件夹添加注释
        /// </summary>
        [MenuItem("TATools/文件夹注释工具/性能测试/添加性能测试注释")]
        public static void ApplyPerformanceTestComments()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int successCount = 0;

            // 检查根目录是否存在
            if (!AssetDatabase.IsValidFolder(PerformanceTestFoldersRoot))
            {
                EditorUtility.DisplayDialog(
                    "添加性能测试注释",
                    $"性能测试根目录不存在: {PerformanceTestFoldersRoot}，请先创建性能测试文件夹。",
                    "确定"
                );
                return;
            }

            // 获取所有子文件夹
            string[] folderPaths = AssetDatabase.GetSubFolders(PerformanceTestFoldersRoot);

            // 为每个文件夹添加注释
            for (int i = 0; i < folderPaths.Length; i++)
            {
                string folderPath = folderPaths[i];
                string folderName = Path.GetFileName(folderPath);

                // 生成随机颜色
                Color color = new Color(
                    UnityEngine.Random.value,
                    UnityEngine.Random.value,
                    UnityEngine.Random.value
                );

                // 添加注释
                string guid = AssetDatabase.AssetPathToGUID(folderPath);
                FolderCommentManager.Instance.SetFolderComment(
                    guid,
                    $"性能测试 {folderName}",
                    $"这是性能测试文件夹 {folderName} 的注释内容。\n包含一些<b>富文本</b>和<color=#FF0000>颜色</color>测试。",
                    color
                );

                successCount++;

                // 每处理10个文件夹输出一次日志
                if (i % 10 == 0)
                {
                    Debug.Log($"已处理 {i} 个性能测试文件夹...");
                }
            }

            // 保存数据库
            FolderCommentManager.Instance.SaveDatabase();

            // 刷新Project窗口
            EditorApplication.RepaintProjectWindow();

            stopwatch.Stop();

            // 显示结果
            EditorUtility.DisplayDialog(
                "添加性能测试注释",
                $"已为 {successCount} 个性能测试文件夹添加注释，耗时 {stopwatch.ElapsedMilliseconds} 毫秒。",
                "确定"
            );
        }

        /// <summary>
        /// 清除性能测试注释
        /// </summary>
        [MenuItem("TATools/文件夹注释工具/性能测试/清除性能测试注释")]
        public static void ClearPerformanceTestComments()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int removedCount = 0;

            // 检查根目录是否存在
            if (!AssetDatabase.IsValidFolder(PerformanceTestFoldersRoot))
            {
                EditorUtility.DisplayDialog(
                    "清除性能测试注释",
                    $"性能测试根目录不存在: {PerformanceTestFoldersRoot}",
                    "确定"
                );
                return;
            }

            // 获取所有子文件夹
            string[] folderPaths = AssetDatabase.GetSubFolders(PerformanceTestFoldersRoot);

            // 清除每个文件夹的注释
            for (int i = 0; i < folderPaths.Length; i++)
            {
                string folderPath = folderPaths[i];
                string guid = AssetDatabase.AssetPathToGUID(folderPath);

                if (FolderCommentManager.Instance.RemoveFolderComment(guid))
                {
                    removedCount++;
                }

                // 每处理10个文件夹输出一次日志
                if (i % 10 == 0)
                {
                    Debug.Log($"已处理 {i} 个性能测试文件夹...");
                }
            }

            // 保存数据库
            FolderCommentManager.Instance.SaveDatabase();

            // 刷新Project窗口
            EditorApplication.RepaintProjectWindow();

            stopwatch.Stop();

            // 显示结果
            EditorUtility.DisplayDialog(
                "清除性能测试注释",
                $"已清除 {removedCount} 个性能测试文件夹的注释，耗时 {stopwatch.ElapsedMilliseconds} 毫秒。",
                "确定"
            );
        }

        /// <summary>
        /// 删除性能测试文件夹
        /// </summary>
        [MenuItem("TATools/文件夹注释工具/性能测试/删除性能测试文件夹")]
        public static void DeletePerformanceTestFolders()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // 先清除注释
            ClearPerformanceTestComments();

            // 检查根目录是否存在
            if (!AssetDatabase.IsValidFolder(PerformanceTestFoldersRoot))
            {
                EditorUtility.DisplayDialog(
                    "删除性能测试文件夹",
                    $"性能测试根目录不存在: {PerformanceTestFoldersRoot}",
                    "确定"
                );
                return;
            }

            // 删除根目录
            bool success = AssetDatabase.DeleteAsset(PerformanceTestFoldersRoot);

            // 刷新资源数据库
            AssetDatabase.Refresh();

            stopwatch.Stop();

            // 显示结果
            if (success)
            {
                EditorUtility.DisplayDialog(
                    "删除性能测试文件夹",
                    $"已成功删除性能测试文件夹: {PerformanceTestFoldersRoot}，耗时 {stopwatch.ElapsedMilliseconds} 毫秒。",
                    "确定"
                );
            }
            else
            {
                EditorUtility.DisplayDialog(
                    "删除性能测试文件夹",
                    $"删除性能测试文件夹失败: {PerformanceTestFoldersRoot}",
                    "确定"
                );
            }
        }

        /// <summary>
        /// 创建空注释测试文件夹
        /// </summary>
        [MenuItem("TATools/文件夹注释工具/创建空注释测试文件夹")]
        public static void CreateEmptyCommentTestFolder()
        {
            string testFolderPath = "Assets/EmptyCommentTest";

            // 创建测试文件夹
            if (!AssetDatabase.IsValidFolder(testFolderPath))
            {
                AssetDatabase.CreateFolder("Assets", "EmptyCommentTest");
                AssetDatabase.Refresh();
                Debug.Log($"已创建空注释测试文件夹: {testFolderPath}");
            }
            else
            {
                Debug.Log($"空注释测试文件夹已存在: {testFolderPath}");
            }

            // 确保该文件夹没有注释
            string guid = AssetDatabase.AssetPathToGUID(testFolderPath);
            FolderCommentManager.Instance.RemoveFolderComment(guid);

            // 刷新Project窗口
            EditorApplication.RepaintProjectWindow();

            EditorUtility.DisplayDialog(
                "创建空注释测试文件夹",
                $"已创建空注释测试文件夹: {testFolderPath}\n请在Inspector中选择该文件夹，验证空注释时不显示任何内容。",
                "确定"
            );
        }

        /// <summary>
        /// 删除空注释测试文件夹
        /// </summary>
        [MenuItem("TATools/文件夹注释工具/删除空注释测试文件夹")]
        public static void DeleteEmptyCommentTestFolder()
        {
            string testFolderPath = "Assets/EmptyCommentTest";

            if (AssetDatabase.IsValidFolder(testFolderPath))
            {
                // 先清除注释
                string guid = AssetDatabase.AssetPathToGUID(testFolderPath);
                FolderCommentManager.Instance.RemoveFolderComment(guid);

                // 删除文件夹
                bool success = AssetDatabase.DeleteAsset(testFolderPath);
                AssetDatabase.Refresh();

                if (success)
                {
                    Debug.Log($"已删除空注释测试文件夹: {testFolderPath}");
                    EditorUtility.DisplayDialog(
                        "删除空注释测试文件夹",
                        $"已成功删除空注释测试文件夹: {testFolderPath}",
                        "确定"
                    );
                }
                else
                {
                    Debug.LogError($"删除空注释测试文件夹失败: {testFolderPath}");
                }
            }
            else
            {
                EditorUtility.DisplayDialog(
                    "删除空注释测试文件夹",
                    $"空注释测试文件夹不存在: {testFolderPath}",
                    "确定"
                );
            }
        }

        /// <summary>
        /// 运行性能测试
        /// </summary>
        [MenuItem("TATools/文件夹注释工具/性能测试/运行性能测试")]
        public static void RunPerformanceTest()
        {
            StringBuilder report = new StringBuilder();
            report.AppendLine("文件夹注释工具性能测试报告");
            report.AppendLine("==========================");
            report.AppendLine($"测试时间: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
            report.AppendLine($"测试文件夹数量: {PerformanceFolderCount}");
            report.AppendLine();

            // 创建测试文件夹
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            CreatePerformanceTestFolders();
            stopwatch.Stop();
            report.AppendLine($"创建文件夹耗时: {stopwatch.ElapsedMilliseconds} 毫秒");

            // 添加注释
            stopwatch.Reset();
            stopwatch.Start();
            ApplyPerformanceTestComments();
            stopwatch.Stop();
            report.AppendLine($"添加注释耗时: {stopwatch.ElapsedMilliseconds} 毫秒");

            // 清除注释
            stopwatch.Reset();
            stopwatch.Start();
            ClearPerformanceTestComments();
            stopwatch.Stop();
            report.AppendLine($"清除注释耗时: {stopwatch.ElapsedMilliseconds} 毫秒");

            // 删除文件夹
            stopwatch.Reset();
            stopwatch.Start();
            DeletePerformanceTestFolders();
            stopwatch.Stop();
            report.AppendLine($"删除文件夹耗时: {stopwatch.ElapsedMilliseconds} 毫秒");

            // 显示报告
            Debug.Log(report.ToString());

            // 显示结果
            EditorUtility.DisplayDialog(
                "性能测试完成",
                "性能测试已完成，详细报告已输出到控制台。",
                "确定"
            );
        }
    }

    /// <summary>
    /// 测试文件夹注释模板
    /// </summary>
    public class TestFolderTemplate
    {
        public string Title { get; private set; }
        public string Comment { get; private set; }
        public Color Color { get; private set; }

        public TestFolderTemplate(string title, string comment, Color color)
        {
            Title = title;
            Comment = comment;
            Color = color;
        }
    }
}
