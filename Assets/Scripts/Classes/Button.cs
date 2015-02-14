using UnityEngine;
using System.Collections;

public class Button {

	public float width;
	public float height;
	public Texture background;

	public Button(float w, float h, Texture bg) {
		width = w;
		height = h;
		background = bg;
	}
}
