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
                stretchHeight = true
            };
            minSize = new Vector2(800, 600);
            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fixedHeight = 50
            };
        }
        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            _inputText = EditorGUILayout.TextArea(_inputText, _textAreaStyle);
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