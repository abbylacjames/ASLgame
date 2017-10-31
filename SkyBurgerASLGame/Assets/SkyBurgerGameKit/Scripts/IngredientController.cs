using UnityEngine;
using System.Collections;

public class IngredientController : MonoBehaviour {

	/// <summary>
	/// This class moves ingredients to the bottom of the screen after creation.
	/// It also handles all the events that might occur to the object such as collision, 
	/// collection or flying away. When an ingredient is instantiated, it automatically 
	/// drops down from the top to the bottom of the screen. If player receives the 
	/// ingredient in the center of its base, the ingredient will stick to the base and 
	/// the ID of the ingredient will gets added to the main array. But if player fails 
	/// to collect the ingredient, it will fall away and get destroyed after exiting the view.
	/// </summary>

	public int ingredientID = 1;					//impotant. Unique ID of this ingredient. Set in prefab.
	private float hitDetectionPrecision = 0.35f;	//the smaller the number, the harder it is to catch the object
	private float tmpDistance;

	private GameObject baseObject;					//player base object (starting ingredient)
	private GameObject attachedObject;				//the object this ingredient has been attached to.
	private GameObject gc;							//main game controller

	private GameObject background;					//background object (we need its component)

	public float speed = 3.0f;						//movement speed (the faster, the harder)
	private float destroyThreshold = -6.0f;			//if position is passed this value, the game is over.

	private bool isAtachedToBase;					//if player catched this ingredient

	public AudioClip catchSfx;
	public AudioClip flyawaySfx;
	
	void Awake() {
		tmpDistance = 0;
		isAtachedToBase = false;
		attachedObject = null;

		baseObject = GameObject.FindGameObjectWithTag("PlayerBase");
		background = GameObject.FindGameObjectWithTag("Background");
		gc = GameObject.FindGameObjectWithTag("GameController");
	}

	void Update() {

		if(!isAtachedToBase) {
			//move the Ingredient down
			transform.position -= new Vector3(0, Time.deltaTime * ( speed + GameController.moveSpeed ), 0);

			if(GameController.gameIsFinished)
				GetComponent<BoxCollider>().enabled = false;	//not catch-able anymore

		} else {
			//slowly follow the attached ingredient 
			followWithDelay();
		}

		//if this ingredient went out of screen without getting catched, detroy it.
		if (transform.position.y < destroyThreshold && !isAtachedToBase) {
			//GameController.gameOver = true;
			Destroy(gameObject);
		}
	}


	private float sx, sy;
	private float xv, yv = 0.0f;
	void followWithDelay() {
		sx = Mathf.SmoothDamp(transform.position.x, attachedObject.transform.position.x, ref xv, 0.035f);
		sy = Mathf.SmoothDamp(transform.position.y, attachedObject.transform.position.y + 0.2f, ref yv, 0.035f);
		transform.position = new Vector3(sx, sy, transform.position.z);
	}


	///***********************************************************************
	/// Collision detection and management
	///***********************************************************************
	void OnCollisionEnter(Collision other) {

		if(isAtachedToBase)
			return;

		if(other.gameObject.tag == "PlayerBase" || other.gameObject.tag == "Ingredient") {

			//print ("hit with base");
			playSfx(catchSfx);

			//check if the hit was near the center of the base, otherwise it was a lose hit
			if(checkHit(other.gameObject)) {

				if(!isAtachedToBase) {
					GameController.catchedIngredients.Add(ingredientID);	//add this ingredient ID to main array
					gc.GetComponent<GameController>().updateMissionUI(ingredientID);
					gc.GetComponent<GameController>().calculateRealtimeMoneyMade(ingredientID);
				}

				isAtachedToBase = true;

				//not movable anymore
				GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | 
						RigidbodyConstraints.FreezePositionY |
						RigidbodyConstraints.FreezePositionZ |
						RigidbodyConstraints.FreezeRotationX |
						RigidbodyConstraints.FreezeRotationY |
						RigidbodyConstraints.FreezeRotationZ;

				attachedObject = other.gameObject;
				//print ("I'm actually attached to: " + other.gameObject.name);

				//move the main background down by each catch
				StartCoroutine(background.GetComponent<PositionFixer>().setNewBgPosition());

				//move the base collector down after 8 catch
				if(GameController.catchedIngredients.Count >= 8)
					StartCoroutine(baseObject.GetComponent<PositionFixer>().setNewBgPosition());

				//check if this ingredient was a finisher (end ingredient with unique ID of 0)
				if(ingredientID == 0) {
					GameController.gameIsFinished = true;
				}

			} else {
				//do not catch this ingredient
				GetComponent<BoxCollider>().enabled = false;

				if(tmpDistance >= 0)
					transform.rotation = Quaternion.Euler(0, 180, -35);
				else
					transform.rotation = Quaternion.Euler(0, 180, 35);

				//print ("Get out of screen view and destroy.");
				playSfx(flyawaySfx);
			}
		
		}
	}

	
	/// <summary>
	/// check if this ingredient has collided with a base object.
	/// if hit is true, this object will follow the attached object from now on.
	/// </summary>
	bool checkHit (GameObject go) {
		//check the distance of the centers of the two colliding object.
		tmpDistance = go.transform.position.x - gameObject.transform.position.x;
		if(Mathf.Abs(tmpDistance) <= hitDetectionPrecision) {
			return true;
		} else {
			return false;
		}
	}


	void playSfx (AudioClip _sfx){
		GetComponent<AudioSource>().clip = _sfx;
		if(!GetComponent<AudioSource>().isPlaying) {
			GetComponent<AudioSource>().Play();
		}
	}
}
