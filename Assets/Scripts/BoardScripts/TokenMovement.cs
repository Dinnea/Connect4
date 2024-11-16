using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenMovement : MonoBehaviour
{
    Vector3 _targetLocation;
    Vector3 _startLocation;

    float _totalDistance;

    float startTime;

    [SerializeField] float _speed = 5f;
    // Start is called before the first frame update
    void Awake()
    {
        _startLocation = transform.position;
    }
    private void Start()
    {
        startTime = Time.time;

        _totalDistance = Vector3.Distance(_startLocation, _targetLocation);
    }
    // Update is called once per frame
    void Update()
    {

        MoveTowards();

        //SmoothMoveTowards();
        
    }

    public void SetTargetLocation(Vector3 targetLocation)
    {
        _targetLocation = targetLocation;
    }

    public void MoveTowards()
    {
        float step = _speed * Time.deltaTime;
        if (_targetLocation != null) transform.position = Vector3.MoveTowards(transform.position, _targetLocation, step);
    }

    public void SmoothMoveTowards()
    {
        float distCovered = (Time.time - startTime) * _speed;
        float fractionCovered = distCovered / _totalDistance;


        if (_targetLocation != null) transform.position = Vector3.Lerp(_startLocation, _targetLocation, fractionCovered);
    }
}
