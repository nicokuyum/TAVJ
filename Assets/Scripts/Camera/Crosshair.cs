using UnityEngine; using System.Collections;

public class Crosshair : MonoBehaviour {

	public Texture2D crosshair;
	public Rect position;

	public float scale;
	// Use this for initialization
	void Start () {
     
	}
     
	// Update is called once per frame
	void Update () {
		position = new Rect((Screen.width - crosshair.width) / 2, (Screen.height - crosshair.height) /2, crosshair.width * scale, crosshair.height);
	}
	
	void OnGUI() {
		//GUI.DrawTexture (position, crosshair);
	}

} 