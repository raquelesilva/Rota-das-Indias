using UnityEngine;
using UnityEngine.UI;

namespace FancyCrab.CustomPackages.FirstPersonController
{
    public abstract class CrosshairBase : MonoBehaviour
    {
        [SerializeField] protected Sprite crosshairSprite;
        
        private Image crosshairImage;
        private void Awake()
        {
            if (crosshairImage == null)
            {
                crosshairImage = GetComponentInChildren<Image>();
                if(crosshairSprite != null)
                {
                    crosshairImage.sprite = crosshairSprite;
                }
            }
        }
        public void ShowCrosshair(bool show)
        {
            if (crosshairImage == null) return; 

            crosshairImage.gameObject.SetActive(show);
        }

        public void ApplyCrosshair(Sprite sprite, Color color, bool visible)
        {
            if (crosshairImage == null) return;
            if (sprite != null)
            {
                crosshairImage.sprite = sprite;
                crosshairImage.color = color;
                crosshairImage.gameObject.SetActive(visible);
            }
        }
    }
}
