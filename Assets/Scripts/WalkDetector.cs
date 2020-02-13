using UnityEngine;

public class WalkDetector : MonoBehaviour {

	// Use this for initialization

	//========================================================
	Vector3 Diferences, CurrentValues, PastValues = new Vector3(9.9f,9.9f,9.9f), lowPassValues;
	Vector3 currentMean, pastMean, meanDif;
	float minYpass, maxYpass, middleYpass, zpass, xpass;//for xioami values

	public bool[] validWalk = new bool[3];
	bool needRevalidation;

	float accelerometerUpdateInterval;
	float lowPassKernelWidthInSeconds;
	float lowPassFilterFactor;

	//=======================================================

	public void Start (){

		lowPassValues = Input.acceleration;

		accelerometerUpdateInterval = 1.0f / 60.0f;
		lowPassKernelWidthInSeconds = 1.0f;
		lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
		
		GetDiviceType();
	}		

	float LowPassFilterAccelerometer (float LowPassValue, float AxisAcceleration){
		// Smooths out noise from accelerometer data
		LowPassValue = Mathf.Lerp(LowPassValue, AxisAcceleration, lowPassFilterFactor);
		LowPassValue = Mathf.Sqrt(LowPassValue*LowPassValue);
		return LowPassValue;
	}

	float GetDiferenceAbsolute(float a, float b){
		return Mathf.Abs(a-b);
	}

	public bool IsWalking(){
		// - it's recommended don't change this values -

		//checking if the device's move it's a true walk or invalid shake
		if(
			validWalk[1] 
			&& validWalk[2]
			&& validWalk[0]
		)
			return true;
		else
			return false;
		
	}

	void MeanOfValues(){
		currentMean = Vector3.zero;
		for(int i = 0; i < 3; i++){
			currentMean[i] = (CurrentValues[i] + pastMean[i])/2;
			meanDif[i] = GetDiferenceAbsolute(pastMean[i],currentMean[i]);
			pastMean[i] = Mathf.Abs(currentMean[i]);
		}
	}

	public void ValidStepWalk(){
		//it's recommended don't change this values.


		//Check if player continues in moving (on axis Y)
		if(
			meanDif[1] >  minYpass &&
			meanDif[1] <= maxYpass
		){
			//Check if the first step is valid (on axis Y)
			if(
				meanDif[1] >  middleYpass &&
				!needRevalidation
			){
				validWalk[1] = true;
			}else{
				needRevalidation = false;
			}
		}else{
			validWalk[1] = false;
			needRevalidation = true;
		}


		//Check if the first step is valid (on axis Z)
		if(
			meanDif[2] >  zpass
		){
			validWalk[2] = true;
		}else
			validWalk[2] = false;
		
		//Check if device is not shaking (on axis x)
		if(
			meanDif[0] <  xpass
		){
			validWalk[0] = true;
		}else
			validWalk[0] = false;
		
	}

	public bool OnMove (){
		
		CurrentValues.x = LowPassFilterAccelerometer(lowPassValues.x,Input.acceleration.x);
		CurrentValues.y = LowPassFilterAccelerometer(lowPassValues.y,Input.acceleration.y);
		CurrentValues.z = LowPassFilterAccelerometer(lowPassValues.z,Input.acceleration.z);

		Diferences.x = GetDiferenceAbsolute(CurrentValues.x, PastValues.x);
		Diferences.y = GetDiferenceAbsolute(CurrentValues.y, PastValues.y);
		Diferences.z = GetDiferenceAbsolute(CurrentValues.z, PastValues.z);

		MeanOfValues();
		ValidStepWalk();

		// Maps the player's real-world steps into in-game movement
		if(IsWalking()){
			PastValues.x = Mathf.Abs(CurrentValues.x);
			PastValues.y = Mathf.Abs(CurrentValues.y);
			PastValues.z = Mathf.Abs(CurrentValues.z);
			return true;
		}else{
			PastValues.x = Mathf.Abs(CurrentValues.x);
			PastValues.y = Mathf.Abs(CurrentValues.y);
			PastValues.z = Mathf.Abs(CurrentValues.z);
			return false;
		}
	}
	
	void GetDiviceType(){
		string device = SystemInfo.deviceModel;
		string letters = (device[0].ToString() + device[1].ToString()).ToLower();
		//Is Device xiaomi?
		if(letters.Equals("mi")){
			minYpass = 0.00005f;
			maxYpass = 0.0015f;
			middleYpass = 0.0002f;
			zpass = 0.000125f;
			xpass = 0.005f;
		}else{
			//if another device (best values!)
			UseStandardValues();
		}
	}

	void UseStandardValues(){
		
			minYpass = 0.0001f;
			maxYpass = 0.0015f;
			middleYpass = 0.00025f;
			zpass = 0.00025f;
			xpass = 0.0045f;

	}
	
}
