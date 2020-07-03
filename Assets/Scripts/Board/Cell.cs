using UnityEngine;

using ChessRun.Board.Pieces;

namespace ChessRun.Board
{
	public class Cell : MonoBehaviour
	{
		[HideInInspector] public Vector2 Pos = Vector2.zero;
		
		[SerializeField] private Sprite _whiteCell;
		[SerializeField] private Sprite _blackCell;

		private SpriteRenderer _renderer;
		private Sprite _bg = null;
		private Sprite Bg
		{
			get { return _bg; }
			set
			{
				_bg = value;
				if (_bg != null)
					_renderBg();
			}
		}

		[HideInInspector] public BasePiece Piece;
		[HideInInspector] public BasePiece AttackerPiece;

		public bool HasFight = false;

		public void Setup(Vector2 pos)
		{
			Pos = pos;
			transform.localPosition = new Vector3(pos.x * GameEngine.POS_TO_COORDS, pos.y * GameEngine.POS_TO_COORDS, 0);
			if (Mathf.RoundToInt(pos.x + pos.y) % 2 == 0)
			{
				Bg = _whiteCell;
			}
			else
			{
				Bg = _blackCell;
			}
		}

		
		private void _renderBg()
		{
			if(_renderer==null) _renderer = GetComponent<SpriteRenderer>();
			_renderer.sprite = _bg;
		}


		
		public void Destructor()
		{
			if (Piece)
			{
				Destroy(Piece.gameObject);
			}

			Piece = null;
		}
		
		
	}
	
}
