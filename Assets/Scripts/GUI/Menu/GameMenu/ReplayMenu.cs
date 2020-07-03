using UnityEngine;
using UnityEngine.UI;

namespace ChessRun.GUI.Menu
{
    public class ReplayMenu: BaseGameMenu
    {
        [HideInInspector] public GameUI GameUI;

        public void btn_replayLevel()
        {
            GameUI.SetMenu(MenuType.NONE);
            GameUI.GameEngine.ReplayLevel();
        }
    }
    
    
}