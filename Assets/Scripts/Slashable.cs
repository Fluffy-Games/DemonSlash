using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slashable : MonoBehaviour
{
    [SerializeField] private List<GameObject> cutPieces;
    [SerializeField] private List<Transform> cutTargets;
    
    public bool oneSlash;
    public ColorType colorType;

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

        while (true)
        {
            timer += Time.deltaTime;

            go1.localPosition = Vector3.Lerp(go1.localPosition, cutTargets[0].localPosition, timer);
            go2.localPosition = Vector3.Lerp(go2.localPosition, cutTargets[1].localPosition, timer);
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
}
