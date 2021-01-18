using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{/*
	public void GetInput()
	{
		m_horizontalInput = Input.GetAxis("Horizontal");
		m_verticalInput = Input.GetAxis("Vertical");
	}*/

	private void Steer()
	{
		m_steeringAngle = maxSteerAngle * m_horizontalInput;
		FrontLeftW.steerAngle = m_steeringAngle;
		FrontRightW.steerAngle = m_steeringAngle;
	}

	private void Accelerate()
	{
		FrontLeftW.motorTorque = m_verticalInput * motorForce;
		FrontRightW.motorTorque = m_verticalInput * motorForce;
	}

	private void UpdateWheelPoses()
	{
		UpdateWheelPose(FrontLeftW, FrontLeftT);
		UpdateWheelPose(FrontRightW, FrontRightT);
		UpdateWheelPose(RearLeftW, RearLeftT);
		UpdateWheelPose(RearLeftW, RearRightT);
	}

	private void UpdateWheelPose(WheelCollider _collider, Transform _transform)
	{
		Vector3 _pos = _transform.position;
		Quaternion _quat = _transform.rotation;

		_collider.GetWorldPose(out _pos, out _quat);

		_transform.position = _pos;
		_transform.rotation = _quat;
	}

	private void FixedUpdate()
	{
		//GetInput();
		Steer();
		Accelerate();
		//UpdateWheelPoses();
	}

	private float m_horizontalInput { get { return Input.GetAxis("Horizontal"); } }
	private float m_verticalInput { get { return Input.GetAxis("Vertical"); } }
	private float m_steeringAngle;

	public WheelCollider FrontLeftW, FrontRightW;
	public WheelCollider RearLeftW, RearRightW;
	public Transform FrontLeftT, FrontRightT;
	public Transform RearLeftT, RearRightT;
	public float maxSteerAngle = 30;
	public float motorForce = 50;
}
