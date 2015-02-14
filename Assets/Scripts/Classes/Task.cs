using UnityEngine;
using System.Collections;

public class Task {

	public string text;
	public int selectedColor;
	public bool deleted = false;

	public Task(string str, int selected) {
		text = str;
		selectedColor = selected;
	}
}
