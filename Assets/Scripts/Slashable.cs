using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Slashable : MonoBehaviour
{
    [SerializeField] private List<GameObject> cutPieces;
    [SerializeField] private List<Transform> cutTargets;
    private Vector3 _originPos = new Vector3(0, 1.77f, 2.82f);
    
    public bool oneSlash;
    public bool finalEnemy;
    public ColorType colorType;

    private GameObject _go1;
    private GameObject _go2;
    
    private void OnEnable()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        ResetModel();
        oneSlash = false;
    }

    private void OnDisable()
    {
        if (!_go1) return;
        _go1.SetActive(true);
        _go2.SetActive(true);
    }

    public void Slash()
    {
        int x = Random.Range(0, 3);
        transform.GetChild(0).gameObject.SetActive(false);
        StartCoroutine(SlashRout(cutPieces[x]));
    }

    private void ResetModel()
    {
        foreach (var item in cutPieces)
        {
            item.transform.GetChild(0).localPosition = _originPos;
            item.transform.GetChild(1).localPosition = _originPos;
        }
    }

    private IEnumerator SlashRout(GameObject go)
    {
        go.SetActive(true);
        Transform go1 = go.transform.GetChild(0);
        Transform go2 = go.transform.GetChild(1);
        _go1 = go1.gameObject;
        _go2 = go2.gameObject;
        //Material mat1 = go1.gameObject.GetComponent<MeshRenderer>().material;
        //Material mat2 = go2.gameObject.GetComponent<MeshRenderer>().material;
        //float timer = 0f;

        float x1 = Random.Range(-0, 5);
        float x2 = Random.Range(-5, 0);
        float y1 = Random.Range(-2, 2);
        float y2 = Random.Range(-2, 2);
        Vector3 target1 = cutTargets[0].localPosition + (Vector3.right * x1) + (Vector3.up * y1) + Vector3.forward * 5f;
        Vector3 target2 = cutTargets[1].localPosition + (Vector3.right * x2) + (Vector3.up * y2) + Vector3.forward * 5f;
        go1.DOLocalJump(target1, 2, 1, .75f);
        go2.DOLocalJump(target2, 2, 1, .75f);
        /*while (true)
        {
            timer += Time.deltaTime * 1f;

            //go1.localPosition = Vector3.Lerp(go1.localPosition, cutTargets[0].localPosition, timer);
            //go2.localPosition = Vector3.Lerp(go2.localPosition, cutTargets[1].localPosition, timer);
            //mat1.SetFloat("_Dissolve", Mathf.Lerp(0, 1, timer));
            //mat2.SetFloat("_Dissolve", Mathf.Lerp(0, 1, timer));
            yield return null;
            if (timer >= 1f)
            {
                break;
            }
        }*/
        yield return new WaitForSeconds(0.65f);
        go.SetActive(false);
        _go1.SetActive(false);
        _go2.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            transform.GetChild(0).gameObject.SetActive(false);
            StartCoroutine(SlashRout(cutPieces[0]));
            PlayerController.Instance.UpdateDiamondMultiplier(1 + (transform.parent.GetSiblingIndex() * 0.2f));
        }

        /*if (other.CompareTag("Player"))
        {
            Slash();
        }*/
    }
}
