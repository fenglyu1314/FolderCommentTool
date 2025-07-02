using UnityEngine;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using System;

namespace TATools.FolderCommentTool.Tests
{
    /// <summary>
    /// 文件夹注释测试运行器
    /// </summary>
    public static class FolderCommentTestRunner
    {
        /// <summary>
        /// 运行所有测试
        /// </summary>
        [MenuItem("TATools/文件夹注释工具/运行所有测试")]
        public static void RunAllTests()
        {
            // 创建测试运行器API
            var testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();

            // 创建测试过滤器
            var filter = new Filter()
            {
                testMode = TestMode.EditMode, // 编辑器模式测试
                assemblyNames = new[] { "TATools.FolderCommentTool.Editor" } // 只运行我们的程序集中的测试
            };

            // 创建回调
            var callback = new TestRunnerCallback();

            // 运行测试
            testRunnerApi.Execute(new ExecutionSettings(filter));

            Debug.Log("开始运行文件夹注释工具测试...");
        }

        /// <summary>
        /// 测试运行器回调
        /// </summary>
        private class TestRunnerCallback : ICallbacks
        {
            public void RunStarted(ITestAdaptor testsToRun)
            {
                Debug.Log($"开始运行测试，共 {testsToRun.TestCaseCount} 个测试用例");
            }

            public void RunFinished(ITestResultAdaptor result)
            {
                if (result.TestStatus == TestStatus.Passed)
                {
                    Debug.Log($"<color=green>测试完成！所有测试通过！</color>");
                }
                else
                {
                    Debug.LogError($"测试完成，但有失败的测试。通过: {result.PassCount}，失败: {result.FailCount}，跳过: {result.SkipCount}");
                }

                // 显示结果对话框
                EditorUtility.DisplayDialog(
                    "测试结果",
                    $"测试完成！\n通过: {result.PassCount}\n失败: {result.FailCount}\n跳过: {result.SkipCount}",
                    "确定"
                );
            }

            public void TestStarted(ITestAdaptor test)
            {
                if (test.IsSuite) return; // 忽略测试套件
                Debug.Log($"开始测试: {test.FullName}");
            }

            public void TestFinished(ITestResultAdaptor result)
            {
                if (result.Test.IsSuite) return; // 忽略测试套件

                if (result.TestStatus == TestStatus.Passed)
                {
                    Debug.Log($"<color=green>测试通过: {result.Test.Name}</color>");
                }
                else if (result.TestStatus == TestStatus.Failed)
                {
                    Debug.LogError($"测试失败: {result.Test.Name}\n错误信息: {result.Message}\n{result.StackTrace}");
                }
                else
                {
                    Debug.LogWarning($"测试跳过: {result.Test.Name}");
                }
            }
        }

        /// <summary>
        /// 创建测试环境并运行测试
        /// </summary>
        [MenuItem("TATools/文件夹注释工具/创建测试环境并运行测试")]
        public static void SetupAndRunTests()
        {
            // 先创建测试文件夹
            FolderCommentTestHelper.CreateTestFolders();

            // 添加测试注释
            FolderCommentTestHelper.ApplyTestFolderTemplates();

            // 运行测试
            RunAllTests();
        }

        /// <summary>
        /// 清理测试环境
        /// </summary>
        [MenuItem("TATools/文件夹注释工具/清理测试环境")]
        public static void CleanupTestEnvironment()
        {
            // 清除测试注释
            FolderCommentTestHelper.ClearTestFolderComments();

            // 删除测试文件夹
            FolderCommentTestHelper.DeleteTestFolders();

            Debug.Log("测试环境已清理");
        }
    }
}
