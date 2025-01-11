#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TBar.Editor
{
    public class ImgToolbarBtnCtrl : ToolbarButton
    {
	    public delegate TBarUtility.ETextTextureDisplay DisplayGetter();
	    public delegate string TextGetter();
	    public delegate string TextureGetter();
	    public delegate string TooltipGetter();
	    public delegate void Click();

	    public static ImgToolbarBtnCtrl Create(DisplayGetter displayGetter, TextGetter textGetter, TextureGetter textureGetter, TooltipGetter tooltipGetter, Click onClick)
        {
            var imgBtn = new ImgToolbarBtnCtrl
            {
	            name = nameof(ImgToolbarBtnCtrl),
            	tooltip = tooltipGetter(),
	            style =
	            {
		            flexDirection = displayGetter() == TBarUtility.ETextTextureDisplay.TextureText ? FlexDirection.Row : FlexDirection.RowReverse
	            }
            };

            var bHasTex  = TryGetTexture(textureGetter(), out var tex);
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

            imgBtn.clicked += () =>
            {
	            onClick();
            };

            return imgBtn;
        }

        public void SetTexturePath(string texturePath)
        {
	        if(TryGetTexture(texturePath, out var tex))
	        {
		        var img = this.Q<VisualElement>("Image");
		        if (img != null)
		        {
			        img.style.backgroundImage = tex;
		        }
	        }
        }

        private static bool TryGetTexture(string texturePath, out Texture2D texture)
        {
	        texture = null;
	        if (!string.IsNullOrEmpty(texturePath))
	        {
		        texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
		        if (texture == null)
		        {
			        texture = AssetDatabase.LoadAssetAtPath<Texture2D>(TBarUtility.LOST_ICON);
		        }
		        return texture != null;
	        }

	        return false;
        }
    }
}
#endif