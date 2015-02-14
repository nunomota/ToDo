using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Dialog {

	public Vector2 position;
	public float width;
	public float height;
	public Rect rect;
	public Texture background;
	public List<Button> buttons = new List<Button>();

	public Dialog(Vector2 pos, float w, float h, Texture bg) {
		position = pos;
		width = w;
		height = h;
		background = bg;
		rect = new Rect(position.x, position.y, width, height);
	}

}
