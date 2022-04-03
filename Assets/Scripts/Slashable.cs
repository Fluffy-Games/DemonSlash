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
    
    public bool oneSlash;
    public bool finalEnemy;
    public ColorType colorType;

    private void OnEnable()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        oneSlash = false;
    }

    public void Slash()
    {
        int x = Random.Range(0, 3);
        transform.GetChild(0).gameObject.SetActive(false);
        StartCoroutine(SlashRout(cutPieces[x]));
    }

    private IEnumerator SlashRout(GameObject go)
    {
        go.SetActive(true);
        Transform go1 = go.transform.GetChild(0);
        Transform go2 = go.transform.GetChild(1);
        Material mat1 = go1.gameObject.GetComponent<MeshRenderer>().material;
        Material mat2 = go2.gameObject.GetComponent<MeshRenderer>().material;
        float timer = 0f;

        float x1 = Random.Range(-0, 5);
        float x2 = Random.Range(-5, 0);
        float y1 = Random.Range(-2, 2);
        float y2 = Random.Range(-2, 2);
        Vector3 target1 = cutTargets[0].localPosition + (Vector3.right * x1) + (Vector3.up * y1);
        Vector3 target2 = cutTargets[1].localPosition + (Vector3.right * x2) + (Vector3.up * y2);
        go1.DOLocalJump(target1, 2, 1, .75f);
        go2.DOLocalJump(target2, 2, 1, .75f);
        while (true)
        {
            timer += Time.deltaTime * 1.5f;

            //go1.localPosition = Vector3.Lerp(go1.localPosition, cutTargets[0].localPosition, timer);
            //go2.localPosition = Vector3.Lerp(go2.localPosition, cutTargets[1].localPosition, timer);
            mat1.SetFloat("_Dissolve", Mathf.Lerp(0, 1, timer));
            mat2.SetFloat("_Dissolve", Mathf.Lerp(0, 1, timer));
            yield return null;
            if (timer >= 1f)
            {
                break;
            }
        }
        go.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            transform.GetChild(0).gameObject.SetActive(false);
            StartCoroutine(SlashRout(cutPieces[0]));
        }
    }
}
