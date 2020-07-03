using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

using ChessRun.GInput;
using ChessRun.Board;
using ChessRun.Board.Controllers;

using ChessRun.GUI;

namespace ChessRun
{
	public class GameEngine : MonoBehaviour
	{

		public static float TO_UNITS = 0.01f;
		public static float TO_PIXELS = 1 / TO_UNITS;
		public static float CELL_SIZE = 150;
		public static float POS_TO_COORDS = TO_UNITS * CELL_SIZE;
		public static float COORDS_TO_POS = 1 / (POS_TO_COORDS);
		
		public static СGameController GameController;


		[SerializeField] private Camera _camera;
		[SerializeField] public GameUI GameUI;

		[HideInInspector] public GameInput GameInput;
		[HideInInspector] public ChessBoard Board;
		[HideInInspector] public FightUI FightUi;

		private Vector3 _cameraLerp;

		private List<string[]> _levels = new List<string[]>();

		private int _currentLevel = 0;


		private void _setupLevels()
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
			Board = GetComponent<ChessBoard>();
			FightUi = GetComponent<FightUI>();
			GameController = GetComponent<СGameController>();
			GameInput = GetComponent<GameInput>();
			GameUI = GetComponent<GameUI>();

			PieceInteraction.Game = this;

			_setupLevels();
			Board.Setup();

			GameData.ChoosedLevel = 3;
			_setLevel(GameData.ChoosedLevel);

			GameUI.SetMenu(MenuType.NONE);
		}


		private void _setLevel(int level)
		{
			Board.ClearBoard();

			_currentLevel = level;

			Debug.Log("Setup level " + _currentLevel);
			Board.CreateByMap(_levels[_currentLevel]);
			GameController.StartLevel();

			GameUI.LevelText.GetComponent<Text>().text = "Уровень: " + (_currentLevel + 1);
		}

		public int GetCurrentLevel()
		{
			return _currentLevel;
		}

		public void ReplayLevel()
		{
			_setLevel(_currentLevel);
		}

		public void NextLevel()
		{
			_currentLevel++;
			_setLevel(_currentLevel);
		}

		public void CenterCamera(Vector3 pos, bool animate = true)
		{
			_cameraLerp = pos + new Vector3(1, 1, -1);
			if (!animate)
			{
				_camera.transform.position = _cameraLerp;
			}
		}

		void Update()
		{
			if (_cameraLerp != null)
			{
				_camera.transform.position = Vector3.Lerp(_camera.transform.position, _cameraLerp, 0.1f);
			}
		}
	}
	
}
