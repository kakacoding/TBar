using UnityEditor;

namespace TBar.Editor
{
    internal static partial class TBarUtility
    {
        [MenuItem("Tools/1")]
        static void Func1() => EditorUtility.DisplayDialog("测试菜单", "Tools/1", "确定", "取消");
        
        [MenuItem("Tools/2")]
        static void Func2() => EditorUtility.DisplayDialog("测试菜单", "Tools/2", "确定", "取消");
        
        [MenuItem("Tools/3")]
        static void Func3() => EditorUtility.DisplayDialog("测试菜单", "Tools/3", "确定", "取消");
    }
}