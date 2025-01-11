#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using Directory = System.IO.Directory;

namespace TBar.Editor
{
	internal class TBarSettingsProvider : SettingsProvider
	{
		private const string UXML = "Packages/com.kakacoding.tbar/Editor/UI/ProviderSettings.uxml";
		private const string SETTING_PROVIDER_PATH = "Preferences/TBar";
		private SerializedObject _toolbarSettings;
		private static ListView _toolbarListView;

		private static List<Type> ToolbarElementTypes
		{
			get
			{
				if (_ToolbarElementTypes == null)
				{
					var allTypes = TypeCache.GetTypesDerivedFrom<BaseToolbarElement>();
					var internalTypes = new List<Type>
					{
						typeof(ToolbarMenuInvoke),
						typeof(ToolbarThirdPartyInvoke),
						typeof(ToolbarSceneOpen),
						typeof(ToolbarSceneSelection),
						null,
						typeof(ToolbarSeparator),
						typeof(ToolbarSides),
					};
					
					_ToolbarElementTypes = allTypes.Except(internalTypes).ToList();
					if (_ToolbarElementTypes.Count > 0)
					{
						_ToolbarElementTypes.Add(null);
					}
					_ToolbarElementTypes.AddRange(internalTypes);
				}

				return _ToolbarElementTypes;
			}
		}

		private static List<Type> _ToolbarElementTypes; 
		private TBarSettingsProvider(string path, SettingsScope scopes = SettingsScope.User) : base(path, scopes)
		{
		}

		[SettingsProvider]
		public static SettingsProvider CreateProvider()
		{
			return new SettingsProvider(SETTING_PROVIDER_PATH, SettingsScope.User)
			{
				activateHandler = ActivateHandler,
				deactivateHandler = DeactivateHandler
			};
		}
		
		private static void ActivateHandler(string _, VisualElement rootElement)
		{
			ToolbarExtender.EnableZoneBackgroundColor = true;
			var window = EditorGUIUtility.Load(UXML) as VisualTreeAsset;
			if (window == null) return;
			window.CloneTree(rootElement);
			TBarUtility.AttachStyles(rootElement);
			
			ShowAdjustContainer(rootElement);
			ShowConfigContainer(rootElement);
			ShowScrollView(rootElement);
			ShowFootContainer(rootElement);
		}

		private static void DeactivateHandler()
		{
			ToolbarExtender.EnableZoneBackgroundColor = false;
		}

		private static void ShowAdjustContainer(VisualElement rootElement)
		{
			var ve = rootElement.Q<VisualElement>("AdjustContainer");
			var enableTBar = EnableCtrl.Create(
				() => "d_CustomTool@2x",
				() => "启用TBar",
				() => ToolbarExtender.EnableTBar,
				v=>
				{
					ToolbarExtender.EnableTBar = v;
					ToolbarExtender.Reload();
				});
			enableTBar.name = "EnableTBar";
			ve.Add(enableTBar);
			ve.Add(EnableCtrl.Create(
				() => "d_Slider Icon",
				() => "",
				() => ToolbarExtender.EnableAdjustLeftZone,
				v=>
				{
					ToolbarExtender.EnableAdjustLeftZone = v;
					SettingsService.NotifySettingsProviderChanged();
				}));
			var AdjustLeftZone = IntegerCtrl.Create(
				() => "调节左边栏范围",
				() => ToolbarExtender.LeftZoneWidth,
				v =>
				{
					if (ToolbarExtender.EnableAdjustLeftZone)
					{
						ToolbarExtender.LeftZoneWidth = v;
						ToolbarExtender.Reload();
					}
				}, null);
			AdjustLeftZone.name = "AdjustLeftZone";
			AdjustLeftZone.SetEnabled(ToolbarExtender.EnableAdjustLeftZone);
			ve.Add(AdjustLeftZone);
			
			ve.Add(EnableCtrl.Create(
				() => "d_Slider Icon",
				() => "",
				() => ToolbarExtender.EnableAdjustRightZone,
				v=>
				{
					ToolbarExtender.EnableAdjustRightZone = v;
					SettingsService.NotifySettingsProviderChanged();
				}));
			var AdjustRightZone = IntegerCtrl.Create(
				() => "调节右边栏范围",
				() => ToolbarExtender.RightZoneWidth,
				v =>
				{
					if (ToolbarExtender.EnableAdjustRightZone)
					{
						ToolbarExtender.RightZoneWidth = v;
						ToolbarExtender.Reload();
					}
				}, null);
			AdjustRightZone.name = "AdjustRightZone";
			AdjustRightZone.SetEnabled(ToolbarExtender.EnableAdjustRightZone);
			ve.Add(AdjustRightZone);
		}
		private static void ShowConfigContainer(VisualElement rootElement)
		{
			var container = rootElement.Q<VisualElement>("ConfigContainer");
			var labelConfig = new Label
			{
				name = "LabelConfig",
				text = "预设配置",
			};
			container.Add(labelConfig);
			
			var dpField = new DropdownField
			{
				name = "SelectConfig",
				choices = TBarConfig.ConfigMap.Keys.ToList(),
				value = TBarStatus.Instance.CurConfigName,
			};
			dpField.RegisterValueChangedCallback(evt =>
			{
				if(!TBarStatus.Instance.CurConfigName.Equals(evt.newValue))
				{
					TBarConfig.Instance.LoadInDefaultDirByName(evt.newValue);
				}
			});
			container.Add(dpField);

			var btnCreate = ImgBtnCtrl.Create(
				() => TBarUtility.ETextTextureDisplay.TextureText,
				() => "新增",
				() => "Packages/com.kakacoding.tbar/Editor/Resources/NewFile.png",
				() => "添加一个内容为空的配置",
				OnCreateConfig
			);
			btnCreate.name = "BtnCreate";
			container.Add(btnCreate);
			
			var btnDuplicate = ImgBtnCtrl.Create(
				() => TBarUtility.ETextTextureDisplay.TextureText,
				() => "复制",
				() => "Packages/com.kakacoding.tbar/Editor/Resources/DuplicateFile.png",
				() => "将当前配置复制一份副本",
				OnDuplicateConfig
			);
			btnDuplicate.name = "BtnDuplicate";
			container.Add(btnDuplicate);
			
			var btnDel = ImgBtnCtrl.Create(
				() => TBarUtility.ETextTextureDisplay.TextureText,
				() => "删除",
				() => "Packages/com.kakacoding.tbar/Editor/Resources/DeleteFile.png",
				() => "删除当前配置",
				OnDelConfig
			);
			btnDel.name = "BtnDel";
			container.Add(btnDel);
			
			var txtRename = new TextField
			{
				name = "TxtRename",
			};
			container.Add(txtRename);
			
			var btnRename = ImgBtnCtrl.Create(
				() => TBarUtility.ETextTextureDisplay.TextureText,
				() => "改名",
				() => "Packages/com.kakacoding.tbar/Editor/Resources/RenameFile.png",
				() => "改名当前配置",
				 () =>
				{
					var newName = rootElement.Q<TextField>("TxtRename").value;
					if (!string.IsNullOrEmpty(newName))
					{
						if (newName.Equals(dpField.value))
						{
							EditorUtility.DisplayDialog("TBar", "配置名称相同，不需修改！", "确定");	
						}
						else
						{
							TBarConfig.Instance.Rename(newName);	
						}
					}
					else
					{
						EditorUtility.DisplayDialog("TBar", "配置名称不能修改为空！", "确定");
					}
				});
			btnRename.name = "BtnRename";
			container.Add(btnRename);
			
			var btnExport = ImgBtnCtrl.Create(
				() => TBarUtility.ETextTextureDisplay.TextureText,
				() => "分享",
				() => "Packages/com.kakacoding.tbar/Editor/Resources/Share.png",
				() => "输出当前配置字符串",
				() => ExportConfigWindow.ShowWindow(TBarConfig.Instance.ExportConfigStr())
			);
			btnExport.name = "BtnExport";
			container.Add(btnExport);
			
			var btnImport = ImgBtnCtrl.Create(
				() => TBarUtility.ETextTextureDisplay.TextureText,
				() => "导入",
				() => "Packages/com.kakacoding.tbar/Editor/Resources/Import.png",
				() => "导入配置字符串",
				ImportConfigWindow.ShowWindow
			);
			btnImport.name = "BtnImport";
			container.Add(btnImport);
		}
		private static void ShowFootContainer(VisualElement rootElement)
		{
			var container = rootElement.Q<VisualElement>("FootContainer");
			var btnSave = ImgBtnCtrl.Create(
				() => TBarUtility.ETextTextureDisplay.TextureText,
				() => "保存配置",
				() => "Packages/com.kakacoding.tbar/Editor/Resources/Save.png",
				() => "保存当前配置",
				TBarConfig.Instance.Save
			);
			btnSave.name = "BtnSave";
			container.Add(btnSave);
		}

		private static void ShowScrollView(VisualElement rootElement)
		{
			var sv = rootElement.Q<VisualElement>("RecordContainer");
			_toolbarListView = new ListView
			{
				name = "ToolbarSettingListView",
				itemsSource = TBarConfig.Instance.Elements,
				reorderMode = ListViewReorderMode.Animated,
				selectionType = SelectionType.Multiple,
				reorderable = true,
				showAddRemoveFooter = true,
				showBorder = true,
				//fixedItemHeight = 20,//设置listview的item高度，不设置默认30
				makeItem = () =>
				{
					var container = new VisualElement
					{
						name = "ToolbarSettingRecordContainer",
					};
					return container;
				},
				bindItem = (container, i) =>
				{
					container.style.flexGrow = 1;//由于默认设置了fixedItemHeight的值，会在bindItem之前会将子节点的flexGrow置为0
					container.RemoveFromClassList("RecordItemContainer-Side");
					container.AddToClassList("RecordItemContainer");
					if (i < TBarConfig.Instance.Elements.Count)
					{
						TBarConfig.Instance.Elements[i].DrawInSettings(container);
					}
				}
			};
			_toolbarListView.itemIndexChanged += (add, remove) =>
			{
				ToolbarExtender.Reload();
			};
			_toolbarListView.Q<Button>("unity-list-view__remove-button").clickable = new Clickable(() =>
			{
				var idxes = _toolbarListView.selectedIndices.ToList();
				if (idxes.Count == 0)
				{
					var count = _toolbarListView.itemsSource.Count;
					if (count == 0) return;
					
					if (count > 0)
					{
						idxes.Add(count - 1);
					}
				}

				for (var i = idxes.Count - 1; i >= 0; --i)
				{
					TBarConfig.Instance.Elements.RemoveAt(idxes[i]);	
				}
				_toolbarListView.Rebuild();
				ToolbarExtender.Reload();
				var curCount = _toolbarListView.itemsSource.Count;
				if (curCount == 0)
				{
					_toolbarListView.selectedIndex = -1;
					return;
				}
				var newSelectIdx = idxes[0];
				if (curCount <= newSelectIdx)
				{
					newSelectIdx = curCount - 1;
				}

				_toolbarListView.selectedIndex = newSelectIdx;
			});
			_toolbarListView.Q<Button>("unity-list-view__add-button").clickable = new Clickable(() =>
			{
				var menu = new GenericMenu();
				foreach (var toolbarElementType in ToolbarElementTypes)
				{
					if (toolbarElementType == null)
					{
						menu.AddSeparator("");
					}
					else
					{
						var display = ToolbarElementDisplay.GetDisplay(toolbarElementType);
						if (IsToolbarElementValid(toolbarElementType))
						{
							menu.AddItem(new GUIContent(display), false, target =>
							{
								if (target is ToolbarSceneSelection selection)
								{
									selection.Init();
								}

								var idx = _toolbarListView.selectedIndex == -1 ? 0 : _toolbarListView.selectedIndex + 1;
								TBarConfig.Instance.Elements.Insert(idx, target as BaseToolbarElement);
								_toolbarListView.Rebuild();
								ToolbarExtender.Reload();
								_toolbarListView.selectedIndex = idx;
							}, Activator.CreateInstance(toolbarElementType));
						}
						else
						{
							menu.AddDisabledItem(new GUIContent(display), false);
						}
					}
				}
				menu.ShowAsContext();
			});
			sv.Add(_toolbarListView);
		}

		private static void OnDuplicateConfig()
		{
			TBarConfig.Instance.DuplicateCurConfig(TBarConfig.NewDefaultName);
		}
		
		private static void OnCreateConfig()
		{
			var defaultNewName = TBarConfig.NewDefaultName;
			var names = Directory.GetFiles(TBarConfig.ConfigDir).Select(Path.GetFileNameWithoutExtension).ToList();
			if (names.Contains(defaultNewName))
			{
				for (var i = 0; i <= names.Count;++i)
				{
					var name = $"{defaultNewName}{i + 1}";
					if (names.Contains(name)) continue;
					TBarConfig.Instance.CreateNewConfig(name);
					return;
				}
			}
			TBarConfig.Instance.CreateNewConfig(defaultNewName);
		}
		
		private static void OnDelConfig()
		{
			var keys = TBarConfig.ConfigMap.Keys.ToList();
			var idx = keys.FindIndex(x => x.Equals(TBarStatus.Instance.CurConfigName));
			var newName = string.Empty;
			if (idx > 0)
			{
				newName = keys[--idx];
			}
			else
			{
				if (keys.Count > 1)
				{
					newName = keys[1];
				}
			}

			if (string.IsNullOrEmpty(newName))
			{
				EditorUtility.DisplayDialog("TBar", "无法删除最后一个配置", "确定");
			}
			else
			{
				if (EditorUtility.DisplayDialog("TBar", $"删除配置[{TBarStatus.Instance.CurConfigName}]", "确定", "取消"))
				{
					TBarConfig.Instance.DeleteCurAndLoadOther(newName);	
				}
			}
		}

		private static bool IsToolbarElementValid(Type t)
		{
			if (t == typeof(ToolbarSides))
			{
				return TBarConfig.Instance.Elements.All(elem => elem.GetType() != t);
			}
			var methodInfo = t.GetMethod("IsValid", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (methodInfo == null) return true;
			var result = methodInfo.Invoke(null, null);
			return (bool)result;
		}
	}
}
#endif