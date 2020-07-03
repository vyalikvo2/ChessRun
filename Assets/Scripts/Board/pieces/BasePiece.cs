using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

using ChessRun.Board.Controllers;
using ChessRun.GUI;

namespace ChessRun.Board.Pieces
{
	public class BasePiece : MonoBehaviour
	{
		public const string BASE_PREFAB = "prefabs/BasePiece";

		private const string PREFIX = "images/pieces/";

		// pieces
		public const string KING_WHITE = PREFIX + "king_white";
		public const string KING_BLACK = PREFIX + "king_black";
		public const string KING_HORSE_WHITE = PREFIX + "king_horse_white";
		public const string PAWN_WHITE = PREFIX + "pawn_white";
		public const string PAWN_BLACK = PREFIX + "pawn_black";
		public const string HORSE_WHITE = PREFIX + "horse_white";
		public const string HORSE_BLACK = PREFIX + "horse_black";
		public const string BISHOP_WHITE = PREFIX + "bishop_white";
		public const string BISHOP_BLACK = PREFIX + "bishop_black";
		public const string QUEEN_WHITE = PREFIX + "queen_white";
		public const string QUEEN_BLACK = PREFIX + "queen_black";
		public const string ROOK_WHITE = PREFIX + "rook_white";

		public const string ROOK_BLACK = PREFIX + "rook_black";

		// buildings
		public const string BUILDING_HOME_CASTLE = PREFIX + "home_castle";

		private const string CELL_PREFIX = "images/board/";

		// cells
		public const string H_CELL_ENEMY = CELL_PREFIX + "cell_enemy";
		public const string H_CELL_HIGHLIGHTED = CELL_PREFIX + "cell_highlighted";
		public const string H_CELL_MOVE_TO = CELL_PREFIX + "cell_moveto";
		public const string H_CELL_BLACK = CELL_PREFIX + "cell_bg_black";
		public const string H_CELL_WHITE = CELL_PREFIX + "cell_bg_white";
		public const string H_CELL_CURRENT = CELL_PREFIX + "cell_curposition";

		public int Relation = PieceRelation.SELF;
		public char Type = TypePiece.NONE;

		public bool CanJump = false;

		private Vector2 _pos;

		public Vector2 pos
		{
			get { return _pos; }
			set
			{
				_pos = value;
				RefreshPos();
			}
		}

		private string _sprite;

		public string sprite
		{
			get { return _sprite; }
			set
			{
				if (_sprite == value) return;
				_sprite = value;
				currentSprite = ResourceCache.getSprite(_sprite);
			}
		}

		public GameObject SpriteObj;
		private Sprite _currentSprite;

		[HideInInspector]
		public Sprite currentSprite
		{
			get { return _currentSprite; }
			set
			{
				if (!SpriteObj)
				{
					if (transform.childCount > 0)
						SpriteObj = transform.GetChild(0).gameObject;
				}

				if (SpriteObj.GetComponent<SpriteRenderer>().sprite != value)
				{
					SpriteObj.GetComponent<SpriteRenderer>().sprite = value;
					_currentSprite = value;
				}
			}
		}

		public List<Vector2> Moves;
		public List<Vector2> MovesAttack;
		public List<InteractiveMove> InteractiveMoves;

		public PieceStats Stats;
		public FightCellStatus FightStatus = FightCellStatus.NORMAL;

		
		
		private SpriteRenderer _spriteRenderer;
		private int _zIndex = 0;
		public int zIndex
		{
			get { return _zIndex; }
			set
			{
				_zIndex = value;
				if (SpriteObj)
				{
					if (_spriteRenderer == null) _spriteRenderer = SpriteObj.GetComponent<SpriteRenderer>();
					_spriteRenderer.sortingOrder = _zIndex;
				}
				
				if (Stats)
					Stats.zIndex = _zIndex;
			}
		}

		private bool _inited = false;

		public virtual void Setup(Vector2 pos)
		{
			if (_inited) return;
			_inited = true;

			SetBoardPosition(pos);

			zIndex = ZIndex.PIECES_MY;
		}

		protected void CreateStats()
		{
			GameObject statsPrefab = ResourceCache.getPrefab("prefabs/PieceStats");
			GameObject statsObj = Instantiate(statsPrefab, gameObject.transform) as GameObject;
			Stats = statsObj.GetComponent<PieceStats>();
			Stats.transform.localPosition = new Vector3(0, 0, 0);
		}

		// recalculate setter (cause of mouse dragging)
		protected virtual void RefreshPos()
		{
			transform.localPosition = new Vector3(pos.x * GameEngine.POS_TO_COORDS, pos.y * GameEngine.POS_TO_COORDS, 0);
		}

		public bool HasKing()
		{
			return Type == TypePiece.KING || Type == TypePiece.KING_HORSE;
		}

		public virtual void SetBoardPosition(Vector2 pos)
		{
			this.pos = pos;
		}

		public virtual void AnimateScale(float from, float to)
		{
			transform.localScale = Vector3.one * from;
			transform.DOScale(Vector3.one * to, 1f);
		}

		public virtual void KilledAnimation(int direction, float delay, TweenCallback onPlayed)
		{
			SpriteRenderer spriteRenderer = SpriteObj.GetComponent<SpriteRenderer>();
			spriteRenderer.transform
				.DOLocalMove(spriteRenderer.transform.localPosition + new Vector3(0.4f, 0.1f, 0) * direction, 0.2f)
				.SetDelay(delay);
			spriteRenderer.transform.DOLocalRotate(new Vector3(0, 0, -90f) * direction, 0.2f).SetDelay(delay);
			transform.DOScale(new Vector3(0, 0, 0), 0.4f).SetDelay(delay + 0.2f).OnComplete(onPlayed);
		}

		public void SetFightStatus(FightCellStatus fightCellStatus, float time = 0.2f)
		{
			Vector3 statusPosition = Vector3.zero;
			this.FightStatus = fightCellStatus;
			if (fightCellStatus == FightCellStatus.DEFENDER)
				statusPosition = new Vector3(1f, 0, 0);

			Stats.transform.DOLocalMove(statusPosition, time);
		}

		// need to be overrided ( returns interactiontype )
		public virtual bool IsInteractableWith(BasePiece piece)
		{
			return false;
		}

		public virtual void Destructor()
		{
			// override it
		}

	}
}
