using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class ReplayMenu: BaseGameMenu
    {
        [HideInInspector] public GameUI gameUI;

        public void btn_replayLevel()
        {
            gameUI.setMenu(GameUI.MENU_NONE);
            gameUI.game.replayLevel();
        }
    }
    
    
}