using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {
	
	private float _hp = 0;
	public float hp { get { return _hp; } set { _hp = value; RefreshUI ();}}
	private float _hpMax = 0;
	public float hpMax { get { return _hpMax; } set { _hpMax = value; RefreshUI ();}}

	private GameObject bar;
	private GameObject bar_bg;

	private float width = 0;
	private float height = 0;

	// Use this for initialization
	public void Setup (int w, int h, int hp, int hpMax) {
		width = w + 0.0f;
		height = h + 0.0f;
		_hp = hp + 0.0f;
		_hpMax = hpMax + 0.0f;

		bar = transform.FindChild("bar").gameObject;
		bar_bg = transform.FindChild("bar_bg").gameObject;

		RefreshUI ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	void RefreshUI()
	{
		Debug.Log ("refresh3");
		RectTransform rect = bar.GetComponent<RectTransform> () as RectTransform;
		//rect.localScale = new Vector3 (width*(hp/hpMax)/48, height/7, 1);

		rect.sizeDelta = new Vector3 (1, 1, 1);
		//bar.transform.localPosition = new Vector3 (bar.transform.localPosition.x-rect.localScale.x*48*Game.TO_UNITS/2, bar.transform.localPosition.y, bar.transform.localPosition.z);

		rect = bar_bg.GetComponent<RectTransform> () as RectTransform;
		rect.sizeDelta = new Vector3 (48, 7, 1);
		//bar_bg.transform.localPosition = new Vector3 (bar_bg.transform.localPosition.x-rect.localScale.x*48*Game.TO_UNITS/2, bar_bg.transform.localPosition.y, bar_bg.transform.localPosition.z);

	}
}
