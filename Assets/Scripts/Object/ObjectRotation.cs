using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ObjectRotation : MonoBehaviour
{
    [SerializeField] private bool isCollectable;
    private float _originY;
    private float _targetY;
    public enum SpinType
    {
        xAxis,
        yAxis,
        zAxis
    }
    public SpinType spinType;

    [SerializeField] float rotationSpeed;

    // Update is called once per frame
    private void Start()
    {
        if (isCollectable)
        {
            _originY = transform.localPosition.y;
            _targetY = _originY + 0.5f;
            UpAndDown();
        }
    }

    void Update()
    {
        if(spinType == SpinType.yAxis)
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        if (spinType == SpinType.zAxis)
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void UpAndDown()
    {
        transform.DOMoveY(_originY, _targetY).OnComplete(DownAndUp);
    }
    private void DownAndUp()
    {
        transform.DOMoveY(_targetY, _originY).OnComplete(UpAndDown);
    }
}
