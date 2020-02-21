using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{	
	[SerializeField]
    public Sprite whiteCell;
	public Sprite blackCell;

	[HideInInspector]
    public Vector2 pos = Vector2.zero;
    
	private Sprite _bg = null;
	public Sprite bg { 
		get { return _bg ; }
		set {
			_bg = value;
			if (_bg != null)
				renderBg();
		}
	}

	[HideInInspector] public BasePiece piece;
	[HideInInspector] public BasePiece attackerPiece;

	public bool hasFight = false;
	
	private void renderBg()
	{
		SpriteRenderer renderer = GetComponent<SpriteRenderer> () as SpriteRenderer;
		renderer.sprite = _bg;
	}


	public void Setup(Vector2 pos)
    {
	    this.pos = pos;
		this.transform.localPosition = new Vector3 (pos.x *  Game.POS_TO_COORDS, pos.y *  Game.POS_TO_COORDS, 0);
		if (Mathf.Round (pos.x + pos.y) % 2 == 0) 
		{
			bg = whiteCell;
		} 
		else 
		{
			bg = blackCell;
		}

    }

    public void RemovePiece()
    {

    }

	public void Destructor()
	{
		if (piece) {
			piece.gameObject.transform.SetParent (null);
			Destroy (piece.gameObject);
		}
		piece = null;

	}
}
