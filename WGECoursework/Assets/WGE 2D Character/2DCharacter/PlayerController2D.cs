using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2D : MonoBehaviour {

    public delegate void JumpInput();
    public delegate void JumpReleaseInput();
    public delegate void JumpPressedInput();
    public delegate void HorizontalMoveInput(float x);
    public delegate void DashPressedInput(Vector2 direction);

    public event JumpInput _jumpInput;
    public event JumpReleaseInput _jumpReleaseInput;
    public event HorizontalMoveInput _hMoveInput;
    public event JumpPressedInput _jumpPressedInput;
    public event DashPressedInput _dashPressedInput;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float hMove = Input.GetAxis("Horizontal");
        float vMove = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Jump"))
        {
            _jumpPressedInput();
        }
        if (Input.GetButton("Jump"))
        {
            _jumpInput();
        }
        if(Input.GetButtonUp("Jump"))
        {
            _jumpReleaseInput();
        }
        if(Input.GetButtonDown("Fire1"))
        {
            _dashPressedInput(new Vector2(hMove, vMove));
        }

        _hMoveInput(hMove);
	}
}
