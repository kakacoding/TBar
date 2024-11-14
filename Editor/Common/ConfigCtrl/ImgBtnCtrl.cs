#if UNITY_EDITOR && TBAR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TBar.Editor
{
    public class ImgBtnCtrl : VisualElement
    {
	    internal delegate TBarUtility.ETextTextureDisplay DisplayGetter();
	    internal delegate string TextGetter();
	    internal delegate string TextureGetter();
	    internal delegate string TooltipGetter();
	    internal delegate void Click();

        internal static VisualElement Create(DisplayGetter displayGetter, TextGetter textGetter, TextureGetter textureGetter, TooltipGetter tooltipGetter, Click onClick)
        {
            var imgBtn = new Button  
            {
	            name = nameof(ImgBtnCtrl),
            	tooltip = tooltipGetter(),
	            style =
	            {
		            flexDirection = displayGetter() == TBarUtility.ETextTextureDisplay.TextureText ? FlexDirection.Row : FlexDirection.RowReverse
	            }
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

            var img = new VisualElement
            {
	            name = "Image",
	            style =
	            {
		            backgroundImage = bHasTex ? tex : null,
	            }
            };
            img.style.minWidth = bHasTex ? img.style.minHeight : 0;
            img.style.maxWidth = bHasTex ? img.style.maxHeight : 0;
            imgBtn.Add(img);

            var label = new Label
            {
	            name = "Label",
	            text = textGetter()
            };
            imgBtn.Add(label);
            
            imgBtn.clicked += () => onClick();
            return imgBtn;
        }
    }
}
#endif