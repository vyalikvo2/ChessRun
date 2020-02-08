using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{	
	[SerializeField]
    public Sprite whiteCell;
	public Sprite blackCell;

	[HideInInspector]
    public Vector2 pos = Vector2.zero;

    [HideInInspector]
	private float posMultiplier = 0f;

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

	private void renderBg()
	{
		SpriteRenderer renderer = GetComponent<SpriteRenderer> () as SpriteRenderer;
		renderer.sprite = _bg;
	}


	public void Setup(Vector2 pos)
    {
		this.posMultiplier = Game.CELL_SIZE * Game.TO_UNITS;
		this.pos = pos;
		this.transform.position = new Vector3 (pos.x * posMultiplier, pos.y * posMultiplier, 0);
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
