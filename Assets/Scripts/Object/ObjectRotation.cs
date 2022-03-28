using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ObjectRotation : MonoBehaviour
{
    [SerializeField] private bool isCollectable;
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
        transform.DOMoveY(0.6f, 0.5f).OnComplete(DownAndUp);
    }
    private void DownAndUp()
    {
        transform.DOMoveY(0.3f, 0.5f).OnComplete(UpAndDown);
    }
}
