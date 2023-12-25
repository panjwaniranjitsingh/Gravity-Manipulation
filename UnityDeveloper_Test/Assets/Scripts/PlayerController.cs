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

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _hologram.gameObject.SetActive(false);
    }

    
    private void Update()
    {
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
        isGrounded = Physics.Raycast(_groundCheck.position, -transform.up, 0.1f, groundLayerMask);
        float horizontalVelocity = 0;
        if (Input.GetKey(KeyCode.W))
        {
            _rigidbody.AddForce(transform.forward * _moveSpeed * Time.deltaTime,ForceMode.VelocityChange);
            horizontalVelocity = Vector3.Dot(_rigidbody.velocity, transform.forward);

        }
        if (Input.GetKey(KeyCode.S))
        {
            _rigidbody.AddForce(_moveSpeed * -1f * transform.forward * Time.deltaTime, ForceMode.VelocityChange);
            horizontalVelocity = Vector3.Dot(_rigidbody.velocity, -transform.forward);
        }
        if (Input.GetKey(KeyCode.A))
        {
            _rigidbody.AddForce(_moveSpeed * -1f * transform.right * Time.deltaTime, ForceMode.VelocityChange);
            horizontalVelocity = Vector3.Dot(_rigidbody.velocity, -transform.right);
        }
        if (Input.GetKey(KeyCode.D))
        {
            _rigidbody.AddForce(transform.right * _moveSpeed * Time.deltaTime, ForceMode.VelocityChange);
            horizontalVelocity = Vector3.Dot(_rigidbody.velocity, transform.right);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            _rigidbody.AddForce(transform.up * _jumpSpeed * Time.deltaTime, ForceMode.VelocityChange);
        }

        _animator.SetFloat("horizontalSpeed", horizontalVelocity);

        _animator.SetBool("isFalling", !isGrounded);
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
            TeleportPlayerTo(_hologram);
        }
    }
    private void TeleportPlayerTo(Transform moveTo)
    {

        transform.position = moveTo.position;
        transform.rotation = moveTo.rotation;
        _hologram.gameObject.SetActive(false);
    }

    private void ShowHologram(float x, float z)
    {
        _hologram.transform.position = _playerHeadTop.position;

        _hologram.rotation = transform.rotation * Quaternion.Euler(x, 0, z);

        _hologram.transform.position += _playerHeadTop.position - _hologramHeadTop.position;
        _hologram.gameObject.SetActive(true);
        StartCoroutine(HideHologram());
    }
    IEnumerator HideHologram()
    {
        yield return new WaitForSeconds(1f);
        if(_hologram.gameObject.activeInHierarchy)
        {
            _hologram.gameObject.SetActive(false);
        }
    }

    public float GetVerticalVelocity()
    {
        return Vector3.Dot(_rigidbody.velocity, transform.up);
    }
}
