using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	/// <summary>
	/// Main game controller class.
	/// This class monitors game status, creates different ingredients (from top),
	/// checks the main collected ingredients array and also loads the missions 
	/// of each level directly from missionManager class. 
	/// 
	/// Important. If the please beat a level, it will automatically advances to the next.
	/// But if the player lose a level, there is no retry. It will moves to the previous level and
	/// has to beat it again to unlock the next level.
	/// 
	/// If the player catch a final ingredient (top bread), the game is finished.
	/// </summary>

	public static bool gameIsFinished;				//global flag
	public static bool gameIsWon;					//final status of the game
	private bool showFinishPlaneFlag;				//show the final report

	public static int playerProgress;				//player level advancement.
	public static float moveSpeed = 0.1f;

	public Texture2D[] availableIngredientsImage;	//list of all available ingredient images
	public GameObject[] availableIngredients;		//list of all available ingredients
	public GameObject endIngredient;				//Catching this ingredient will finish the current game
	private float endIngredientChance = 0.25f;		//chance of creating a final ingredient (top bread)
	private float relatedIngredientChance = 0.3f;	//chance of creating an ingredient related to level missions
	public float ingredientCloneDelay = 2.0f;		//delay between cloning a new ingredients

	/// <summary>
	/// Mission related variables.
	/// We can have up to 6 missions for each level.
	/// These are the object we are using inside UI to show the mission ingredient images and 
	/// the amount of each ingredient player needs to collect.
	/// </summary>
	//***************************
	//caching the reference objects (Game UI)
	public GameObject[] Missions;				//we have a total of 6 missions in the game
	public GameObject[] MissionTargets;			//Text objects that show the requirement of each ingredients

	//caching the reference objects (Start plane UI)
	public GameObject[] MissionsSP;				
	public GameObject[] MissionTargetsSP;

	//caching the reference objects (Finish plane UI)
	public GameObject finishPlane;
	public GameObject levelStatusDynamic;
	public GameObject moneyDynamic;
	public GameObject tipDynamic;
	public GameObject totalDynamic;

	//reference to Money in UI
	public GameObject moneyMadeText;

	//Fetch and store mission date from MissionManager class
	private List<Vector2> missionData;
	//***************************

	private float realtimeMoneyMade;			//money made from collecting level objective ingredients
	private float ingredientValue = 0.6f;		//fixed temporary value for each correct ingredient

	public static List<int> catchedIngredients;		//Main array of all catched ingredients
	private List<int> correctCollectedIngredient;	//array of all correctly catched ingredients

	private bool createIngredient;			//flag
	private Vector3 startPoint;				//starting point of the clones object
	private float zModifier;				//we modify Z value of each new ingredient to avoid Z-fight bug

	//AudioClips
	public AudioClip winSfx;
	public AudioClip loseSfx;


	//init the game and fetch mission properties
	void Awake () {
		createIngredient = true;
		gameIsFinished = false;
		gameIsWon = false;
		showFinishPlaneFlag = false;
		startPoint = Vector3.zero;
		zModifier = -0.01f;

		catchedIngredients = new List<int>();
		correctCollectedIngredient = new List<int>();

		missionData = new List<Vector2>();
		realtimeMoneyMade = 0;

		if(finishPlane)
			finishPlane.SetActive(false);

		playerProgress = PlayerPrefs.GetInt("playerProgress", 1);
		//playerProgress = 7;

		//get mission data
		setMissionParameters(playerProgress);
	}


	/// <summary>
	/// Set mission data: The id and required amount of each ingredient
	/// </summary>
	void setMissionParameters (int level) {

		//deactive all mission objects in the UI
		for(int x = 0; x < 6; x++) {
			Missions[x].SetActive(false);
			MissionsSP[x].SetActive(false);
		}

		missionData = GetComponent<MissionManager>().getLevelMissionData(level);
		//print ("mission total ingredients: " + missionData.Count);

		//activate the missions set for this level
		for(int i = 0; i < missionData.Count; i++) {
			Missions[i].SetActive(true);
			MissionsSP[i].SetActive(true);

			//deduct 1 from missionData[i].x to match the mission array which starts from 0;
			Missions[i].GetComponent<Renderer>().material.mainTexture = availableIngredientsImage[(int)missionData[i].x - 1];
			MissionsSP[i].GetComponent<Renderer>().material.mainTexture = availableIngredientsImage[(int)missionData[i].x - 1];

			MissionTargets[i].GetComponent<TextMesh>().text = missionData[i].y.ToString();
			MissionTargetsSP[i].GetComponent<TextMesh>().text = "x " + missionData[i].y.ToString();
		}

		//set game difficulty by increasing the speed each time player advance
		moveSpeed  = playerProgress * 0.1f;
	}

	/// <summary>
	/// FSM
	/// </summary>
	void Update () {

		if(gameIsFinished) {
			//game is finished. Now we can show the finish plane.
			//this runs just once.
			showFinishPlane();
			return;
		}

		//clone new ingredient
		if(createIngredient) 
			cloneIngredient();

		//Debug
		UpdateCatchedIngredientsArray();
		
		//Debug
		if(Input.GetKeyDown(KeyCode.R))
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}


	/// <summary>
	/// Monitors game status and checked for level mission items.
	/// When player collects a new ingredient, this function is called to update the remaining required ingredients.
	/// if all ingredients and missions are passed, the game is won.
	/// </summary>
	public void updateMissionUI(int cID) {

		int passedMissions = 0;		//total number of passed missions
		for(int i = 0; i < missionData.Count; i++) {

			string output = "";		//used to show the remaining amount of each ingredient on the U
			int remaining = (int)missionData[i].y - getCollectedIng( (int)missionData[i].x );
			if(remaining <= 0) {
				output = "©";		//we use this character as a sign of checkmark/passed. You can change this at any time.
				passedMissions += 1;
			} else {
				output = remaining.ToString();
			}

			MissionTargets[i].GetComponent<TextMesh>().text = output;

			//check game finish condition
			if(passedMissions == missionData.Count) {
				//we have collected all the required ingredeints
				print ("All ingredients are collected...");
				gameIsWon = true;
			}
		}
	}


	/// <summary>
	/// Check conditions: The catched ingredient id should match one of the mission ingredients id
	/// </summary>
	bool isCorrectIngredient(int cID) {

		bool result = false;
		int isAvailable = 0;

		for(int i = 0; i < missionData.Count; i++) {
			if(missionData[i].x == cID) {
				isAvailable++;
			}
		}

		if(isAvailable > 0) {
			result = true;
			//print ("Ingredient \" " + cID + " \" is included in level missions");
		} 

		return result;
	}


	/// <summary>
	/// Check conditions: If we haven't collected enough of this ingredient id, 
	/// it is still required and must be collected.
	/// </summary>
	bool isRequired(int cID) {

		bool result = false;

		int availableAmount = getCollectedIng(cID);
		int targetAmount = 0;

		for(int i = 0; i < missionData.Count; i++) {
			if(missionData[i].x == cID) {
				//if this is the index we are looking for, check the amount
				targetAmount = (int)missionData[i].y;
				break;
			}
		}

		if(availableAmount <= targetAmount)
			result = true;
		
		//print ("Ingredient " + cID + " | availableAmount: " + availableAmount.ToString() + " | targetAmount: " + targetAmount.ToString() );
		return result;
	}


	/// <summary>
	/// get the total number of coorect ingredients collected by player and 
	/// times it with ingredient price to calculate total money made in this level.
	/// </summary>
	public void calculateRealtimeMoneyMade(int cID) {

		//if this is a correct ing, add it to list
		if(isCorrectIngredient(cID)) {
			if(isRequired(cID)) {
				correctCollectedIngredient.Add(cID);
			}
		}

		realtimeMoneyMade = correctCollectedIngredient.Count * ingredientValue;
		moneyMadeText.GetComponent<TextMesh>().text = realtimeMoneyMade.ToString();
	}



	/// <summary>
	/// Get the total amount of collected ingredient id (which we already have in array)
	/// </summary>
	public int getCollectedIng(int ingID) {
		int collectedAmount = 0;
		for(int i = 0; i < catchedIngredients.Count; i++) {
			if(catchedIngredients[i] == ingID) {
				collectedAmount++;
			}
		}
		return collectedAmount;
	}


	void UpdateCatchedIngredientsArray () {
		//debug
		/*
		print ("****************************************");
		for(int i = 0; i < catchedIngredients.Count; i++) {
			print ("Catched ingredient index[" + i.ToString() + "] " + "is: " + catchedIngredients[i]);
			print ("-----");
		}
		print ("****************************************");
		*/
	}

	///***********************************************************************
	/// Clone a new ingredient object and let it fall from sky
	///***********************************************************************
	void cloneIngredient() {
		createIngredient = false;
		zModifier -= 0.001f;
		startPoint = new Vector3( Random.Range(-2.1f, 2.1f) , 5.8f, zModifier);

		GameObject ing = null;

		if(Random.value >= endIngredientChance) {

			//Optional.
			//To ease the game and make the gameplay faster, we can increase the chance of cloning ingredients
			//that are related to level missions.
			if(Random.value <= relatedIngredientChance) {
				int rnd = Random.Range(0, missionData.Count);	//get a random index of mission ings
				int tmpID = (int)missionData[rnd].x - 1;		//available ingredients index starts from 0
				ing = Instantiate(availableIngredients[tmpID], startPoint, Quaternion.Euler( new Vector3(0, 180, 0))) as GameObject;	
			} else {
				//create a random regular ingredient
				ing = Instantiate(availableIngredients[Random.Range(0, availableIngredients.Length)], startPoint, Quaternion.Euler( new Vector3(0, 180, 0))) as GameObject;	
			}


		} else {
			//create an End ingredient
			ing = Instantiate(endIngredient, startPoint, Quaternion.Euler( new Vector3(0, 180, 0))) as GameObject;	
		}

		ing.name = "Ingredient-" + Random.Range(100, 1000).ToString();
		float scaleModifier = -0.05f;
		ing.transform.localScale = new Vector3(ing.transform.localScale.x + scaleModifier,
		                                       ing.transform.localScale.y + scaleModifier,
		                                       ing.transform.localScale.z);
		StartCoroutine(reactiveIngredientCreation());
	}


	//enable this controller to be able to clone ingredient objects again
	IEnumerator reactiveIngredientCreation() {
		yield return new WaitForSeconds(ingredientCloneDelay);
		createIngredient = true;
	}


	/// <summary>
	/// Finish plane shows the game status, win/lose situations, and reports about the income of the level.
	/// </summary>
	void showFinishPlane() {

		if(showFinishPlaneFlag)
			return;

		showFinishPlaneFlag = true;

		//calculate the prize (Money) for this level. Each ingredient is worth $0.6 for now.
		float moneyMade = getTotalIngredientsRequired() * ingredientValue;
		float tipMade = moneyMade * 0.15f;
		float totalMoneyMade = moneyMade + tipMade;

		//set variables
		if(gameIsWon) {

			playSfx(winSfx);

			levelStatusDynamic.GetComponent<TextMesh>().text = playerProgress.ToString() + "   Completed!";
			moneyDynamic.GetComponent<TextMesh>().text = "$" + moneyMade.ToString();
			tipDynamic.GetComponent<TextMesh>().text = "$" + tipMade.ToString();
			totalDynamic.GetComponent<TextMesh>().text = "$" + totalMoneyMade.ToString();

			//save progress, and unlock next mission
			PlayerPrefs.SetInt("playerProgress", PlayerPrefs.GetInt("playerProgress") + 1);

			//save new money
			PlayerPrefs.SetFloat("playerMoney", PlayerPrefs.GetFloat("playerMoney") + totalMoneyMade);

		} else {

			playSfx(loseSfx);

			levelStatusDynamic.GetComponent<TextMesh>().text = playerProgress.ToString() + "   Failed!";
			moneyDynamic.GetComponent<TextMesh>().text = "$0.0";
			tipDynamic.GetComponent<TextMesh>().text = "$0.0";
			totalDynamic.GetComponent<TextMesh>().text = "$0.0";

			//save progress, lock the current mission as the punishment, and unlock previous level again
			int punishmentLevel = PlayerPrefs.GetInt("playerProgress") - 1;
			if(punishmentLevel < 1) 
				punishmentLevel = 1;
			PlayerPrefs.SetInt("playerProgress", punishmentLevel);

		}

		finishPlane.SetActive(true);
	}


	/// <summary>
	/// Gets the total required amount of all mission ingredients
	/// Used to calculate the prize of the player after the game is finished.
	/// </summary>
	float getTotalIngredientsRequired() {
		float value = 0;
		for(int i = 0; i < missionData.Count; i++) {
			value += missionData[i].y;
		}

		print ("Total Ingredients Required: " + value);
		return value;
	}


	//*****************************************************************************
	// Play sound clips
	//*****************************************************************************
	void playSfx ( AudioClip _clip  ){
		GetComponent<AudioSource>().clip = _clip;
		if(!GetComponent<AudioSource>().isPlaying) {
			GetComponent<AudioSource>().Play();
		}
	}
}