using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	/// <summary>
	/// This class is attached to playerBase game object which plays the role of starting ingredient.
	/// This object will always follows player mouse/touch/input position on the screen in X direction.
	/// You can set how fast/slow this object should follow the input position by tweaking the "followSpeedDelay".
	/// </summary>

	[Range(0, 0.5f)]
	public float followSpeedDelay = 0.1f;		//we want the base to follow player's input with a small delay.
	//delay of 0 leads to immediate follow and delay of 0.5 leads to a lazy follow

	private float sideLimit = 2.5f;
	private Vector3 startingPosition;

	private float xVelocity = 0.0f;
	//private float yVelocity = 0.0f;
	private Vector3 screenToWorldVector;

	void Start () {
		startingPosition = transform.position;
	}
	
	void Update () {
		followPlayerInput();
	}


	///***********************************************************************
	/// Control ball's position with touch position parameters
	///***********************************************************************
	private float touchX = 0;
	//private float touchY = 0;
	void followPlayerInput() {
		if (Input.touchCount > 0 || Application.isEditor) {
			screenToWorldVector = Camera.main.ScreenToWorldPoint(new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10));
			touchX = Mathf.SmoothDamp(transform.position.x, screenToWorldVector.x, ref xVelocity, followSpeedDelay);
			//touchY = Mathf.SmoothDamp(transform.position.y, screenToWorldVector.y, ref yVelocity, followSpeedDelay);

			//movement limiters
			if(touchX > sideLimit)
				touchX = sideLimit;
			if(touchX < -sideLimit)
				touchX = -sideLimit;

			transform.position = new Vector3(touchX, startingPosition.y, 0);
		}
	}
}
