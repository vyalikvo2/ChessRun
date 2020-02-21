using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class BaseGameMenu : MonoBehaviour
    {
        private bool _visible = true;

        public virtual void onShow()
        {
            
        }
        
        public void setVisible(bool visibility)
        {
            Text[] texts= GetComponentsInChildren<Text>();
            for (int i = 0; i < texts.Length; i++)
                texts[i].enabled = visibility;
            
            Image[] images= GetComponentsInChildren<Image>();
            for (int i = 0; i < images.Length; i++)
                images[i].enabled = visibility;
            
            Button[] buttons= GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
                buttons[i].enabled = visibility;

            _visible = visibility;

            if (visibility)
            {
                onShow();
            }
        }
    }
}