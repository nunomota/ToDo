using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TopMenu {

	public Texture background;
	public List<Button> buttons = new List<Button>();
	public float width;
	public float height;

	public TopMenu(float w, float h, Texture bg) {
		width = w;
		height = h;
		background = bg;
	}
}
