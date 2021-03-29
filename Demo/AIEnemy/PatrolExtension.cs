using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolExtension : MonoBehaviour
{
    public TextMesh textMesh;
    public List<string> texts = new List<string>();
    Coroutine coroutine;

    void Start()
    {
        GetComponent<PatrolAction>().onRefindTarget += OnRefindTarget;
    }

    private void OnRefindTarget()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        textMesh.text = texts[Random.Range(0, texts.Count)];
        coroutine = StartCoroutine(ClearText());
    }

    IEnumerator ClearText()
    {
        yield return new WaitForSeconds(2);
        textMesh.text = "";
    }
}
