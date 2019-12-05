using System;
using UnityEngine;

public class CamControl : MonoBehaviour {

	public GameObject cameraPosition;
	public float slerpValue = 0.2f;
	public int verticalOffsetAngle = -90;
	public int horizontalOffsetAngle = 0;
	private Vector3 cameraOffset;
	private Quaternion phoneOrientation;
	private Quaternion correctedPhoneOrientation;
	private Quaternion horizontalRotationCorrection;
	private Quaternion verticalRotationCorrection;
	private Quaternion inGameOrientation;
	
	public bool useGyroMode;

	void  Start (){
		// Checks if device has a gyroscope to enable
		useGyroMode = SystemInfo.supportsGyroscope;
		if (useGyroMode) 
			Input.gyro.enabled = true;

	}
		
	void  Update (){
		//if(EventSystem.current.IsPointerOverGameObject()) return;
		
		if(useGyroMode)
			UsingGyro();
		else
			UsingTouch();
	}
	
	
	Vector3 initialClick, endClick;
	void UsingTouch(){
		if(Input.GetMouseButtonDown(0)){
			initialClick.x = Input.mousePosition.x;
			initialClick.y = Input.mousePosition.y;
		}
		if(Input.GetMouseButton(0)){
			endClick.x += Input.mousePosition.x - initialClick.x;
			endClick.y += Input.mousePosition.y - initialClick.y;
		}
		RotateCam();
	}
	
	void RotateCam(){
		transform.rotation = Quaternion.Slerp(
				transform.rotation,
				Quaternion.Euler(endClick.y*-0.005f,endClick.x*0.02f,0),
				0.3f
			);
							
			cameraPosition.transform.rotation = transform.rotation;
			transform.position = cameraPosition.transform.position;
	}
		
		
	void UsingGyro(){
		// Retrieves gyroscopic information from phone
		phoneOrientation = Input.gyro.attitude;
		correctedPhoneOrientation = new Quaternion(phoneOrientation.x, phoneOrientation.y, -phoneOrientation.z, -phoneOrientation.w);
		verticalRotationCorrection = Quaternion.AngleAxis(verticalOffsetAngle, Vector3.left);
		horizontalRotationCorrection = Quaternion.AngleAxis(horizontalOffsetAngle, Vector3.up);
		inGameOrientation = horizontalRotationCorrection * verticalRotationCorrection * correctedPhoneOrientation;
		// Changes orientation of in-game camera to reflect orientation of phone
		transform.rotation = Quaternion.Slerp(transform.rotation, inGameOrientation, slerpValue);
		// Forces camera to follow player
		transform.position = cameraPosition.transform.position;
		cameraPosition.transform.rotation = transform.rotation;
		
	}
			
}
