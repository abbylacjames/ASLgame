using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionManager : MonoBehaviour {

	/// <summary>
	/// This is the main mission manager of the game. You can add unlimited number of levels to the game
	/// within minutes. All you have to do is to: create a new switch case (see the examples below),
	/// set the count of the missions (1 to 6)
	/// set the ID and Amount of each required ingredients and that's it. You have a new mission ;)
	/// </summary>
	/// <returns>The level mission data.</returns>
	/// <param name="level">Level.</param>

	public List<Vector2> getLevelMissionData(int level) {
	
		// Very important!!
		// Args for Vector2: 
		// Arg1 = ID of selected ingredients (from 1 to 11)
		// Arg2 = Required amount of the selected ingredients (from 1 to 20)

		List<Vector2> ingIdAmount = new List<Vector2>();

		switch(level) {
		case 1:
			//Set level objectives
			ingIdAmount = new List<Vector2>(2);		//we will have 2 missions
			ingIdAmount.Add(new Vector2(1, 2));		//mission 1 is ingredient 1 with 2 required amount
			ingIdAmount.Add(new Vector2(2, 1));		//mission 2 is ingredient 2 with 1 required amount
			break;
		case 2:
			ingIdAmount = new List<Vector2>(3);
			ingIdAmount.Add(new Vector2(2, 2));
			ingIdAmount.Add(new Vector2(3, 1));
			ingIdAmount.Add(new Vector2(4, 3));
			break;
		case 3:
			ingIdAmount = new List<Vector2>(3);
			ingIdAmount.Add(new Vector2(3, 3));
			ingIdAmount.Add(new Vector2(4, 4));
			ingIdAmount.Add(new Vector2(5, 2));
			break;
		case 4:
			ingIdAmount = new List<Vector2>(4);
			ingIdAmount.Add(new Vector2(2, 2));
			ingIdAmount.Add(new Vector2(1, 3));
			ingIdAmount.Add(new Vector2(6, 5));
			ingIdAmount.Add(new Vector2(7, 2));
			break;
		case 5:
			ingIdAmount = new List<Vector2>(4);
			ingIdAmount.Add(new Vector2(4, 4));
			ingIdAmount.Add(new Vector2(7, 5));
			ingIdAmount.Add(new Vector2(8, 5));
			ingIdAmount.Add(new Vector2(9, 3));
			break;
		case 6:
			ingIdAmount = new List<Vector2>(5);
			ingIdAmount.Add(new Vector2(3, 6));
			ingIdAmount.Add(new Vector2(5, 5));
			ingIdAmount.Add(new Vector2(7, 4));
			ingIdAmount.Add(new Vector2(9, 2));
			ingIdAmount.Add(new Vector2(10, 6));
			break;
		case 7:
			ingIdAmount = new List<Vector2>(5);
			ingIdAmount.Add(new Vector2(2, 4));
			ingIdAmount.Add(new Vector2(5, 5));
			ingIdAmount.Add(new Vector2(4, 4));
			ingIdAmount.Add(new Vector2(10, 6));
			ingIdAmount.Add(new Vector2(11, 7));
			break;
		case 8:
			ingIdAmount = new List<Vector2>(6);
			ingIdAmount.Add(new Vector2(1, 4));
			ingIdAmount.Add(new Vector2(3, 5));
			ingIdAmount.Add(new Vector2(5, 4));
			ingIdAmount.Add(new Vector2(7, 6));
			ingIdAmount.Add(new Vector2(9, 7));
			ingIdAmount.Add(new Vector2(11, 7));
			break;
		case 9:
			ingIdAmount = new List<Vector2>(6);
			ingIdAmount.Add(new Vector2(2, 6));
			ingIdAmount.Add(new Vector2(4, 5));
			ingIdAmount.Add(new Vector2(6, 3));
			ingIdAmount.Add(new Vector2(7, 8));
			ingIdAmount.Add(new Vector2(8, 9));
			ingIdAmount.Add(new Vector2(10, 7));
			break;
		case 10:
			ingIdAmount = new List<Vector2>(6);
			ingIdAmount.Add(new Vector2(1, 8));
			ingIdAmount.Add(new Vector2(2, 10));
			ingIdAmount.Add(new Vector2(4, 9));
			ingIdAmount.Add(new Vector2(7, 8));
			ingIdAmount.Add(new Vector2(9, 7));
			ingIdAmount.Add(new Vector2(1, 8));
			break;
		}

		return ingIdAmount;
	}


}
