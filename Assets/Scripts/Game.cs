using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

using ChessRun.GInput;
using ChessRun.Board;
using ChessRun.Board.Controllers;

using ChessRun.GUI;

namespace ChessRun
{
	public class Game : MonoBehaviour
	{

		public static float TO_UNITS = 0.01f;
		public static float TO_PIXELS = 1 / TO_UNITS;
		public static float CELL_SIZE = 150;
		public static float POS_TO_COORDS = TO_UNITS * CELL_SIZE;
		public static float COORDS_TO_POS = 1 / (POS_TO_COORDS);

		public Camera camera;

		public static GameController gameController;

		[SerializeField] public GameUI gameUI;

		[HideInInspector] public GameInput gameInput;
		[HideInInspector] public ChessBoard board;
		[HideInInspector] public FightUI fightUI;

		private Vector3 _cameraLerp;

		private List<string[]> _levels = new List<string[]>();

		private int _currentLevel = 0;


		private void setupLevels()
		{
			_levels.Add(new string[]
			{
				"E",
				"O",
				"O",
				"K"
			});

			_levels.Add(new string[]
			{
				"E",
				"O",
				"1",
				"K"
			});

			_levels.Add(new string[]
			{
				"EO",
				"1O",
				"O1",
				"OK"
			});

			_levels.Add(new string[]
			{
				"E",
				"22",
				"OOOOO1",
				"HK"
			});
		}

		void Start()
		{
			board = GetComponent<ChessBoard>();
			fightUI = GetComponent<FightUI>();
			gameController = GetComponent<GameController>();
			gameInput = GetComponent<GameInput>();
			gameUI = GetComponent<GameUI>();

			PieceInteraction.game = this;

			setupLevels();
			board.Setup();

			GameData.choosedLevel = 3;
			setLevel(GameData.choosedLevel);

			gameUI.setMenu(MenuType.NONE);
		}


		public void setLevel(int level)
		{
			board.clearBoard();

			_currentLevel = level;

			Debug.Log("Setup level " + _currentLevel);
			board.CreateByMap(_levels[_currentLevel]);
			gameController.startLevel();

			gameUI.levelText.GetComponent<Text>().text = "Уровень: " + (_currentLevel + 1);
		}

		public int getCurrentLevel()
		{
			return _currentLevel;
		}

		public void replayLevel()
		{
			setLevel(_currentLevel);
		}

		public void nextLevel()
		{
			_currentLevel++;
			setLevel(_currentLevel);
		}

		public void centerCamera(Vector3 pos, bool animate = true)
		{
			_cameraLerp = pos + new Vector3(1, 1, -1);
			if (!animate)
			{
				camera.transform.position = _cameraLerp;
			}
		}

		void Update()
		{
			if (_cameraLerp != null)
			{
				camera.transform.position = Vector3.Lerp(camera.transform.position, _cameraLerp, 0.1f);
			}
		}
	}
	
}
