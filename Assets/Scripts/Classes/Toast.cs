using UnityEngine;
using System.Collections;

public class Toast {

	public float timer;
	public string text;
	public Texture background;

	public Toast(Texture bg) {
		timer = 4.0f;
		background = bg;
	}

}
