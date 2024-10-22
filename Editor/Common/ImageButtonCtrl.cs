#if UNITY_EDITOR && TBAR
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TBar.Editor
{
    public class ImageButtonCtrl : VisualElement
    {
	    internal delegate TBarUtility.ETextTextureDisplay DisplayGetter();
	    internal delegate string TextGetter();
	    internal delegate string TextureGetter();
	    internal delegate string TooltipGetter();
	    internal delegate void Click();

        internal static VisualElement Create(DisplayGetter displayGetter, TextGetter textGetter, TextureGetter textureGetter, TooltipGetter tooltipGetter, Click onClick)
        {
            var imgBtn = new ToolbarButton  
            {
	            name = nameof(ImageButtonCtrl),
            	tooltip = tooltipGetter(),
            };

            var bHasTex = false;
            Texture2D tex = null;
            var texturePath = textureGetter();
            if (!string.IsNullOrEmpty(texturePath))
            {
            	tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            	if (tex == null)
            	{
            		tex = AssetDatabase.LoadAssetAtPath<Texture2D>(TBarUtility.LOST_ICON);
            	}
            	bHasTex = tex != null;
            }

            var btnText = textGetter();
            var helpBox = new HelpBox(btnText, HelpBoxMessageType.Info)
            {
            	style =
            	{
            		flexDirection = displayGetter() == TBarUtility.ETextTextureDisplay.TextTexture ? FlexDirection.Row : FlexDirection.RowReverse
            	}
            };
            imgBtn.Add(helpBox);
            var texFieldInHelpBox = helpBox.Children().FirstOrDefault(child => child is not Label);
            if (texFieldInHelpBox != null)
            {
            	texFieldInHelpBox.style.backgroundImage = bHasTex ? tex : null;
            	texFieldInHelpBox.style.minWidth = bHasTex ? texFieldInHelpBox.style.minHeight : 0;
            	texFieldInHelpBox.style.maxWidth = bHasTex ? texFieldInHelpBox.style.maxHeight : 0;
            }

            imgBtn.clicked += () =>
            {
	            onClick();
            };

            return imgBtn;
        }
    }
}
#endif