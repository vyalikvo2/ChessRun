using UnityEngine;
using UnityEngine.UI;

namespace ChessRun.GUI.Menu
{
    public class LevelCompleteMenu: BaseGameMenu
    {
        [HideInInspector] public GameUI gameUI;

        public override void onShow()
        {
            GameObject lvlTxtObj = transform.Find("LevelTxt").gameObject;
            lvlTxtObj.GetComponent<Text>().text = (gameUI.game.getCurrentLevel() + 1) + "";
        }
        
        public void btn_replayLevel()
        {
            gameUI.setMenu(MenuType.NONE);
            gameUI.game.replayLevel();
        }
        
        public void btn_nextLevel()
        {
            gameUI.setMenu(MenuType.NONE);
            gameUI.game.nextLevel();
        }
        
        public void btn_toMenu()
        {
            
        }
    }
    
    
}