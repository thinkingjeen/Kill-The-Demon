using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MobileConsole
{
	internal class Style : MonoBehaviour
	{
		private StyleSettings styleSettings;

		[SerializeField]
		private StyleType styleType;

		private bool hasGraphicBeenAssigned;
		private Graphic graphic;

		public void SetStyleSettings(StyleSettings styleSettings)
		{
			this.styleSettings = styleSettings;
			Refresh();
		}
		
		public void SetStyle(StyleType styleType)
		{
			this.styleType = styleType;
			Refresh();
		}

		private void Refresh()
		{
			if (!hasGraphicBeenAssigned)
			{
				AssignGraphic();
			}

			if (hasGraphicBeenAssigned)
			{
				switch (styleType)
				{
					case StyleType.LightBackground:
						graphic.color = styleSettings.LightBackgroundColor;
						break;
					case StyleType.DarkBackground:
						graphic.color = styleSettings.DarkBackgroundColor;
						break;
					case StyleType.SelectedBackground:
						graphic.color = styleSettings.SelectedBackgroundColor;
						break;
					case StyleType.Text:
						graphic.color = styleSettings.TextColor;
						break;
					case StyleType.SelectedText:
						graphic.color = styleSettings.SelectedTextColor;
						break;
					case StyleType.ScrollbarBackground:
						graphic.color = styleSettings.ScrollbarBackgroundColor;
						break;
					case StyleType.ScrollbarHandle:
						graphic.color = styleSettings.ScrollbarHandleColor;
						break;
					case StyleType.ScrollBackground:
						graphic.color = styleSettings.ScrollBackgroundColor;
						break;
					case StyleType.LogBackground:
						((Image) graphic).sprite = styleSettings.LogBackgroundSprite;
						break;
					case StyleType.LogCountBackground:
						graphic.color = styleSettings.LogCountBackgroundColor;
						break;
				}
			}
		}

		private void AssignGraphic()
		{
			switch (styleType)
			{
				case StyleType.Text:
				case StyleType.SelectedText:
					graphic = GetComponent<TMP_Text>();
					break;
				default:
					graphic = GetComponent<Image>();
					break;
			}

			hasGraphicBeenAssigned = true;
		}
	}
}
