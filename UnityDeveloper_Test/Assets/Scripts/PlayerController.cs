using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private Transform _modelTransform;
    private Transform _playerTransform;
    private Rigidbody _rigidbody;
    private Vector3 _moveDir;
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _jumpSpeed = 10f;
    [SerializeField] private float _gravityForce = -9.81f;
    [SerializeField] private float _decelerationFactor = 10f;
    private bool _isGrounded;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private Transform _groundCheck;
    private Transform mainCameraTransform;

    private Animator _animator;

    [SerializeField] Transform _hologram;
    [SerializeField] Transform _playerHeadTop;
    [SerializeField] Transform _hologramHeadTop;
    private Coroutine _hologramCoroutine;

    [SerializeField] Transform freeLookCam1;
    [SerializeField] Transform freeLookCam2;

    private bool _transition;
    private void Start()
    {
        _rigidbody = _modelTransform.GetComponent<Rigidbody>();
        _animator = _modelTransform.GetComponent<Animator>();
        _playerTransform = _modelTransform.transform;
        mainCameraTransform = Camera.main.gameObject.transform;
        _hologram.gameObject.SetActive(false);
        freeLookCam2.gameObject.SetActive(false);
    }


    private void Update()
    {
        if (_transition)
            return;
        PlayerMovementInput();

        GravityManipulation();

        ApplyGravity();
    }

    private void ApplyGravity()
    {
        _rigidbody.AddForce(_playerTransform.up * _gravityForce, ForceMode.Force);
    }

    private void PlayerMovementInput()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKeyDown(KeyCode.Space))
        {
            AlignPlayerRotationToCamera();
        }
        _isGrounded = Physics.Raycast(_groundCheck.position, -_playerTransform.up, 2f, groundLayerMask);
        _moveDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            _moveDir = _playerTransform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            _moveDir = -1f * _playerTransform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            _moveDir = -1f * _playerTransform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            _moveDir = _playerTransform.right;
        }

        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            _rigidbody.AddForce(_playerTransform.up * _jumpSpeed * Time.deltaTime, ForceMode.VelocityChange);
        }

        if (_moveDir.magnitude >= 0.1f)
        {
            _rigidbody.AddForce(_moveDir * _moveSpeed * Time.deltaTime);
        }
        else
        {
            if (Mathf.Abs(Vector3.Dot(_rigidbody.velocity, _playerTransform.up)) < 0.1)
            {
                _rigidbody.velocity -= _rigidbody.velocity * _decelerationFactor * Time.deltaTime;
                if (_rigidbody.velocity.magnitude < 0.1)
                {
                    _rigidbody.velocity = Vector3.zero;
                }
            }
        }

        _animator.SetFloat("horizontalSpeed", _moveDir.magnitude);

        _animator.SetBool("isFalling", !_isGrounded);
    }

    private void AlignPlayerRotationToCamera()
    {
        RotatePlayer(Quaternion.LookRotation(Vector3.Cross(mainCameraTransform.right, _playerTransform.up), _playerTransform.up));
    }

    private void GravityManipulation()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ShowHologram(HologramPosition.Forward);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ShowHologram(HologramPosition.Left);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ShowHologram(HologramPosition.Backward);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ShowHologram(HologramPosition.Right);
        }
        if (Input.GetKeyDown(KeyCode.Return) && _hologram.gameObject.activeInHierarchy)
        {
            StartCoroutine(TeleportPlayerTo(_hologram));
        }
    }
    private IEnumerator TeleportPlayerTo(Transform moveTo)
    {
        _transition = true;
        StopCoroutine(_hologramCoroutine);
        _hologram.gameObject.SetActive(false);
        _playerTransform.position = _playerHeadTop.position;
        RotatePlayer(moveTo.rotation);
        ChangeCamera();
        yield return new WaitForSeconds(1f);
        _transition = false;
    }

    private void ShowHologram(HologramPosition hologramPosition)
    {
        _hologram.transform.position = _playerHeadTop.position;

        switch (hologramPosition)
        {
            case HologramPosition.Forward:
                _hologram.transform.rotation = Quaternion.LookRotation(_playerTransform.up, -_playerTransform.forward);
                break;
            case HologramPosition.Backward:
                _hologram.transform.rotation = Quaternion.LookRotation(-_playerTransform.up, _playerTransform.forward);
                break;
            case HologramPosition.Left:
                _hologram.transform.rotation = Quaternion.LookRotation(_playerTransform.forward, _playerTransform.right);
                break;
            case HologramPosition.Right:
                _hologram.transform.rotation = Quaternion.LookRotation(_playerTransform.forward, -_playerTransform.right);
                break;
        }



        _hologram.transform.position += _playerHeadTop.position - _hologramHeadTop.position + _playerHeadTop.up * 0.1f;

        _hologram.gameObject.SetActive(true);
        if (_hologramCoroutine != null)
        {
            StopCoroutine(_hologramCoroutine);
        }
        _hologramCoroutine = StartCoroutine(HideHologram());
    }
    IEnumerator HideHologram()
    {
        yield return new WaitForSeconds(5f);
        if (_hologram.gameObject.activeInHierarchy)
        {
            _hologram.gameObject.SetActive(false);
        }
    }

    public bool CheckGameOver()
    {
        float verticalVelocity = Vector3.Dot(_rigidbody.velocity, _playerTransform.up);
        return Mathf.Abs(verticalVelocity) > 25f;
    }

    private void ChangeCamera()
    {
        if (!freeLookCam2.gameObject.activeInHierarchy)
        {
            freeLookCam2.gameObject.SetActive(true);
            freeLookCam1.gameObject.SetActive(false);
        }
        else if (!freeLookCam1.gameObject.activeInHierarchy)
        {
            freeLookCam1.gameObject.SetActive(true);
            freeLookCam2.gameObject.SetActive(false);
        }
    }

    public void RotatePlayer(Quaternion rotation)
    {
        _transition = true;
        _playerTransform.rotation = rotation;
        ChangeCamera();
        _transition = false;
    }
}

public enum HologramPosition
{
    Forward, Backward, Left, Right
}
