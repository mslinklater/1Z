using UnityEngine;

[System.Serializable]
public class CharacterControlAsset : ScriptableObject
{
	// These all need decent tooltips etc
	public float moveSpeed;
	public uint maxStandJumpFrames;
	public uint maxRunJumpFrames;
	public float jumpForce;
	public float doubleJumpForce;
	public float wallJumpForce;
	public float hangJumpForce;
	public float maxFallSpeed;
	public float wallFriction;
	public float maxWallSlideSpeed;
	public float pushMoveForce;
	public float groundMoveForce;
	public float groundTurnForce;
	public float groundStopForce;
	public float airMoveForce;
	public float airTurnForce;
	public float airStopForce;
	public float wallJumpMoveForce;
	public float wallJumpStopForce;
}