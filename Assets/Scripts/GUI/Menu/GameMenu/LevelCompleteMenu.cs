using UnityEngine;
using UnityEngine.UI;

namespace ChessRun.GUI.Menu
{
    public class LevelCompleteMenu: BaseGameMenu
    {
        [HideInInspector] public GameUI GameUI;

        protected override void OnShow()
        {
            GameObject lvlTxtObj = transform.Find("LevelTxt").gameObject;
            lvlTxtObj.GetComponent<Text>().text = (GameUI.GameEngine.GetCurrentLevel() + 1) + "";
        }
        
        public void btn_replayLevel()
        {
            GameUI.SetMenu(MenuType.NONE);
            GameUI.GameEngine.ReplayLevel();
        }
        
        public void btn_nextLevel()
        {
            GameUI.SetMenu(MenuType.NONE);
            GameUI.GameEngine.NextLevel();
        }
        
        public void btn_toMenu()
        {
            
        }
    }
    
    
}