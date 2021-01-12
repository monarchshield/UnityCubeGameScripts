using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update

    //The canvas object itself
    public GameObject CanvasObject;
    public GameObject UIObject;
    
    private int m_playerTurn; //Player 1 or Player 2

    //Player ones score
    private int m_playeronescore;

    //Player twos score
    private int m_playertwoscore;

    //Rotation for the canvas
    public Vector3 targetRotationCanvas;

    //Rotation for the UI
    public Vector3 TargetRotationUI;

    //Is anything rotating? great dont allow input
    public bool isRotating;

    IEnumerator LerpFunctionCanvas(Quaternion endValue, float duration)
    {
        float time = 0;
        Quaternion startValue = CanvasObject.transform.rotation;

        isRotating = true;

        

        while (time < duration)
        {
            CanvasObject.transform.rotation = Quaternion.Lerp(startValue, endValue, time / duration);
           
            time += Time.deltaTime;
            yield return null;
        }
        CanvasObject.transform.rotation = endValue;
        isRotating = false;

    }

    IEnumerator LerpFunctionUI(Quaternion endValue, float duration)
    {
        float time = 0;
        Quaternion startValue = UIObject.transform.rotation;

        isRotating = true;

        while (time < duration)
        {
        
            UIObject.transform.rotation = Quaternion.Lerp(startValue, endValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        UIObject.transform.rotation = endValue;
        isRotating = false;

    }



    void Start()
    {
        targetRotationCanvas = CanvasObject.transform.rotation.eulerAngles;
        TargetRotationUI = UIObject.transform.rotation.eulerAngles;

        isRotating = false;
       
    }

    // Update is called once per frame

    public void RotationButtonClicked()
    {
        if (!isRotating)
        {

            targetRotationCanvas.y += 90;
            TargetRotationUI.z -= 90;
            StartCoroutine(LerpFunctionCanvas(Quaternion.Euler(targetRotationCanvas), 1));
            StartCoroutine(LerpFunctionUI(Quaternion.Euler(TargetRotationUI), 1));
        }
    }

    public void RotateCanvasByQuarter()
    {
        CanvasObject.transform.Rotate(Vector2.up, 90);
        isRotating = true;
    }

    public int GetPlayerTurn() { return m_playerTurn; }
    public void SetPlayerTurn(int playernum) { m_playerTurn = playernum; }

    public void IncrimentPlayerOneScore() { m_playeronescore += 1; Debug.Log("Wooohoo! Point awarded for player 1");  }
    public void IncrimentPlayerTwoScore() { m_playertwoscore += 1; Debug.Log("Wooohoo! Point awarded for player 2"); }

}
