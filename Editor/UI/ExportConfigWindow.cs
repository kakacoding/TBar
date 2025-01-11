#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace TBar.Editor
{
    public class ExportConfigWindow : EditorWindow
    {
        private string _inputText = "";
        private GUIStyle _textAreaStyle;
        private GUIStyle _notificationStyle;
        private string _notificationText = ""; // 用于存储提示文本
        private float _notificationTime = 0f; // 用于跟踪提示的显示时间
        private GUIStyle _buttonStyle; // 定义按钮的样式
        private Vector2 _scrollPosition; // 滚动视图位置

        public static void ShowWindow(string text)
        {
            var window = GetWindow<ExportConfigWindow>("导出配置");
            window._inputText = text;
            window.Show();
        }

        private void OnEnable()
        {
            _textAreaStyle = new GUIStyle(EditorStyles.textArea)
            {
                wordWrap = true,
            };
            // 固定窗口大小
            minSize = new Vector2(800, 600);
            maxSize = new Vector2(800, 600);

            // 初始化提示文本的样式
            _notificationStyle = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 16,
                normal = { textColor = Color.white, background = MakeTexture(2, 2, new Color(0.345f, 0.345f, 0.345f, 0.8f)) }
            };

            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fixedHeight = 50,
            };
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            // 添加滚动视图
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            _inputText = EditorGUILayout.TextArea(_inputText, _textAreaStyle, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("复制配置串到剪贴板", _buttonStyle))
            {
                EditorGUIUtility.systemCopyBuffer = _inputText;
                ShowNotification("已复制到剪贴板");
            }

            // 显示提示文本
            if (!string.IsNullOrEmpty(_notificationText))
            {
                // 计算提示框的宽高，并使其居中
                var width = position.width * 0.6f; // 60% 窗口宽度
                var height = 40f; // 固定高度
                var rect = new Rect((position.width - width) / 2, (position.height - height) / 2, width, height);
                GUI.Box(rect, _notificationText, _notificationStyle); // 显示带背景的提示
            }

            EditorGUILayout.EndVertical();
        }

        private void ShowNotification(string message)
        {
            _notificationText = message;
            _notificationTime = Time.realtimeSinceStartup + 1f; // 提示显示1秒
        }

        private void Update()
        {
            if (_notificationTime > 0f && Time.realtimeSinceStartup > _notificationTime)
            {
                _notificationText = ""; // 隐藏提示
            }
        }

        private Texture2D MakeTexture(int width, int height, Color color)
        {
            var texture = new Texture2D(width, height);
            var pix = new Color[width * height];
            for (var i = 0; i < pix.Length; i++)
            {
                pix[i] = color;
            }
            texture.SetPixels(pix);
            texture.Apply();
            return texture;
        }
    }
}
#endif
