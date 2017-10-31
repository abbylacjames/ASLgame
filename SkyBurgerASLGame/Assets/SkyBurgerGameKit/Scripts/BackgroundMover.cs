using UnityEngine;
using System.Collections;

public class BackgroundMover : MonoBehaviour {

	/// <summary>
	/// This class moves the menu background up and down.
	/// </summary>

	private bool canMove;
	private float speed;

	void Start () {
		canMove = true;
		speed = 0.09f;
	}
	
	void Update () {
		if(canMove)
			StartCoroutine(AnimateMenuBG());
	}

	IEnumerator AnimateMenuBG() {
		canMove = false;
		float t = 0; 
		while (t <= 1.0f) {
			t += Time.deltaTime * speed;
			transform.position = new Vector3(transform.position.x, Mathf.SmoothStep(4, -4, t), transform.position.z);
			yield return 0;
		}
		
		float r = 0; 
		if(t >= 1) {
			while (r <= 1.0f) {
				r += Time.deltaTime * speed;
				transform.position = new Vector3(transform.position.x, Mathf.SmoothStep(-4, 4, r), transform.position.z);
				yield return 0;
			}
		}
		
		if(r >= 1)
			canMove = true;
	}
}
