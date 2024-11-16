#if UNITY_EDITOR && TBAR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TBar.Editor
{
    public class MenuItemSelectWindow : EditorWindow
    {
        private List<string> _allMenuItems = new(); // 所有菜单项
        private List<string> _filteredMenuItems = new(); // 过滤后的菜单项
        private string _searchString = ""; // 搜索框内容
        private TaskCompletionSource<string> _taskCompletionSource; // 异步任务源
        private string _selectedItem; // 选中的菜单项
        private Action<string> _onSelect; // 回调函数
        private ListView _listView; // ListView 组件

        /// <summary>
        /// 同步调用接口
        /// </summary>
        /// <param name="onSelectCallback"></param>
        public static void ShowWindow(Action<string> onSelectCallback)
        {
            var window = GetWindow<MenuItemSelectWindow>("菜单列表 - 双击选择菜单");
            window._onSelect = onSelectCallback;
            window.LoadMenuItems();
            window.Show();
        }

        /// <summary>
        /// 异步调用接口
        /// </summary>
        /// <returns></returns>
        public static Task<string> ShowWindowAsync()
        {
            var window = GetWindow<MenuItemSelectWindow>("菜单列表 - 双击选择菜单");
            window._taskCompletionSource = new TaskCompletionSource<string>();
            window.LoadMenuItems();
            window.Show();
            return window._taskCompletionSource.Task;
        }

        private void CreateGUI()
        {
            var root = rootVisualElement;

            // 搜索框容器
            var toolbar = new Toolbar();
            var searchField = new ToolbarSearchField
            {
                value = _searchString
            };
            searchField.RegisterValueChangedCallback(evt =>
            {
                _searchString = evt.newValue;
                FilterMenuItems(); // 过滤菜单项并刷新 ListView
            });
            searchField.style.flexGrow = 1;
            toolbar.Add(searchField);
            root.Add(toolbar);

            // 初始化 ListView
            _listView = new ListView
            {
                style =
                {
                    flexGrow = 1
                },
                showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly,
                selectionType = SelectionType.Single,
                makeItem = () =>
                {
                    var label = new Label
                    {
                        style =
                        {
                            unityTextAlign = TextAnchor.MiddleLeft,
                            flexGrow = 1
                        }
                    };
                    return label;
                },
                bindItem = (element, index) =>
                {
                    if (element is Label label && index < _filteredMenuItems.Count)
                    {
                        label.text = _filteredMenuItems[index];
                    }
                }
            };

            _listView.onSelectionChange += objects =>
            {
                if (objects.FirstOrDefault() is string selected)
                {
                    _selectedItem = selected;
                }
            };

            _listView.onItemsChosen += objects =>
            {
                if (objects.FirstOrDefault() is string chosen)
                {
                    _onSelect?.Invoke(chosen);
                    Close();
                }
            };

            root.Add(_listView);

            // 加载初始菜单项
            if (_allMenuItems.Any())
            {
                RefreshListView();
            }
        }

        private void OnDestroy()
        {
            // 如果是异步调用，完成任务返回选中的值
            _taskCompletionSource?.TrySetResult(_selectedItem);
        }

        private void LoadMenuItems()
        {
            // 获取所有菜单项
            var methods = TypeCache.GetMethodsWithAttribute<MenuItem>();
            _allMenuItems = methods
                .SelectMany(method => method.GetCustomAttributes<MenuItem>().Select(attr => attr.menuItem))
                .Distinct()
                .OrderBy(item => item)
                .ToList();

            // 初始化筛选列表并刷新
            _filteredMenuItems = new List<string>(_allMenuItems);
            RefreshListView();
        }

        private void FilterMenuItems()
        {
            // 根据搜索框内容筛选菜单项
            if (string.IsNullOrEmpty(_searchString))
            {
                _filteredMenuItems = new List<string>(_allMenuItems);
            }
            else
            {
                _filteredMenuItems = _allMenuItems
                    .Where(item => item.IndexOf(_searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            }

            RefreshListView();
        }

        private void RefreshListView()
        {
            // 更新 ListView 数据源并重建
            _listView.itemsSource = _filteredMenuItems;
            _listView.Rebuild();
        }
        
        private void OnGUI()
        {
            // 监听按键事件
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            {
                Close();  // 按下 ESC 时关闭窗口
                Event.current.Use();  // 使用事件以防止进一步传播
            }
        }
    }
}
#endif