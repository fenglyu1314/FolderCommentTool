using System.Collections;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;

namespace TATools.FolderCommentTool.Tests
{
    public class FolderCommentTest
    {
        // 测试文件夹路径
        private const string TestFolderPath = "Assets/FolderCommentTestTemp";

        [OneTimeSetUp]
        public void SetUp()
        {
            // 创建测试文件夹
            if (!AssetDatabase.IsValidFolder(TestFolderPath))
            {
                string parentFolder = Path.GetDirectoryName(TestFolderPath);
                string folderName = Path.GetFileName(TestFolderPath);
                AssetDatabase.CreateFolder(parentFolder, folderName);
                Debug.Log($"已创建测试文件夹: {TestFolderPath}");
            }

            AssetDatabase.Refresh();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            // 删除测试文件夹
            if (AssetDatabase.IsValidFolder(TestFolderPath))
            {
                AssetDatabase.DeleteAsset(TestFolderPath);
                Debug.Log($"已删除测试文件夹: {TestFolderPath}");
            }

            AssetDatabase.Refresh();
        }

        [Test]
        public void TestFolderCommentManager()
        {
            // 确保FolderCommentManager实例已创建
            var manager = FolderCommentManager.Instance;
            Assert.IsNotNull(manager, "FolderCommentManager实例不应为空");
        }

        [Test]
        public void TestFolderCommentSettings()
        {
            // 确保FolderCommentSettings实例已创建
            var settings = FolderCommentSettings.Instance;
            Assert.IsNotNull(settings, "FolderCommentSettings实例不应为空");

            // 测试设置的默认值
            Assert.IsTrue(settings.enableFolderComment, "默认应启用文件夹注释功能");
            Assert.AreEqual(11, settings.listViewFontSize, "列表视图字体大小默认应为11");
            Assert.AreEqual(11, settings.iconViewFontSize, "图标视图字体大小默认应为11");
            Assert.IsTrue(settings.useBoldFont, "默认应使用粗体");
        }

        [Test]
        public void TestFolderCommentStyles()
        {
            // 测试样式是否正确创建
            var listViewStyle = FolderCommentStyles.ListViewLabelStyle;
            var iconViewStyle = FolderCommentStyles.IconViewLabelStyle;

            Assert.IsNotNull(listViewStyle, "列表视图样式不应为空");
            Assert.IsNotNull(iconViewStyle, "图标视图样式不应为空");

            // 测试样式属性
            Assert.IsTrue(listViewStyle.richText, "列表视图样式应支持富文本");
            Assert.IsTrue(iconViewStyle.richText, "图标视图样式应支持富文本");
        }

        [Test]
        public void TestAddAndRemoveComment()
        {
            // 获取测试文件夹的GUID
            string guid = AssetDatabase.AssetPathToGUID(TestFolderPath);
            Assert.IsFalse(string.IsNullOrEmpty(guid), "测试文件夹GUID不应为空");

            // 测试添加注释
            string testTitle = "测试标题";
            string testComment = "测试注释内容";
            Color testColor = new Color(1f, 0f, 0f);

            FolderCommentManager.Instance.SetFolderComment(guid, testTitle, testComment, testColor);

            // 验证注释是否添加成功
            var commentData = FolderCommentManager.Instance.GetFolderComment(guid);
            Assert.IsNotNull(commentData, "注释数据不应为空");
            Assert.AreEqual(testTitle, commentData.title, "标题应匹配");
            Assert.AreEqual(testComment, commentData.comment, "注释内容应匹配");
            Assert.AreEqual(testColor, commentData.titleColor, "颜色应匹配");

            // 测试更新注释
            string updatedTitle = "更新后的标题";
            FolderCommentManager.Instance.SetFolderComment(guid, updatedTitle, testComment, testColor);

            // 验证注释是否更新成功
            commentData = FolderCommentManager.Instance.GetFolderComment(guid);
            Assert.AreEqual(updatedTitle, commentData.title, "更新后的标题应匹配");

            // 测试删除注释
            bool removed = FolderCommentManager.Instance.RemoveFolderComment(guid);
            Assert.IsTrue(removed, "注释应成功删除");

            // 验证注释是否删除成功
            commentData = FolderCommentManager.Instance.GetFolderComment(guid);
            Assert.IsNull(commentData, "删除后注释数据应为空");
        }

        [Test]
        public void TestPathBasedOperations()
        {
            // 测试基于路径的操作
            string testTitle = "路径测试";
            string testComment = "基于路径的操作测试";
            Color testColor = new Color(0f, 1f, 0f);

            // 添加注释
            FolderCommentManager.Instance.SetFolderCommentByPath(TestFolderPath, testTitle, testComment, testColor);

            // 验证注释是否添加成功
            var commentData = FolderCommentManager.Instance.GetFolderCommentByPath(TestFolderPath);
            Assert.IsNotNull(commentData, "基于路径的注释数据不应为空");
            Assert.AreEqual(testTitle, commentData.title, "基于路径的标题应匹配");

            // 删除注释
            bool removed = FolderCommentManager.Instance.RemoveFolderCommentByPath(TestFolderPath);
            Assert.IsTrue(removed, "基于路径的注释应成功删除");
        }

        [Test]
        public void TestRichTextComment()
        {
            // 获取测试文件夹的GUID
            string guid = AssetDatabase.AssetPathToGUID(TestFolderPath);

            // 测试富文本注释
            string richTitle = "<b>粗体标题</b>";
            string richComment = "<color=#FF0000>红色文本</color>\n<b>粗体文本</b>\n<i>斜体文本</i>";

            FolderCommentManager.Instance.SetFolderComment(guid, richTitle, richComment);

            // 验证富文本注释是否添加成功
            var commentData = FolderCommentManager.Instance.GetFolderComment(guid);
            Assert.IsNotNull(commentData, "富文本注释数据不应为空");
            Assert.AreEqual(richTitle, commentData.title, "富文本标题应匹配");
            Assert.AreEqual(richComment, commentData.comment, "富文本注释内容应匹配");

            // 清理
            FolderCommentManager.Instance.RemoveFolderComment(guid);
        }

        [Test]
        public void TestUtilityFunctions()
        {
            // 测试文本裁剪功能
            string longText = "这是一个非常长的文本，用于测试文本裁剪功能";
            string croppedText = FolderCommentUtils.CropText(EditorStyles.label, longText, 100f);
            Assert.IsNotNull(croppedText, "裁剪后的文本不应为空");
            Assert.IsTrue(croppedText.Length <= longText.Length, "裁剪后的文本长度应小于等于原文本长度");

            // 测试日期格式化
            string formattedDate = FolderCommentUtils.FormatDateTime(System.DateTime.Now);
            Assert.IsNotNull(formattedDate, "格式化后的日期不应为空");
            Assert.IsTrue(formattedDate.Contains("-"), "格式化后的日期应包含分隔符");
        }
    }
}
