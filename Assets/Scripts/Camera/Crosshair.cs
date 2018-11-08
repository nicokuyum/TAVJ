using UnityEngine; using System.Collections;

public class Crosshair : MonoBehaviour {

	public Texture2D crosshair;

	public float scale;
	// Use this for initialization
	void Start () {
     
	}
     
	// Update is called once per frame
	void Update () {
	}
	
	void OnGUI() {
		GUI.DrawTexture (Rect.zero, crosshair);
	}

} 