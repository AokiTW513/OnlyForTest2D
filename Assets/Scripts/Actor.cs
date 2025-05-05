using Unity.Mathematics;
using UnityEngine;

public class Actor : MonoBehaviour
{
    #region Variables
    private float _health = 100;
    private float _maxHealth = 100;
    private float _minHealth = 0;
    private float _speed = 5;
    #endregion

    #region Variables Can't Change
    private float _groundDistance = 0.01f;
    private float _checkGroundDistanceRay = 10f;
    private float _wallDistance = 0.01f;
    private float _checkWallDistanceRay = 10f;
    private LayerMask _groundLayer;
    protected bool _isGround; //child can read this var
    private float _gravity = -25f;
    protected float _verticalVelocity = 0f;
    private bool _fixGround = false;
    private float _checkGroundDistance;
    private float _checkWallDistance;
    private float _checkWallDistance2;
    private Vector3 _originalPosition;
    protected int direction = 1;
    #endregion

    #region RaycastPosition
    private Transform _groundCheck;
    private Transform _wallCheck;
    private Transform _wallCheck2;
    #endregion

    #region Awake/Start
    protected virtual void Awake()
    {
        _groundLayer = LayerMask.GetMask("Ground");
    }

    protected virtual void Start()
    {
        //Notthing
    }
    #endregion

    #region Update/FixedUpdate
    protected virtual void Update()
    {
        Movement();
    }

    protected virtual void FixedUpdate()
    {
        Gravity();
        IsGround();
        IsWall();
    }
    #endregion

    #region InitializeAttributes
    //Init Setting
    protected virtual void InitializeAttributes(float newHealth, float newMaxHealth, float newMinHealth, float newSpeed, GameObject objectName, Transform newGroundCheck, Transform newWallCheck, Transform newWallCheck2)
    {
        Debug.Log(objectName.gameObject.name + " Start To Init");
        _health = newHealth;
        _maxHealth = newMaxHealth;
        _minHealth = newMinHealth;
        _speed = newSpeed;
        _groundCheck = newGroundCheck;
        _wallCheck = newWallCheck;
        _wallCheck2 = newWallCheck2;
        Debug.Log(objectName.gameObject.name + " Health:" + _health);
        Debug.Log(objectName.gameObject.name + " MaxHealth:" + _maxHealth);
        Debug.Log(objectName.gameObject.name + " MinHealth:" + _minHealth);
        Debug.Log(objectName.gameObject.name + " Speed:" + _speed);
    }
    #endregion

    #region Movement
    //Test Movement
    protected virtual void Movement()
    {
        transform.position += 2f * Vector3.right * Time.deltaTime;
        //Debug.Log("Test Movement");
    }
    #endregion

    #region Physics
    //Gravity
    protected virtual void Gravity()
    {
        if(!_isGround)
        {
            _verticalVelocity += _gravity * Time.deltaTime;
            //Debug.Log("Oh Yeah Gravity is working :D");
        }
        else
        {
            _verticalVelocity = 0f;
        }

        transform.position += new Vector3(0, _verticalVelocity * Time.deltaTime, 0);
    }
    #endregion

    #region RaycastCheck
    //Is Ground or Not
    protected virtual void IsGround()
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
            if(!_fixGround)
            {
                transform.position = new Vector3(transform.position.x, _originalPosition.y - _checkGroundDistance, 0);
                _fixGround = true;
            }
        }
    }

    //Is Ground or Not
    protected virtual void IsWall()
    {
        RaycastHit2D hit = Physics2D.Raycast(_wallCheck2.transform.position, Vector2.left, _wallDistance, _groundLayer);
        RaycastHit2D hit2 = Physics2D.Raycast(_wallCheck.transform.position, Vector2.right, _wallDistance, _groundLayer);
        RaycastHit2D rayDistance = Physics2D.Raycast(_wallCheck2.transform.position, Vector2.left, _checkWallDistanceRay, _groundLayer);
        RaycastHit2D rayDistance2 = Physics2D.Raycast(_wallCheck.transform.position, Vector2.right, _checkWallDistanceRay, _groundLayer);
        Debug.DrawRay(_groundCheck.position, Vector2.left * _checkWallDistanceRay, Color.blue);
        Debug.DrawRay(_groundCheck.position, Vector2.right * _checkWallDistanceRay, Color.blue);
        if(hit.collider == null && hit2.collider == null)
        {
            _checkWallDistance = rayDistance.distance;
            _checkWallDistance2 = rayDistance2.distance;
            Debug.Log(_checkWallDistance2);
            _originalPosition = transform.position;
        }
        //Left
        if(hit.collider != null && direction == -1)
        {
            transform.position = new Vector3(_originalPosition.x - _checkWallDistance, transform.position.y, 0);
        }
        //Right
        if(hit2.collider != null && direction == 1)
        {   
            Debug.Log(_originalPosition.x + _checkWallDistance2);
            Debug.Log(_originalPosition.x);
            Debug.Log(_checkWallDistance2);
            transform.position = new Vector3(_originalPosition.x + _checkWallDistance2, transform.position.y, 0);
        }
    }
    #endregion
}