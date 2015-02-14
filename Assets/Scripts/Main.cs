using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour {

	private string path;

	private Texture background;

	private TopMenu topMenu;
	private float topMenuFixedHeight = 64.0f;
	private float topMenuButtonOffset = 16.0f;

	private Dialog addDialog;
	private string newTask = "";
	private float taskScrollPosition;
	private float taskFixedHeight = 128.0f;
	private bool isCorrectingTaskScroll = false;
	private Texture checkButtonTexture;

	private int buttonSelected = -1;

	private GUIStyle buttonStyle = new GUIStyle();

	private List<Task> tasks = new List<Task>();
	private List<int> deletedTasks = new List<int>();
	private List<Texture> taskColors = new List<Texture>();

	private float fingerLastPos;
	private float fingerCurPos;

	private Toast toast;

	// Use this for initialization
	void Start () {

		path = Application.persistentDataPath + "/ToDo.txt";

		background = Resources.Load ("Textures/Background") as Texture;
		toast = new Toast(Resources.Load ("Textures/Toast/Background") as Texture);
		CreateTopMenu();
		taskScrollPosition = topMenu.height + 2.0f;
		CreateAddDialog();
		PopulateColorList();
		PopulateTaskList();

		checkButtonTexture = Resources.Load("Textures/Task/CheckBox") as Texture;
	}

	void CreateTopMenu() {
		topMenu = new TopMenu(Screen.width, topMenuFixedHeight, Resources.Load ("Textures/TopMenu/Background") as Texture);
		topMenu.buttons.Add(new Button(topMenu.height, topMenu.height, Resources.Load ("Textures/TopMenu/Buttons/Options") as Texture));
		topMenu.buttons.Add(new Button(topMenu.height, topMenu.height, Resources.Load ("Textures/TopMenu/Buttons/Add") as Texture));
		topMenu.buttons.Add(new Button(topMenu.height, topMenu.height, Resources.Load ("Textures/TopMenu/Buttons/UndoDisabled") as Texture));
	}

	void CreateAddDialog() {
		addDialog = new Dialog(new Vector2(Screen.width/6.0f, Screen.height/3.0f), Screen.width/1.5f, Screen.height/3.0f, Resources.Load("Textures/Dialog/Background") as Texture);
		addDialog.buttons.Add(new Button(addDialog.width/4.0f, addDialog.height/8.0f, Resources.Load ("Textures/Dialog/Buttons/Background") as Texture));
	}

	void PopulateColorList() {
		taskColors.Add(Resources.Load ("Textures/Task/Orange") as Texture);
		taskColors.Add(Resources.Load ("Textures/Task/Blue") as Texture);
		taskColors.Add(Resources.Load ("Textures/Task/Blue1") as Texture);
		taskColors.Add(Resources.Load ("Textures/Task/Blue2") as Texture);
		taskColors.Add(Resources.Load ("Textures/Task/Pink") as Texture);
		taskColors.Add(Resources.Load ("Textures/Task/Pink1") as Texture);
		taskColors.Add(Resources.Load ("Textures/Task/Green") as Texture);
		taskColors.Add(Resources.Load ("Textures/Task/Green1") as Texture);
		taskColors.Add(Resources.Load ("Textures/Task/Red") as Texture);
		taskColors.Add(Resources.Load ("Textures/Task/Yellow") as Texture);
	}

	void PopulateTaskList () {
		if (File.Exists(path)) {
			string[] fileLines = File.ReadAllLines(path);
			List<string> newFileLines = new List<string>();

			for (int i = 0; i < fileLines.Length; i++) {
				if (fileLines[i] != "\n" && fileLines[i] != "") {
					CreateTask(fileLines[i]);
					newFileLines.Add(fileLines[i]);
				} else {
					fileLines[i] = "";
				}
			}
			File.WriteAllLines(path, newFileLines.ToArray());
		} else {
			File.WriteAllText(path, "");
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.touchCount == 1) {

			Touch touch = Input.GetTouch(0);

			switch (touch.phase) {

				case TouchPhase.Began:
				fingerLastPos = touch.position.y;
				break;
				
				case TouchPhase.Moved:
				fingerCurPos = touch.position.y;
				taskScrollPosition += fingerLastPos - fingerCurPos;
				fingerLastPos = fingerCurPos;
				break;

				case TouchPhase.Ended:
				isCorrectingTaskScroll = true;
				break;
			}
		}
	}

	void LateUpdate() {
		if (isCorrectingTaskScroll) {
			CorrectTaskScrollPosition();
		}
	}

	void OnGUI() {

		//draw the background
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), background, ScaleMode.StretchToFill);

		//draw the task list
		GUI.skin.label.normal.textColor = new Color(0.83f, 0.83f, 0.83f, 1.0f);
		GUI.skin.label.hover.textColor = new Color(0.83f, 0.83f, 0.83f, 1.0f);
		GUI.skin.label.active.textColor = new Color(0.83f, 0.83f, 0.83f, 1.0f);
		float fontSize = taskFixedHeight/3.0f;
		GUI.skin.label.fontSize = (int)fontSize;

		float curPos = taskScrollPosition;
		float xOffset = 32.0f;

		for (int j = 0; j < tasks.Count; j++) {
			if (tasks[j].deleted) {
				//do nothing
			} else {
				if (curPos > -taskFixedHeight && curPos <= Screen.height) {
					GUI.DrawTexture(new Rect(0, curPos, Screen.width, taskFixedHeight), taskColors[tasks[j].selectedColor], ScaleMode.StretchToFill);
					Rect checkPos = new Rect(Screen.width - xOffset - topMenuFixedHeight, curPos + taskFixedHeight/4.0f, topMenuFixedHeight, topMenuFixedHeight);
					GUI.Label(new Rect(xOffset, curPos + taskFixedHeight/4.0f, Screen.width - 3.0f*xOffset - topMenuFixedHeight, 1.4f*fontSize), tasks[j].text);

					GUI.skin.button.normal.background = checkButtonTexture as Texture2D;
					GUI.skin.button.hover.background = checkButtonTexture as Texture2D;
					GUI.skin.button.active.background = checkButtonTexture as Texture2D;

					if (curPos > topMenuFixedHeight) {
						if (GUI.Button(checkPos, "")) {
							DeleteTask(j);
						}
					}
				}
				curPos += taskFixedHeight;
			}
		}

		//draw the top menu bar
		GUI.DrawTexture(new Rect(0, 0, topMenu.width, topMenu.height), topMenu.background, ScaleMode.StretchToFill);
		for (int i = 0; i < topMenu.buttons.Count; i++) {
			if (GUI.Button(new Rect(getButtonPosition(topMenu.buttons, i), 0, topMenu.buttons[i].width, topMenu.buttons[i].height), topMenu.buttons[i].background, buttonStyle)) {
				buttonSelected = i;
			}
		}

		switch (buttonSelected) {
			
		case 0:			//"Options" button pressed
			/*TODO Show "Options" dialog*/
			break;
			
		case 1:			//"Add" button pressed

			GUI.skin.button.normal.background = addDialog.buttons[0].background as Texture2D;
			GUI.skin.button.hover.background = addDialog.buttons[0].background as Texture2D;
			GUI.skin.button.active.background = addDialog.buttons[0].background as Texture2D;

			GUI.DrawTexture(addDialog.rect, addDialog.background, ScaleMode.StretchToFill);
			float textAreaOffset = addDialog.width/20.0f;

			GUI.skin.textArea.fontSize = (int)fontSize;
			GUI.skin.button.fontSize = (int)(fontSize/1.5f);

			newTask = GUI.TextArea(new Rect(addDialog.position.x + textAreaOffset, addDialog.position.y + textAreaOffset, addDialog.width - 2.0f*textAreaOffset, addDialog.height/1.5f), newTask);
			if (GUI.Button(new Rect(addDialog.position.x + addDialog.width/3.0f - addDialog.buttons[0].width/2.0f, addDialog.position.y + 7.0f*addDialog.height/8.0f - addDialog.buttons[0].height/2.0f, addDialog.buttons[0].width, addDialog.buttons[0].height), "Add") && newTask.Length > 0) {
				string tempTask = "";
				for (int h = 0; h < newTask.Length; h++) {
					if (newTask[h] == '\n') {
						CreateTask(tempTask);
						tempTask = "";
					} else {
						tempTask += newTask[h];
					}
				}

				if (tempTask.Length > 0) {
					CreateTask(tempTask);
					tempTask = "";
				}

				newTask = "";
				buttonSelected = -1;
			}
			if (GUI.Button(new Rect(addDialog.position.x + 2.0f*addDialog.width/3.0f - addDialog.buttons[0].width/2.0f, addDialog.position.y + 7.0f*addDialog.height/8.0f - addDialog.buttons[0].height/2.0f, addDialog.buttons[0].width, addDialog.buttons[0].height), "Cancel")) {
				newTask = "";
				buttonSelected = -1;
			}
			break;
		case 2:			//"Undo" was pressed
			if (deletedTasks.Count > 0) {
				RestoreTask();
			}
			buttonSelected = -1;
			break;
		default:
			break;
		}

		//Draw the toast
		if (toast.timer <= 3.0f) {
			float yOffset = topMenuFixedHeight/8.0f;
			xOffset = yOffset;
			Rect toastRect = new Rect(xOffset, yOffset, fontSize*toast.text.Length/2.0f, topMenuFixedHeight - 2.0f*yOffset);
			GUI.DrawTexture(toastRect, toast.background, ScaleMode.StretchToFill);
			GUI.Label(toastRect, toast.text);
			toast.timer += Time.deltaTime;
		}
		
	}

	void CreateToast(string message) {
		toast.text = message;
		toast.timer = 0.0f;
	}

	void CreateTask(string newTask) {
		int selectedColor = Random.Range(0, taskColors.Count-1);
		tasks.Add(new Task(newTask, selectedColor));

		File.AppendAllText(path, newTask+"\n");

		CreateToast("Task created");
	}

	void DeleteTask(int index) {
		tasks[index].deleted = true;
		deletedTasks.Add(index);

		string[] fileLines = File.ReadAllLines(path);
		fileLines[index] = "";
		File.WriteAllLines(path, fileLines);

		//toggle the "undo" texture
		if (deletedTasks.Count == 1) {
			topMenu.buttons[2].background = Resources.Load ("Textures/TopMenu/Buttons/UndoEnabled") as Texture;
		}
		//------------------------

		CreateToast("Task deleted");
	}

	void RestoreTask() {
		int restoreIndex = deletedTasks[deletedTasks.Count-1];
		tasks[restoreIndex].deleted = false;
		deletedTasks.RemoveAt(deletedTasks.Count-1);

		string[] fileLines = File.ReadAllLines(path);
		fileLines[restoreIndex] = tasks[restoreIndex].text+"\n";
		File.WriteAllLines(path, fileLines);

		//toggle the "undo" texture
		if (deletedTasks.Count == 0) {
			topMenu.buttons[2].background = Resources.Load ("Textures/TopMenu/Buttons/UndoDisabled") as Texture;
		}
		//------------------------

		CreateToast("Task restored");
	}

	float getButtonPosition(List<Button> buttons, int i) {

		float position = Screen.width;

		for (int j = 0; j <= i; j++) {
			position -= buttons[j].width + topMenuButtonOffset;
		}

		return position;
	}

	void CorrectTaskScrollPosition() {

		float fixSpeed = 10.0f;

		if (taskScrollPosition > topMenuFixedHeight) {
			taskScrollPosition -= (Mathf.Abs(topMenuFixedHeight - taskScrollPosition)) * Time.deltaTime*fixSpeed;
			isCorrectingTaskScroll = IsCorrected(0);
		} else if (taskScrollPosition + (tasks.Count - deletedTasks.Count)*taskFixedHeight < Screen.height) {
			taskScrollPosition += (Mathf.Abs(Screen.height - (taskScrollPosition + (tasks.Count - deletedTasks.Count)*taskFixedHeight))) * Time.deltaTime*fixSpeed;
			isCorrectingTaskScroll = IsCorrected(1);
		}


	}

	bool IsCorrected(int opt) {

		float errorMargin = 5.0f;

		if (opt == 0 && Mathf.Abs(taskScrollPosition - topMenuFixedHeight) < errorMargin) {
			return false;
		} else if (opt == 1 && Mathf.Abs(Screen.height - (taskScrollPosition + (tasks.Count - deletedTasks.Count)*taskFixedHeight)) < errorMargin) {
			return false;
		} else {
			return true;
		}
	}
}
