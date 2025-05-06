using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    #region Private Attributes
    private float _playerSpeed = 15f;
    private float _jumpForce = 20f;
    #endregion 

    #region Component
    private PlayerInput _playerInput; //這個要在玩家上綁PlayerInput，並且要放Input System進去
    private InputAction _moveAction; //移動的InputAction定義
    #endregion

     #region Variables Can't Change
    private float _groundDistance = 0.01f;
    private float _checkGroundDistanceRay = 10f;
    private float _wallDistance = 0.01f;
    private float _checkWallDistanceRay = 10f;
    private LayerMask _groundLayer;
    protected bool _isGround; //child can read this bool
    protected bool _isWallLeft; //child can read this bool
    protected bool _isWallRight; //child can read this bool
    private float _gravity = -40f;
    protected float _verticalVelocity = 0f;
    private bool _fixGround = false;
    private bool _fixWallLeft = false;
    private bool _fixWallRight = false;
    private float _checkGroundDistance;
    private float _checkWallDistance;
    private float _checkWallDistance2;
    private float _offset = 0.1f;
    private Vector3 _originalPosition;
    protected int direction = 1;
    #endregion

    #region RaycastPosition
    [SerializeField]private Transform _groundCheck;
    [SerializeField]private Transform _wallCheck;
    [SerializeField]private Transform _wallCheck2;
    #endregion

    #region Awake/Start
    //Init
    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _moveAction = _playerInput.actions["Move"]; //把InputAction指定到PlayerInput的Move上面
        _groundLayer = LayerMask.GetMask("Ground");
        Debug.Log("Awake");
    }

    private void Start()
    {
        //Debug.Log("Start");
    }
    #endregion

    #region Update/FixedUpdate
    private void Update()
    {
        Movement();
    }
    private void FixedUpdate()
    {
        Gravity();
    }
    #endregion

    #region Player Movement
    private void Movement()
    {
        IsWall();
        //Move
        Vector2 horizontal = _moveAction.ReadValue<Vector2>(); //horizontal = Move輸出出來的值
        if(horizontal.x == 0)
        {
            direction = 0;
        }
        else if(horizontal.x > 0)
        {
            direction = 1;
            _fixWallLeft = false;
        }
        else if(horizontal.x < 0)
        {  
            direction = -1; 
            _fixWallRight = false;
        }

        // Debug.Log(transform.position.x);
        // Debug.Log(transform.position.x + (direction * _playerSpeed * Time.deltaTime));
        // Debug.Log(transform.position.x + _checkWallDistance);
        // Debug.Log(_checkWallDistance);
        if(direction == -1 && transform.position.x + (direction * _playerSpeed * Time.deltaTime) < transform.position.x - (_checkWallDistance - _offset))
        {
            direction = 0;
            // Debug.LogError("Left");
            if(!_fixWallLeft)
            {
                _fixWallLeft = true;
                transform.position -= new Vector3(_checkWallDistance - _offset, 0 ,0);
                // Debug.Log(new Vector3(_checkWallDistance -_offset, 0 ,0));
            }
        }
        if(direction == 1 && transform.position.x + (direction * _playerSpeed * Time.deltaTime) > transform.position.x + (_checkWallDistance2 - _offset))
        {
            direction = 0;
            // Debug.LogError("Right");
            if(!_fixWallRight)
            {
                _fixWallRight = true; 
                transform.position += new Vector3(_checkWallDistance2 - _offset, 0 ,0);
                // Debug.Log("Right");
            }
        }

        transform.position += new Vector3(direction * _playerSpeed * Time.deltaTime, 0, 0);
    }

    //Is Press Jump Key or Not
    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.performed && _isGround)
        {
            _verticalVelocity = _jumpForce;
            _isGround = false;
            // Debug.Log(_isGround);
            // Debug.Log(_verticalVelocity);
            // Debug.Log(_jumpForce);
        }
    }
    #endregion

    #region Physics
    //Gravity
    private void Gravity()
    {
        if(!_isGround)
        {
            //Debug.Log(transform.position.y + (_verticalVelocity * Time.deltaTime));
            if(transform.position.y + (_verticalVelocity * Time.deltaTime) > transform.position.y - _checkGroundDistance)
            {
                _verticalVelocity += _gravity * Time.deltaTime;
                //Debug.Log("Oh Yeah Gravity is working :D");
            }
        }
        else
        {
            _verticalVelocity = 0f;
        }

        transform.position += new Vector3(0, _verticalVelocity * Time.deltaTime, 0);

        IsGround();
    }
    #endregion

    #region RaycastCheck
    //Is Ground or Not
    private void IsGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(_groundCheck.transform.position, Vector2.down, _groundDistance, _groundLayer);
        RaycastHit2D rayDistance = Physics2D.Raycast(_groundCheck.transform.position, Vector2.down, _checkGroundDistanceRay, _groundLayer);
        Debug.DrawRay(_groundCheck.position, Vector2.down * _checkGroundDistanceRay, Color.blue);
        if(hit.collider == null)
        {
            _checkGroundDistance = rayDistance.distance;
            _originalPosition = transform.position;
            if(_fixGround)
            {
                _fixGround = false;
            }
        }
        _isGround = hit.collider != null;
        if(hit.collider != null)
        {
            //Debug.Log(_isGround);
            if(!_fixGround)
            {
                transform.position = new Vector3(transform.position.x, _originalPosition.y - _checkGroundDistance, 0);
                _fixGround = true;
            }
        }
    }

    //Is Ground or Not
    private void IsWall()
    {
        Vector3 leftStart = _wallCheck2.transform.position + Vector3.right * _offset;
        Vector3 rightStart = _wallCheck.transform.position + Vector3.left * _offset;
        RaycastHit2D hit = Physics2D.Raycast(leftStart, Vector2.left, _wallDistance, _groundLayer);
        RaycastHit2D hit2 = Physics2D.Raycast(rightStart, Vector2.right, _wallDistance, _groundLayer);
        RaycastHit2D rayDistance = Physics2D.Raycast(leftStart, Vector2.left, _checkWallDistanceRay, _groundLayer);
        RaycastHit2D rayDistance2 = Physics2D.Raycast(rightStart, Vector2.right, _checkWallDistanceRay, _groundLayer);
        Debug.DrawRay(leftStart, Vector2.left * _checkWallDistanceRay, Color.blue);
        Debug.DrawRay(rightStart, Vector2.right * _checkWallDistanceRay, Color.blue);
        _originalPosition = transform.position;

        // 左牆距離更新
        if (hit.collider == null)
        {
            _checkWallDistance = rayDistance.collider != null ? rayDistance.distance : _checkWallDistanceRay;
        }
        else
        {
            _checkWallDistance = _checkWallDistanceRay;
        }

        // 右牆距離更新
        if (hit2.collider == null)
        {
            _checkWallDistance2 = rayDistance2.collider != null ? rayDistance2.distance : _checkWallDistanceRay;
        }
        else
        {
            _checkWallDistance2 = _checkWallDistanceRay;
        }
    }
    #endregion
}