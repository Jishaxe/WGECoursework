using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController2D))]

public class PlayerMovement2D : MonoBehaviour {
    public delegate void PlayerEvent();
    public PlayerEvent OnPlayerLand;

    //member variables
    PlayerController2D _pController;
    Rigidbody2D _rBody;
    public Transform _feet;
    public GameObject _cameraTarget;
    Vector3 _cameraTargetInitialPosition;
    public float _cameraPushMultiplier = 1f;
    public MovementState _mState = MovementState.ON_GROUND;

    public float _speed = 10f;
    public float _JumpImpulse = 10f;
    public float _airAcceleration = 8f;

    public float _jumpPosGravity = 0.5f;
    public float _jumpNegGravity = 0.5f;

    public int _dashChargesMax = 2;
    public int _dashCharges = 2;
    public float _dashImpulse = 4f;
    public float _dashTime = 0.5f;
    Coroutine _dashReloadHandle = null;

    public bool _isInConversation = false;
    NPCConversationScript _conversationTarget;

    // Use this for initialization
    void Start () {
        _pController = GetComponent<PlayerController2D>();
        _cameraTargetInitialPosition = _cameraTarget.transform.localPosition;
        _pController._jumpInput += Jump;
        _pController._hMoveInput += Move;
        _pController._jumpReleaseInput += JumpEnd;
        _pController._jumpPressedInput += JumpStart;
        _pController._dashPressedInput += Dash;

        _rBody = GetComponent<Rigidbody2D>();
        _rBody.gravityScale = _jumpNegGravity;

        SwitchState(MovementState.ON_GROUND);
	}

    private void OnDisable()
    {
        _pController._jumpInput -= Jump;
        _pController._hMoveInput -= Move;
        _pController._jumpReleaseInput -= JumpEnd;
        _pController._jumpPressedInput -= JumpStart;
        _pController._dashPressedInput -= Dash;
    }

    // Update is called once per frame
    void Update () {

        if (_mState == MovementState.DISABLED)
            return;

        if (_mState != MovementState.DASHING)
        {
            if (Physics2D.OverlapBoxAll(new Vector2(_feet.position.x, _feet.position.y), new Vector2(0.25f, 0.25f), 0f).Length > 1 && _mState != MovementState.DASHING)
            {
                // if the state is IN_AIR and we're currently moving downwards, send OnPlayerLand event
                if (_mState == MovementState.IN_AIR && _rBody.velocity.y < 0) OnPlayerLand();

                SwitchState(MovementState.ON_GROUND);
            }
            else
            {
                SwitchState(MovementState.IN_AIR);
            }
        }

        switch (_mState)
        {
            case MovementState.DISABLED:
                break;
            case MovementState.ON_GROUND:
                //draw feet
                Debug.DrawLine(new Vector3(_feet.position.x - 0.25f, _feet.position.y - 0.25f, 0), new Vector3(_feet.position.x + 0.25f, _feet.position.y - 0.25f, 0), Color.green, 0.016f, false);
                break;
            case MovementState.IN_AIR:
                //draw feet
                _rBody.velocity = new Vector2(Mathf.Clamp(_rBody.velocity.x, -_speed, _speed), _rBody.velocity.y);
                if (_rBody.velocity.y < 0) _rBody.gravityScale = _jumpNegGravity;
                Debug.DrawLine(new Vector3(_feet.position.x - 0.25f, _feet.position.y - 0.25f, 0), new Vector3(_feet.position.x + 0.25f, _feet.position.y - 0.25f, 0), Color.red, 0.016f, false);
                break;
        }
	}

    // stops the player from moving and pushes them back if they got too close
    public void StartConversationWith(NPCConversationScript npc)
    {
        // stop the player moving
        _rBody.velocity = new Vector2(0, 0);
        _isInConversation = true;
        _conversationTarget = npc;
        SwitchState(MovementState.DISABLED);
    }

    public void EndConversation()
    {
        _isInConversation = false;
        _conversationTarget = null;
        SwitchState(MovementState.ON_GROUND);
    }

    void SwitchState(MovementState nextState)
    {
        _mState = nextState;
        switch (_mState)
        {
            case MovementState.DISABLED:
                break;
            case MovementState.ON_GROUND:
                _dashCharges = _dashChargesMax;
                break;
            case MovementState.IN_AIR:
                break;
            case MovementState.DASHING:
                _rBody.gravityScale = 0;
                _dashReloadHandle = StartCoroutine(DashTimer(_dashTime));
                _dashCharges--;
                break;
        }
    }

    void Move(float x)
    {
        // push the camera target a little bit with our movement (if the player isn't in conversation)
        if (_isInConversation)
        {
            _cameraTarget.transform.localPosition = new Vector3(0, 0, 0);
        }
        else
        {
            _cameraTarget.transform.localPosition = new Vector3(_cameraTargetInitialPosition.x + x * _cameraPushMultiplier, _cameraTargetInitialPosition.y, _cameraTargetInitialPosition.z);
        }

        switch (_mState)
        {
            case MovementState.ON_GROUND:
                _rBody.velocity = new Vector2(x * _speed, _rBody.velocity.y);
                break;
            case MovementState.IN_AIR:
                _rBody.velocity += new Vector2(x * _airAcceleration * Time.deltaTime, 0f);
                break;
            case MovementState.DISABLED:
                break;
        }
    }

    void JumpStart()
    {
        switch (_mState)
        {
            case MovementState.DISABLED:
                break;
            case MovementState.ON_GROUND:
                SwitchState(MovementState.IN_AIR);
                _rBody.velocity = new Vector2(_rBody.velocity.x, 0);
                _rBody.AddForce(new Vector2(0, _JumpImpulse), ForceMode2D.Impulse);
                _rBody.gravityScale = _jumpNegGravity;
                break;
        }
    }

    void Jump()
    {
        if(_mState == MovementState.IN_AIR)_rBody.gravityScale = _rBody.velocity.y >= 0 ? _jumpPosGravity : _jumpNegGravity;
    }

    void JumpEnd()
    {
        if (_mState == MovementState.IN_AIR)_rBody.gravityScale = _jumpNegGravity;
    }

    void Dash(Vector2 direction)
    {
        if(_mState != MovementState.DISABLED)
        {
            if(_dashCharges > 0 && _dashReloadHandle == null)
            {
                if (direction == Vector2.zero) direction = Vector2.up;
                SwitchState(MovementState.DASHING);
                _rBody.velocity = direction.normalized * _dashImpulse;
            }
        }
    }

    IEnumerator DashTimer(float dashTime)
    {
        float timer = dashTime;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        _rBody.velocity = Vector2.zero;
        _dashReloadHandle = null;
        _rBody.gravityScale = _jumpNegGravity;
        SwitchState(MovementState.IN_AIR);
        Debug.Log("DashEnd");
    }
}

public enum MovementState { ON_GROUND, IN_AIR, DASHING, DISABLED }
