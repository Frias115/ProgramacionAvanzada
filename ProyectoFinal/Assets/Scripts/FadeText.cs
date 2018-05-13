using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeText : MonoBehaviour
{

    public float timer;
    void Start()
    {
        StartCoroutine("HideUnhide");
    }

    IEnumerator HideUnhide()
    {
        while (true)
        {
            yield return (new WaitForSeconds(timer));
            GetComponent<Text>().enabled = true;
            yield return (new WaitForSeconds(timer));
            GetComponent<Text>().enabled = false;
        }
    }
}
