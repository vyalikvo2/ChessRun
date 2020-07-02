using UnityEngine;
using UnityEngine.UI;

namespace ChessRun.GUI.Menu
{
    public class ReplayMenu: BaseGameMenu
    {
        [HideInInspector] public GameUI gameUI;

        public void btn_replayLevel()
        {
            gameUI.setMenu(MenuType.NONE);
            gameUI.game.replayLevel();
        }
    }
    
    
}