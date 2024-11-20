#if UNITY_EDITOR && TBAR
using UnityEditor;
using UnityEngine;

namespace TBar.Editor
{
    public class ImportConfigWindow : EditorWindow
    {
        private string _inputText = "";
        private GUIStyle _textAreaStyle;
        private GUIStyle _buttonStyle; // 定义按钮的样式
        private Vector2 _scrollPosition; // 滚动视图位置

        public static void ShowWindow()
        {
            var window = GetWindow<ImportConfigWindow>("导入配置");
            window.Show();
        }

        private void OnEnable()
        {
            var configStr = EditorGUIUtility.systemCopyBuffer.Trim();
            if (!string.IsNullOrEmpty(configStr) && configStr.StartsWith(TBarConfig.ConfigTitle))
            {
                _inputText = configStr;
            }

            _textAreaStyle = new GUIStyle(EditorStyles.textArea)
            {
                wordWrap = true,
            };

            // 固定窗口大小
            minSize = new Vector2(800, 600);
            maxSize = new Vector2(800, 600);

            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fixedHeight = 50
            };
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            // 添加滚动视图
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            _inputText = EditorGUILayout.TextArea(_inputText, _textAreaStyle, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("导入为新配置", _buttonStyle))
            {
                TBarConfig.Instance.ImportConfigStr(_inputText);
                Close();
            }

            EditorGUILayout.EndVertical();
        }
    }
}
#endif