using UnityEngine;
using System.Collections;

public class PositionFixer : MonoBehaviour {

	/// <summary>
	/// This class moves the main game background down to simulate the rise effect. 
	/// With each new ingredients the player catches, this class will move the background a little bit down.
	/// </summary>

	private float bgStartPos;
	private float bgCurrentPos;
	private float bgTargetPos;

	void Start () {
		bgStartPos = transform.position.y;
		bgCurrentPos = bgStartPos;
	}


	//gets called from other classes.
	public IEnumerator setNewBgPosition() {

		bgTargetPos = bgCurrentPos - 0.2f;

		float t = 0;
		while(t < 1) {
			t += Time.deltaTime * 2.5f;
			transform.position = new Vector3(transform.position.x, 
                                    	     Mathf.SmoothStep(bgCurrentPos, bgTargetPos, t),
                                       	     transform.position.z);
			yield return 0;
		}

		if(t >= 1) {
			bgCurrentPos = transform.position.y;
		}

	}
}
