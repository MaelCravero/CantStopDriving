using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

[System.Serializable]
public class WheelPair
{
	public WheelCollider leftWheel;
	public GameObject leftWheelMesh;
	public WheelCollider rightWheel;
	public GameObject rightWheelMesh;
	public bool motor;
	public bool steering;
	public bool reverseTurn;
}

public class Vehicle : MonoBehaviour
{
	public List<WheelPair> wheelPairs;
	public float maxMotorTorque;
	public float maxSteeringAngle;
	
    public GameObject obj;
	new private Transform transform;
	new private Rigidbody rigidbody;

    public ParticleSystem CoreFire, FrontFire, RearFire;
    public GameObject CarBody, CarFront, CarRear;
    private bool Burning;


    private void Start()
    {
        transform = obj.transform;
        rigidbody = obj.GetComponent<Rigidbody>();
    }

    public void UpdateWheel(WheelPair wheelPair)
	{
		Quaternion rot;
		Vector3 pos;
		wheelPair.leftWheel.GetWorldPose(out pos, out rot);
		wheelPair.leftWheelMesh.transform.position = pos;
		wheelPair.leftWheelMesh.transform.rotation = rot;
		wheelPair.rightWheel.GetWorldPose(out pos, out rot);
		wheelPair.rightWheelMesh.transform.position = pos;
		wheelPair.rightWheelMesh.transform.rotation = rot;
	}

    public void Explode()
    {
        if(CarBody == null)
        {
            return;
        }

        CarFront.transform.position = new Vector3(CarFront.transform.position.x, CarFront.transform.position.y + 2, CarFront.transform.position.z);

        // Core
        Rigidbody CarBodyBody = CarBody.AddComponent<Rigidbody>();
        BoxCollider carBodyCollider = CarBody.AddComponent<BoxCollider>();
        carBodyCollider.size = new Vector3(0.5f, 0.5f, 0.5f);
        var locVel = transform.InverseTransformDirection(CarBodyBody.velocity);
        locVel.y += 12;
        locVel.x += 2;
        CarBodyBody.velocity = transform.TransformDirection(locVel);
        CoreFire.Play();
        
        // Front
        Rigidbody CarFrontBody = CarFront.AddComponent<Rigidbody>();
        BoxCollider carFrontCollider = CarFront.AddComponent<BoxCollider>();
        carFrontCollider.size = new Vector3(0.5f, 0.5f, 0.5f);
        locVel = transform.InverseTransformDirection(CarFrontBody.velocity);
        locVel.z += 8;
        locVel.y += 6;
        CarFrontBody.velocity = transform.TransformDirection(locVel);
        FrontFire.Play();
        
        // Rear
        Rigidbody CarRearBody = CarRear.AddComponent<Rigidbody>();
        BoxCollider carRearCollider = CarRear.AddComponent<BoxCollider>();
        carRearCollider.size = new Vector3(0.5f, 0.5f, 0.5f);
        locVel = transform.InverseTransformDirection(CarRearBody.velocity);
        locVel.z -= 8;
        locVel.y += 6;
        CarRearBody.velocity = transform.TransformDirection(locVel);
        RearFire.Play();
        
        //transform.DetachChildren(); // crash
    }

    public void Reset()
	{
		transform.position = new Vector3(0, 0, 0);
        transform.rotation = new Quaternion (0, 0, 0, 0);
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
	}

	public void Jump()
	{
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 10, rigidbody.velocity.z);
	}

	public void Boost()
	{
        Debug.Log("Boost");
        rigidbody.velocity = new Vector3 (rigidbody.velocity.x * 2f, rigidbody.velocity.y * 2f, rigidbody.velocity.z * 2f);
	}

	public void Brake()
	{
        rigidbody.velocity = new Vector3 (rigidbody.velocity.x * 0.99f, rigidbody.velocity.y * 0.99f, rigidbody.velocity.z * 0.99f);
	}

	public void Drift()
	{
		WheelFrictionCurve w = wheelPairs[1].leftWheel.sidewaysFriction;
		w.extremumSlip = 2f;
		wheelPairs[1].leftWheel.sidewaysFriction  = w;
		wheelPairs[1].rightWheel.sidewaysFriction = w;
	}
	
	public void nDrift()
	{
		WheelFrictionCurve w = wheelPairs[1].leftWheel.sidewaysFriction;
		w.extremumSlip = 0.2f;
		wheelPairs[1].leftWheel.sidewaysFriction  = w;
		wheelPairs[1].rightWheel.sidewaysFriction = w;
	}
	
	public void Update()
	{
        if (Input.GetKey (KeyCode.LeftShift))
			Drift();
		else
			nDrift();
		
        if (Input.GetKey (KeyCode.B))
			//Boost();
		
		if (Input.GetKey (KeyCode.J))
			//Jump();
		
		if (Input.GetKey (KeyCode.R))
			Reset();

        if (Input.GetKey (KeyCode.Space))
        {
            Brake();
        }

        float motor = maxMotorTorque * Input.GetAxis("Vertical");
		float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
		float brakeTorque = Mathf.Abs(Input.GetAxis("Jump") * 2);

        rigidbody.velocity = new Vector3(0, 0, 0);
        if (!Input.GetKey(KeyCode.UpArrow))
		{
            rigidbody.velocity = new Vector3(0, 0, 0);
            //rigidbody.velocity = new Vector3 (rigidbody.velocity.x * 0.98f, rigidbody.velocity.y, rigidbody.velocity.z * 0.98f);
		}
		
		if (brakeTorque > 0.001)
		{
			brakeTorque = maxMotorTorque;
			motor = 0;
		} 
		else
        {
            brakeTorque = 0;
        }
		
		foreach (WheelPair wheelPair in wheelPairs)
		{
			if (wheelPair.steering) 
				wheelPair.leftWheel.steerAngle = wheelPair.rightWheel.steerAngle = ((wheelPair.reverseTurn)?-1:1)*steering;

			if (wheelPair.motor)
			{
				wheelPair.leftWheel.motorTorque = motor;
				wheelPair.rightWheel.motorTorque = motor;
			}
            
			wheelPair.leftWheel.brakeTorque = brakeTorque;
			wheelPair.rightWheel.brakeTorque = brakeTorque;

			UpdateWheel(wheelPair);
		}

    }

}