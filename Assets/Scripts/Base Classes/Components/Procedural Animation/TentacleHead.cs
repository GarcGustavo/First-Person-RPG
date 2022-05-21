using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleHead : MonoBehaviour
{
    public Transform _target;
    public float _rotationSpeed;

    private Vector2 _direction;

    void Update()
    {
        _direction = _target.position - transform.position;
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, _rotationSpeed * Time.deltaTime);
    }
}
