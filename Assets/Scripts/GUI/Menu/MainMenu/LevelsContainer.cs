using UnityEngine;
using UnityEngine.UI;

namespace ChessRun.GUI.Menu
{
	public class LevelsContainer : MonoBehaviour
	{
		private GameObject slotPrefab;

		private static int W = 100;
		private static int H = 100;
		private static int W_S = 20;
		private static int H_S = 20;
		private static int MAX_X = 720;

		GameObject[] levelsList;

		// Use this for initialization
		void Start()
		{

			slotPrefab = Resources.Load("prefabs/menu/level_slot") as GameObject;

			int X = 0;
			int Y = 0;

			for (int i = 0; i < 3; i++)
			{

				GameObject level = (GameObject) Instantiate(slotPrefab);
				RectTransform rect = (RectTransform) level.GetComponent<RectTransform>();
				rect.SetParent(GetComponent<RectTransform>() as RectTransform);
				//rect.anchoredPosition = new Vector2(X, Y);

				(level.transform.transform.Find("level").gameObject.GetComponent<Text>() as Text).text = "" + (i + 1);

				ChooseLevelButton lvlComponent = (ChooseLevelButton) level.GetComponent<ChooseLevelButton>();
				lvlComponent.level = i;

				X += W + W_S;
				if (X > MAX_X)
				{
					X = 0;
					Y -= H + H_S;
				}
			}

		}
	}
	
}
