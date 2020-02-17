using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class BasePiece : MonoBehaviour
{
	public int relation = Relation.SELF;
	public char type	= TypePiece.NONE;
	
	private Vector2 _pos;
	public Vector2 pos {
		get { return _pos; }
		set { _pos = value; RefreshPos();}
	}

	public GameObject spriteObj;
	private Sprite _currentSprite;
	[HideInInspector] public Sprite currentSprite { 
		get { return _currentSprite; } 
		set
		{
			if (!spriteObj) return;
			if((spriteObj.GetComponent<SpriteRenderer> () as SpriteRenderer).sprite != value){
				(spriteObj.GetComponent<SpriteRenderer> () as SpriteRenderer).sprite = value;
				_currentSprite = value;
			}
		}
	}

	public List<Vector2> moves;
	public List<Vector2> movesAttack;
	public List<InteractiveMove> interactiveMoves;

	public PieceStats stats;
	public int fightStatus = FightController.FIGHT_STATUS_NORMAL;
	
	private int _zIndex = 0;

	public int zIndex
	{
		get { return _zIndex; } 
		set { 
			_zIndex = value; 
			if(spriteObj)
				spriteObj.GetComponent<SpriteRenderer>().sortingOrder = _zIndex;
			if(stats)
				stats.zIndex = _zIndex;
		}
	}

	public virtual void Setup(Vector2 pos)
	{
		if(transform.childCount > 0)
			spriteObj = transform.GetChild(0).gameObject;
		
		setBoardPosition(pos);
		
		zIndex = ZIndex.PIECES_MY;
	} 
	public void createStats()
	{
		GameObject statsPrefab = Resources.Load("prefabs/PieceStats") as GameObject;
		GameObject statsObj = Instantiate(statsPrefab, gameObject.transform) as GameObject;
		stats = statsObj.GetComponent<PieceStats>();
		stats.transform.localPosition = new Vector3(0,0,0);
	}

	// recalculate setter (cause of mouse dragging)
	public virtual void RefreshPos()
	{
		transform.localPosition = new Vector3(pos.x * Game.POS_TO_COORDS, pos.y * Game.POS_TO_COORDS, 0);
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
		spriteRenderer.transform.DOLocalMove(spriteRenderer.transform.localPosition + new Vector3(0.4f,0.1f,0) * direction, 0.2f)
			.SetDelay(delay);
		spriteRenderer.transform.DOLocalRotate(new Vector3(0,0, -90f)*direction, 0.2f).SetDelay(delay);
		transform.DOScale(new Vector3(0,0, 0), 0.4f).SetDelay(delay + 0.2f).OnComplete(onPlayed);
	}

	public void setFightStatus(int fightStatus, float time = 0.2f)
	{
		Vector3 statusPosition = Vector3.zero;
		this.fightStatus = fightStatus;
		if (fightStatus == FightController.FIGHT_STATUS_DEFENDER)
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
