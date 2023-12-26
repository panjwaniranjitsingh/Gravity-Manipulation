using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rigidbody;
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _jumpSpeed = 10f;
    [SerializeField] private float _gravityForce = -9.81f;
    private bool isGrounded;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private Transform _groundCheck;

    private Animator _animator;
    [SerializeField] Transform _hologram;
    [SerializeField] Transform _playerHeadTop;
    [SerializeField] Transform _hologramHeadTop;

    [SerializeField] Transform freeLookCam1;
    [SerializeField] Transform freeLookCam2;
    private Vector3 moveDir;

    private bool transition;
    private Coroutine hologramCoroutine;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _hologram.gameObject.SetActive(false);
        freeLookCam2.gameObject.SetActive(false);
    }


    private void Update()
    {
        if (transition)
            return;
        PlayerMovementInput();

        GravityManipulation();

        ApplyGravity();
    }

    private void ApplyGravity()
    {
        _rigidbody.AddForce(transform.up * _gravityForce, ForceMode.Force);
    }

    private void PlayerMovementInput()
    {
        if (Input.anyKeyDown)
        {
            AlignPlayerRotationToCamera();
        }
        isGrounded = Physics.Raycast(_groundCheck.position, -transform.up, 0.1f, groundLayerMask);
        moveDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            moveDir = transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDir = -1f * transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDir = -1f * transform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDir = transform.right;
        }
        if (moveDir.magnitude >= 0.1f)
        {
            _rigidbody.AddForce(moveDir * _moveSpeed * Time.deltaTime);
        }
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            _rigidbody.AddForce(transform.up * _jumpSpeed * Time.deltaTime, ForceMode.VelocityChange);
        }

        _animator.SetFloat("horizontalSpeed", moveDir.magnitude);

        _animator.SetBool("isFalling", !isGrounded);
    }

    private void AlignPlayerRotationToCamera()
    {
        Transform cameraForward = Camera.main.gameObject.transform;
        Debug.Log("Transform.up=" + transform.up);
        if (Mathf.Abs(transform.up.x) > 0.95)
        {
            transform.eulerAngles = new Vector3(cameraForward.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
        }
        else if (Mathf.Abs(transform.up.y) > 0.95)
        {
            Debug.Log(cameraForward.eulerAngles.y);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, cameraForward.eulerAngles.y, transform.eulerAngles.z);
        }
        else
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, cameraForward.eulerAngles.z);
        }


    }

    private void GravityManipulation()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ShowHologram(-90, 0);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ShowHologram(0, -90);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ShowHologram(90, 0);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ShowHologram(0, 90);
        }
        if (Input.GetKeyDown(KeyCode.Return) && _hologram.gameObject.activeInHierarchy)
        {
            StartCoroutine(TeleportPlayerTo(_hologram));
        }
    }
    private IEnumerator TeleportPlayerTo(Transform moveTo)
    {
        transition = true;
        StopCoroutine(hologramCoroutine);
        _hologram.gameObject.SetActive(false);
        transform.position = moveTo.position;
        RotatePlayer(moveTo.rotation);
        yield return new WaitForSeconds(1f);
        ChangeCamera();
        yield return new WaitForSeconds(1f);
        transition = false;
    }

    private void ShowHologram(float x, float z)
    {
        _hologram.transform.position = _playerHeadTop.position;

        _hologram.rotation = transform.rotation * Quaternion.Euler(x, 0, z);

        _hologram.transform.position += _playerHeadTop.position - _hologramHeadTop.position;
        _hologram.gameObject.SetActive(true);
        hologramCoroutine = StartCoroutine(HideHologram());
    }
    IEnumerator HideHologram()
    {
        yield return new WaitForSeconds(1f);
        if (_hologram.gameObject.activeInHierarchy)
        {
            _hologram.gameObject.SetActive(false);
        }
    }

    public bool CheckGameOver()
    {
        float verticalVelocity = Vector3.Dot(_rigidbody.velocity, transform.up);
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

    private void RotatePlayer(Quaternion rotation)
    {
        transform.rotation = rotation;
    }
}
