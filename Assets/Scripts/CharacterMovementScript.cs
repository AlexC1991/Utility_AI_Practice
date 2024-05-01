using UnityEngine;
using Random = UnityEngine.Random;

namespace AlexzanderCowell
{

    public class CharacterMovementScript : MonoBehaviour
    {
        [Header("Inspector Changes")]
        [Range(-50,50)]
        [SerializeField] private float gravitySlider;
        [Range(0,30)]
        [SerializeField] private float walkSpeed = 6;
        [SerializeField] private float downValue, upValue;
        /*[SerializeField] private LayerMask collisionLayer;*/
        [SerializeField] private float jumpHeight;
        /*[SerializeField] private GameObject[] weaponAndShield;
        [TagSelector]
        [SerializeField] private string thisTag;*/
        [SerializeField] private Camera _playerCamera;
        
        [Header("Internal Edits")]
        public static CharacterController _controller;
        private bool _runFaster;
        [HideInInspector] public static float _mouseSensitivityY;
        [HideInInspector] public static float _mouseSensitivityX;
        private float _normalWalkSpeed;
        private float _mouseXposition,
            _moveHorizontal,
            _moveVertical,
            _mouseYposition;
        private Vector3 _moveDirection;
        private Animator _playersAnimation;
        private float _timeElapsed = 0;
        private float _duration = 3;
        private float idleTimer = 2;
        private float _idleResetTimer;
        private bool _playerIsJumping;
        private float _running;
        private bool _isRunning;
        private bool _weaponKey;
        private int _weaponButtonCounter;
        private float _randomAttack;
        private bool checkAttackNumber;
        private float storedAttackNumber;
        public static bool _playerIsBlocking;
        public static bool _playerIsAttacking;
        private bool _playerIsInAttackRange;
        private Vector3 _playerRotation;

        private void Awake()
        {
            _playerRotation = transform.rotation.eulerAngles;
            _controller = GetComponent<CharacterController>();
            _playersAnimation = GetComponentInChildren<Animator>();
        }

        private void Start()
        {
            _running = walkSpeed * 2;
            Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
            Cursor.visible = false;
            _mouseSensitivityY = 0.7f;
            _mouseSensitivityX = 1;
            _normalWalkSpeed = walkSpeed;
            _runFaster = false;
            _idleResetTimer = idleTimer;
        }
        private void Update()
        {
            CharacterGravity();
            CharacterMovementBase();
            PlayerMovementAnimations();
            RunningMovement();
        }

        private void CharacterMovementBase()
        {
            transform.rotation = Quaternion.Euler(_playerRotation);
            _mouseXposition += Input.GetAxis("Mouse X") * _mouseSensitivityX;
            _mouseYposition -= Input.GetAxis("Mouse Y") * _mouseSensitivityY;
            _mouseYposition = Mathf.Clamp(_mouseYposition, downValue, upValue);
            
            transform.rotation = Quaternion.Euler(0f, _mouseXposition, 0f);
            _playerCamera.transform.localRotation = Quaternion.Euler(_mouseYposition, 0f, 0f);
            
                _moveHorizontal = Input.GetAxis("Horizontal"); // Gets the horizontal movement of the character.
                _moveVertical = Input.GetAxis("Vertical"); // Gets the vertical movement of the character.
                
            Vector3 movement = new Vector3(_moveHorizontal, 0f, _moveVertical); // Allows the character to move forwards and backwards & left & right.
            movement = transform.TransformDirection(movement) * walkSpeed; // Gives the character movement speed.
            _controller.Move((movement + _moveDirection) * Time.deltaTime); // Gets all the movement variables and moves the character.
        }
        

        private void CharacterGravity()
        {
            _moveDirection.y -= gravitySlider * Time.deltaTime;
            // Move the character controller with gravity
            _controller.Move(_moveDirection * Time.deltaTime);
        }
        
        bool IsAnimationPlaying(string clipName)
        {
            return _playersAnimation.GetCurrentAnimatorStateInfo(0).IsName(clipName);
        }

        private void RunningMovement()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                _isRunning = true;
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
            {
                _isRunning = false;
            }

            if (_isRunning)
            {
                walkSpeed = _running;
            }
            else
            {
                walkSpeed = _normalWalkSpeed;
            }
        }
        private void PlayerMovementAnimations()
        {
            if (_moveHorizontal > 0 || _moveVertical > 0 || _moveHorizontal < 0 || _moveVertical < 0)
            {
                if (walkSpeed == _normalWalkSpeed)
                {
                    _playersAnimation.SetFloat("Blend", 0.4f, 0.2f, Time.deltaTime);
                }
                else if (walkSpeed == _running)
                {
                    _playersAnimation.SetFloat("Blend", 0.9f, 0.2f, Time.deltaTime);
                }
                else if (_moveVertical < 0)
                {
                    _playersAnimation.SetFloat("Blend", -1, 0.2f, Time.deltaTime);
                }
            }
            else
            {
                _playersAnimation.SetFloat("Blend", 0, 0.2f, Time.deltaTime);
            }

            if (_moveHorizontal > 0 || _moveVertical > 0)
            {
                idleTimer -= 0.5f * Time.deltaTime;
            }
            
            if (idleTimer < 0.2f && _moveHorizontal > 0)
            {
                idleTimer = 0;
                _playersAnimation.SetBool("SleepAnimation", true);
            }
            else
            {
                _playersAnimation.SetBool("SleepAnimation", false);
                idleTimer = _idleResetTimer;
            }
        }
    }
}
