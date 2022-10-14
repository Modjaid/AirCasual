using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableAnimation : MonoBehaviour
{
    public string animName;

    public void OnEnable()
    {
        GetComponent<Animation>().Play(animName);
    }
    public void Disable()
    {
        GetComponent<Animation>().Play("ModelDown");
        StartCoroutine(delay());

        IEnumerator delay()
        {
            yield return new WaitForSeconds(1.5f);
            gameObject.SetActive(false);
        }
    }

}
