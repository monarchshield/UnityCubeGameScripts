using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject m_player1Prefab; //This is the object that the spawner will create for player 1
    public GameObject m_player2Prefab; //This is the object the spawner will create for player 2. 

    public int m_magicvalue; //The associated magic value number for all player moves.
    private GameObject m_playermoves;

    private List<GameObject> m_blockobjects; //This is the amount of objects the spawner has
    private int m_maxobjects; //This is the max amount of objects a spawner can have created
    private GameManager m_gamemanager; //This is to get a reference to the player turn

    private bool m_spawnerdisabled; //For 1 second to allow blocks to drop


    // Start is called before the first frame update
    void Start()
    {
        m_gamemanager = GameObject.Find("GameManager").GetComponent<GameManager>();
        m_playermoves = GameObject.Find("PlayerMoves");
        m_spawnerdisabled = false;


        m_maxobjects = 3;
        m_blockobjects = new List<GameObject>();
    }

    // Update is called once per frame
    public void SpawnBlockClick()
    {
        StartCoroutine(SpawnBlock());
    }

    IEnumerator SpawnBlock()
    {

        if (m_blockobjects.Count <= m_maxobjects - 1 && !m_spawnerdisabled)
        {
            m_spawnerdisabled = true;

            if (m_gamemanager.GetPlayerTurn() == 1)
            {

                GameObject Player1TurnObject = Instantiate(m_player1Prefab, transform.position, Quaternion.identity);
                Player1TurnObject.transform.SetParent(m_playermoves.transform);
                Player1TurnObject.GetComponent<PlayerMove>().SetMagicNumberValue(m_magicvalue);

                m_blockobjects.Add(Player1TurnObject);

                m_gamemanager.SetPlayerTurn(2);
            }

            else
            {
                GameObject Player2TurnObject = Instantiate(m_player2Prefab, transform.position, Quaternion.identity);
                Player2TurnObject.transform.SetParent(m_playermoves.transform);
                Player2TurnObject.GetComponent<PlayerMove>().SetMagicNumberValue(m_magicvalue);

                m_blockobjects.Add(Player2TurnObject);
                m_gamemanager.SetPlayerTurn(1);
            }

            yield return new WaitForSeconds(1);
            m_spawnerdisabled = false;


        }

        //Print the time of when the function is first called.
        //Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(5);

        //After we have waited 5 seconds print the time again.
        //Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }



}
