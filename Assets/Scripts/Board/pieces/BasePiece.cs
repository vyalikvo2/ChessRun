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

		public int relation = Relation.SELF;
		public char type = TypePiece.NONE;

		public bool canJump = false;

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

		public GameObject spriteObj;
		private Sprite _currentSprite;

		[HideInInspector]
		public Sprite currentSprite
		{
			get { return _currentSprite; }
			set
			{
				if (!spriteObj)
				{
					if (transform.childCount > 0)
						spriteObj = transform.GetChild(0).gameObject;
				}

				if (spriteObj.GetComponent<SpriteRenderer>().sprite != value)
				{
					spriteObj.GetComponent<SpriteRenderer>().sprite = value;
					_currentSprite = value;
				}
			}
		}

		public List<Vector2> moves;
		public List<Vector2> movesAttack;
		public List<InteractiveMove> interactiveMoves;

		public PieceStats stats;
		public FightCellStatus fightCellStatus = FightCellStatus.NORMAL;

		private int _zIndex = 0;

		public int zIndex
		{
			get { return _zIndex; }
			set
			{
				_zIndex = value;
				if (spriteObj)
					spriteObj.GetComponent<SpriteRenderer>().sortingOrder = _zIndex;
				if (stats)
					stats.zIndex = _zIndex;
			}
		}

		private bool _inited = false;

		public virtual void Setup(Vector2 pos)
		{
			if (_inited) return;
			_inited = true;

			setBoardPosition(pos);

			zIndex = ZIndex.PIECES_MY;
		}

		public void createStats()
		{
			GameObject statsPrefab = ResourceCache.getPrefab("prefabs/PieceStats");
			GameObject statsObj = Instantiate(statsPrefab, gameObject.transform) as GameObject;
			stats = statsObj.GetComponent<PieceStats>();
			stats.transform.localPosition = new Vector3(0, 0, 0);
		}

		// recalculate setter (cause of mouse dragging)
		public virtual void RefreshPos()
		{
			transform.localPosition = new Vector3(pos.x * Game.POS_TO_COORDS, pos.y * Game.POS_TO_COORDS, 0);
		}

		public bool hasKing()
		{
			return type == TypePiece.KING || type == TypePiece.KING_HORSE;
		}

		public virtual void setBoardPosition(Vector2 pos)
		{
			this.pos = pos;
		}

		public virtual void animateScale(float from, float to)
		{
			transform.localScale = Vector3.one * from;
			transform.DOScale(Vector3.one * to, 1f);
		}

		public virtual void killedAnimation(int direction, float delay, TweenCallback onPlayed)
		{
			SpriteRenderer spriteRenderer = spriteObj.GetComponent<SpriteRenderer>();
			spriteRenderer.transform
				.DOLocalMove(spriteRenderer.transform.localPosition + new Vector3(0.4f, 0.1f, 0) * direction, 0.2f)
				.SetDelay(delay);
			spriteRenderer.transform.DOLocalRotate(new Vector3(0, 0, -90f) * direction, 0.2f).SetDelay(delay);
			transform.DOScale(new Vector3(0, 0, 0), 0.4f).SetDelay(delay + 0.2f).OnComplete(onPlayed);
		}

		public void setFightStatus(FightCellStatus fightCellStatus, float time = 0.2f)
		{
			Vector3 statusPosition = Vector3.zero;
			this.fightCellStatus = fightCellStatus;
			if (fightCellStatus == FightCellStatus.DEFENDER)
				statusPosition = new Vector3(1f, 0, 0);

			stats.transform.DOLocalMove(statusPosition, time);
		}

		// need to be overrided ( returns interactiontype )
		public virtual bool isInteractableWith(BasePiece piece)
		{
			return false;
		}

		public void Destructor()
		{
		}

	}
}
