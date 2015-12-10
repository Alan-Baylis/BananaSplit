using UnityEngine;

/// <summary>
/// Behaviour should be attached to a camera GameObject to get a FlyCam
/// </summary>
public class FlyCam : MonoBehaviour
{
    [SerializeField]
    private float _normalSpeedMax = 3f;
    [SerializeField]
    private float _fastSpeedMax = 6f;
    [SerializeField]
    private float _mouseSens = 1f;

    private Transform _trans = null;

    private bool _lookEnabled = true;
    private bool _forward = false;
    private bool _back = false;
    private bool _left = false;
    private bool _right = false;
    private bool _up = false;
    private bool _down = false;
    private bool _sprint = false;

    private void Start()
    {
        _trans = gameObject.transform;

        Cursor.visible = !_lookEnabled;
    }

    private void Update()
    {
        UpdateKeys();
		if (!Input.GetMouseButton (0)) 
		{
			UpdatePosition ();
		}
        if (_lookEnabled)
        {
            UpdateLook();
        }
    }
    
    private void UpdateKeys()
    {
        _forward = Input.GetKey(KeyCode.W);
        _back = Input.GetKey(KeyCode.S);
        _left = Input.GetKey(KeyCode.A);
        _right = Input.GetKey(KeyCode.D);
        _up = Input.GetKey(KeyCode.E);
        _down = Input.GetKey(KeyCode.Q);
        _sprint = Input.GetKey(KeyCode.LeftShift);

        bool rightClick = Input.GetMouseButton(1);

        if (rightClick && !_lookEnabled)
        {
            _lookEnabled = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (!rightClick && _lookEnabled)
        {
            _lookEnabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void UpdatePosition()
    {
        Vector3 dir = Vector3.zero;

        if (_forward && !_back)
            dir += _trans.forward;
        else if (_back && !_forward)
            dir -= _trans.forward;

        if (_left && !_right)
            dir -= _trans.right;
        else if (_right && !_left)
            dir += _trans.right;

        if (_up && !_down)
            dir += Vector3.up;
        else if (_down && !_up)
            dir -= Vector3.up;

        dir.Normalize();

        float speed = _sprint ? _fastSpeedMax : _normalSpeedMax;

        Vector3 translation = dir * speed * Time.deltaTime;

        _trans.Translate(translation, Space.World);
    }

    private void UpdateLook()
    {
        float my = Input.GetAxis("Mouse Y");

        Vector3 axis = Vector3.Cross(_trans.forward, Vector3.up);

        _trans.Rotate(axis, my * _mouseSens, Space.World);

        float mx = Input.GetAxis("Mouse X");

        _trans.Rotate(Vector3.up, mx * _mouseSens, Space.World);

    }
}
