using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekExtesion : MonoBehaviour
{
    public TextMesh textMesh;
    public List<string> texts = new List<string>();

    float interval;
    Coroutine coroutine;

    private void Start()
    {
        GetComponent<SeekAction>().onPrePerform += OnPrePerform;
        GetComponent<SeekAction>().onPerform += OnPerform;
        GetComponent<SeekAction>().onSuccess += OnSuccess;
        GetComponent<SeekAction>().onFailed += OnFailed;
    }

    private void OnFailed()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        textMesh.text = "啊啊啊啊！追不到，不追了！！";
        coroutine = StartCoroutine(ClearText());
    }

    private void OnPrePerform()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        interval = UnityEngine.Random.Range(3, 7);
        textMesh.text = "警告！！发现敌人！！！";
        coroutine = StartCoroutine(ClearText());
    }

    void OnSuccess()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        textMesh.text = "抓到了！";
        coroutine = StartCoroutine(ClearText());
    }

    void OnPerform()
    {
        interval -= Time.deltaTime;
        if (interval <= 0)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
            interval = Random.Range(3, 7);
            textMesh.text = texts[Random.Range(0, texts.Count)];
            coroutine = StartCoroutine(ClearText());
        }
    }

    IEnumerator ClearText()
    {
        yield return new WaitForSeconds(2);
        textMesh.text = "";
    }
}
