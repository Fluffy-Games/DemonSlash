using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotation : MonoBehaviour
{
    public enum SpinType
    {
        xAxis,
        yAxis,
        zAxis
    }
    public SpinType spinType;

    [SerializeField] float rotationSpeed;

    // Update is called once per frame
    void Update()
    {
        if(spinType == SpinType.yAxis)
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        if (spinType == SpinType.zAxis)
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
