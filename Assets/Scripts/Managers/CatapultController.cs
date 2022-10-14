using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CatapultController : MonoBehaviour
{
    public UnityEvent FinishGame;

    [SerializeField] private Transform hand;
    [SerializeField] private Transform finishCatapult;
    [SerializeField] private GameObject indicator;
    [SerializeField] private Image slideBarImage;


    [Header("Силла Катапульты")]
    [SerializeField] private float greenHandForce;
    [SerializeField] private float yellowHandForce;
    [SerializeField] private float redHandForce;

    private float deltaTrack; // Кусок вссего трэка их 9


    private Vector3 startSwipePos;
    private Vector3 endSwipePos;
    private Vector3 startHandPos;

    private Rigidbody rbHand;

    private SlideBarData slideData;

    private void Update()
    {
        InputSwipe();

    }


    private IEnumerator MoveIndicator()
    {
        while (true)
        {
            slideData.Update();
            var restDistance = Vector3.Distance(hand.position, finishCatapult.position);
            slideData.TargetPoints = slideData.slideBarDiapason - (restDistance * slideData.slideBarDiapason / slideData.slideBarDiapason);
            slideBarImage.fillAmount = slideData.GetBarNormalized();
            yield return null;
        }
    }

    public void EnableScript()
    {
        indicator.SetActive(true);
        indicator.GetComponent<Animation>().Play("Up");
        rbHand = hand.GetComponent<Rigidbody>();
        startHandPos = hand.position;
        deltaTrack = Vector3.Distance(hand.position, finishCatapult.position) / 9;
        slideData = new SlideBarData(deltaTrack * 9);

        FinishGame.AddListener(DisableScript);
        this.enabled = true;
        StartCoroutine(MoveIndicator());
    }
    public void DisableScript()
    {
        StopAllCoroutines();
      //  slideBarImage.fillAmount = 1;
        indicator.GetComponent<Animation>().Play("Down");
        this.enabled = false;
    }

    public void InputSwipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startSwipePos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            endSwipePos = Input.mousePosition;
            Vector2 distance = endSwipePos - startSwipePos;
            if ((distance.y * -1) > (Screen.height / 7)) // height/4 - четверть экрана нужно провести пальцем чтоб двигатель начал обороты
            {
                float forceCatapult = PushForceControl(hand.position, finishCatapult.position,deltaTrack, redHandForce, yellowHandForce, greenHandForce);

                rbHand.AddForce(Vector3.forward * forceCatapult, ForceMode.Impulse);
                if (forceCatapult == 0)
                {
                    FinishGame?.Invoke();
                }
            }
        }
        else
        {
            hand.transform.position = Vector3.Lerp(hand.transform.position, startHandPos,0.05f * Time.deltaTime);
        }


    }

    private float PushForceControl(Vector3 currentHand, Vector3 finishHand, float delta, float redForce, float yellowForce, float greenForce)
    {
       var restDistance = Vector3.Distance(currentHand, finishHand);
        float force;

        if (restDistance > delta * 5) // REDZONE
        {
            force = redForce;
        }
        else if(restDistance > delta * 3) // YELLOW ZONE
        {
            force = yellowForce;
        }
        else if(restDistance > delta * 2)// GREENZONE
        {

            force = greenForce;
        }
        else
        {
            force = 0;
        }
        return force;
    }


}
