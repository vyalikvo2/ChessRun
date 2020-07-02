using UnityEngine;
using UnityEngine.UI;


namespace ChessRun.GUI
{

	public class PieceStats : MonoBehaviour
	{

		[SerializeField] public Text healthText;
		[SerializeField] public Text attackText;
		[SerializeField] public GameObject canvasObj;

		private int _health = 0;

		public int health
		{
			get { return _health; }
			set
			{
				healthText.text = value + "";
				_health = value;
			}
		}

		private bool _visible = true;

		public bool visible
		{
			get { return _visible; }
			set
			{
				_visible = value;
				canvasObj.GetComponent<Canvas>().enabled = _visible;
			}
		}

		private int _zIndex = 0;

		public int zIndex
		{
			get { return _zIndex; }
			set
			{
				_zIndex = value;
				canvasObj.GetComponent<Canvas>().sortingOrder = _zIndex;
			}
		}

		private int _attack = 0;

		public int attack
		{
			get { return _attack; }
			set
			{
				attackText.text = value + "";
				_attack = value;
			}
		}
	}

}
