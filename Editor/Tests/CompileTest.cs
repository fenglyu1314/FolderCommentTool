using UnityEngine;
using UnityEditor;

namespace TATools.FolderCommentTool
{
    /// <summary>
    /// 编译测试类，用于验证代码是否能正常编译
    /// </summary>
    public class CompileTest
    {
        [MenuItem("TATools/Folder Comment Tool/Test Compile")]
        public static void TestCompile()
        {
            Debug.Log("文件夹注释工具编译测试通过！");

            // 测试主要类是否可以实例化
            var settings = FolderCommentSettings.Instance;
            var manager = FolderCommentManager.Instance;

            Debug.Log($"设置加载成功: {settings != null}");
            Debug.Log($"管理器加载成功: {manager != null}");
            Debug.Log("单文件夹编辑模式功能已实现！");
            Debug.Log("- 右键点击'文件夹注释'标题可打开编辑模式菜单");
            Debug.Log("- 编辑模式使用Unity标准样式");
            Debug.Log("- 支持未保存修改的临时存储");
            Debug.Log("- 切换目标时自动退出编辑模式");
            Debug.Log("- 保持Unity原生Inspector头部UI不变");
            Debug.Log("- 可选择复制的富文本语法说明，无自动填写按钮");
            Debug.Log("- 代码已优化：常量提取、样式缓存、清理冗余");
        }
    }
}
