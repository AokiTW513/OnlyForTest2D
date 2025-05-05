using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Actor
{
    #region Private Attributes
    private float _playerHealth = 100f;
    private float _playerMaxHealth = 100f;
    private float _playerMinHealth = 0f;
    private float _playerSpeed = 8f;
    private float _jumpForce = 7.5f;
    #endregion 

    #region Public Attributes
    public float Health => _playerHealth; //讓別的程式可以讀取Health值
    #endregion

    #region Component
    private PlayerInput _playerInput; //這個要在玩家上綁PlayerInput，並且要放Input System進去
    private InputAction _moveAction; //移動的InputAction定義
    #endregion

    #region SerializeField Things
    [SerializeField] private Transform _playerGroundCheck;
    [SerializeField] private Transform _playerWallCheck;
    [SerializeField] private Transform _playerWallCheck2;
    #endregion

    #region Awake/Start
    //Init
    protected override void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _moveAction = _playerInput.actions["Move"]; //把InputAction指定到PlayerInput的Move上面
        InitializeAttributes(_playerHealth, _playerMaxHealth, _playerMinHealth, _playerSpeed, this.gameObject, _playerGroundCheck, _playerWallCheck, _playerWallCheck2); //初始化數值用，呼叫父物件Actor的函數
        base.Awake();
        Debug.Log("Awake");
    }

    protected override void Start()
    {
        base.Start();
        //Debug.Log("Start");
    }
    #endregion

    #region Update/FixedUpdate
    protected override void Update()
    {
        base.Update();
        //Debug.Log("Update");
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        //Debug.Log("FixedUpdate");
    }
    #endregion

    #region Player Movement
    protected override void Movement()
    {
        //Move
        Vector2 horizontal = _moveAction.ReadValue<Vector2>(); //horizontal = Move輸出出來的值
        transform.position += new Vector3(horizontal.x * _playerSpeed * Time.deltaTime, 0, 0);
        if(horizontal.x == 0)
        {
            direction = 0;
        }
        else if(horizontal.x > 0)
        {
            direction = 1;
        }
        else if(horizontal.x < 0)
        {  
            direction = -1; 
        }
    }

    //Is Press Jump Key or Not
    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.performed && _isGround)
        {
            _verticalVelocity = _jumpForce;
            _isGround = false;
        }
    }
    #endregion
}