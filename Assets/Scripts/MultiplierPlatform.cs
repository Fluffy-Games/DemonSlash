using UnityEngine;
using TMPro;
using DG.Tweening;

public class MultiplierPlatform : MonoBehaviour
{
    public Color mColor;
    [SerializeField] private Color mDefaultColor;

    [SerializeField] private TextMeshProUGUI mMultiplierText;

    private Material _mMaterial;
    private int _siblingIndex;

    private void OnValidate()
    {
        _siblingIndex = transform.GetSiblingIndex();
        mMultiplierText.text = "X" + (1.2f + (_siblingIndex * .2f)).ToString("F1");
        mMultiplierText.fontSize = 300f;
        transform.localPosition = Vector3.forward * ((_siblingIndex * 2) + 1);
        int temp1 = _siblingIndex % 36;
        int temp2 = _siblingIndex % 6;
        if (temp1 < 6)
        {
            mColor = new Vector4(0, temp2 * .2f, 1, 1);
        }

        if (temp1 >= 6 && temp1 < 12)
        {
            mColor = new Vector4(0, 1, 1 - temp2 * .2f, 1);
        }

        if (temp1 >= 12 && temp1 < 18)
        {
            mColor = new Vector4(temp2 * .2f, 1, 0, 1);
        }
        if (temp1 >= 18 && temp1 < 24)
        {
            mColor = new Vector4(1, 1 - temp2 * .2f, 0, 1);
        }

        if (temp1 >= 24 && temp1 < 30)
        {
            mColor = new Vector4(1, 0, temp2 * .2f, 1);
        }

        if (temp1 >= 30)
        {
            mColor = new Vector4(1 - temp2 * .2f, 0, 1, 1);
        }
    }
    private void OnEnable()
    {
        _mMaterial = GetComponent<MeshRenderer>().material;
        _mMaterial.color = mDefaultColor;
    }

    public void ChangeMaterial()
    {
        Transform transform1;
        (transform1 = transform).DORewind();
        transform.DOPunchScale(transform1.localScale * .12f, .21f, 5, .85f);

        _mMaterial.color = mColor;
    }
}
