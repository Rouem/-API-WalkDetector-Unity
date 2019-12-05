using UnityEngine;

public class PlayerControl : MonoBehaviour {

	// Use this for initialization
	 float timeVel, constVel;

//========================================================
	WalkDetector device;
//=======================================================

	private Rigidbody rb;

	public GameObject cam;
	public GameObject CamPos;
	public GameObject CamPrefab;


	void  Start (){

		rb = GetComponent<Rigidbody>();
	
		cam = Instantiate(CamPrefab) as GameObject;
		cam.GetComponent<CamControl>().cameraPosition = CamPos;

		device = new WalkDetector();
	}		

	void Update (){

		PlayerDirection();
		PlayerPosition();
		ConstantVelocity();

	}

	bool InGround(){
		return Physics.Raycast (
			transform.position, //ponto inicial
			-transform.up,//direção do raio
			transform.GetComponent<CapsuleCollider>().height/3//comprimento
		);
	}
		
	void ConstantVelocity(){
		if(device.IsWalking())
			timeVel += 0.9f * Time.deltaTime;
		else
			timeVel -= 1f * Time.deltaTime;

		if(timeVel <= 0)
			constVel = timeVel = 0;

		if(timeVel > 1f)
			timeVel = constVel = 1f;
		else
			constVel = timeVel*0.85f;
	}

	void PlayerDirection(){
		Vector3 rot = cam.transform.eulerAngles;
		rot.x = rot.z = 0;
		transform.eulerAngles = rot;
	}

	void PlayerPosition(){
		
		if(device.OnMove()){
			if(InGround()){
				rb.velocity = transform.forward * ( 5 * constVel);
			}else{
				rb.MovePosition(transform.position + transform.forward * (5 * 0.5f * constVel) * Time.deltaTime);
			}
		}else{
			rb.MovePosition(transform.position);
		}
	}


}
