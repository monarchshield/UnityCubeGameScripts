using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // Start is called before the first frame update

    private GameManager m_gamemanager; //This is to get a reference to the player turn
    private Rigidbody m_rigidbody; //This is to enable and disable physics on this object
    private bool m_traversed; //This is to see if this node has already been scored
    
    public int m_layernumber; //This is used for the verticallity
    public int m_magicnumbervalue; //This is the magic number value;


    public GameObject[] m_playermoves;
   

    void Start()
    {
        m_gamemanager = GameObject.Find("GameManager").GetComponent<GameManager>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_traversed = false;
      
        m_playermoves = GameObject.FindGameObjectsWithTag(gameObject.tag); //Return all other moves

        foreach (GameObject playermove in m_playermoves)
        {
            playermove.GetComponent<PlayerMove>().SetTraversed(false);
        }


    }

    

    void Update()
    {
        
    }


    // Update is called once per frame
    private void OnCollisionEnter(Collision collision)
    {
        m_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        m_rigidbody.isKinematic = true;
        m_layernumber = (int)Mathf.Floor(transform.position.y);
        

        RayCastCheckPoints();
    }

    private void RayCastCheckPoints()
    {
        /* Pseudocode:
         * 1) Loop through every block with tag matching the gameobjects own
         * 2) Raycast in the direction of each block to this block.
         * 3) Check all hits
         * 4) Make sure the blocks cant be retraversed or double points may occur
         * 5) Update Game score for player moves
         */
        int i = 0;
        Color testing = Color.red;
        foreach (GameObject playermove in m_playermoves)
        {
            if (i == 1) { testing = Color.blue; }
            if (i == 2) { testing = Color.green; }




            Vector3 Direction = transform.position - playermove.transform.position;
            float Angle = Vector3.Angle(Direction, transform.forward);
            Direction.Normalize();

            Debug.DrawRay(playermove.transform.position, Direction * 50, testing, 10f);
            i++;
            RaycastHit[] hits;
            hits = Physics.RaycastAll(playermove.transform.position, Direction * 50, 50,9);

           if(hits.Length >= 2)
           {
                PlayerMove playerobj1 = hits[0].transform.gameObject.GetComponent<PlayerMove>();
                PlayerMove playerobj2 = hits[1].transform.gameObject.GetComponent<PlayerMove>();
                PlayerMove loopcomponent = playermove.GetComponent<PlayerMove>();

                if (hits[0].transform.tag.Equals(transform.tag) && !playerobj1.Traversed() &&
                    hits[1].transform.tag.Equals(transform.tag) && !playerobj2.Traversed())
                {

                    Debug.Log("Raycast Angle:" + Angle);
                    if (isRayCastAngleRecursive(Angle) && GridCheckMagicNumber(loopcomponent, playerobj1,playerobj2))
                    {

                        loopcomponent.SetTraversed(true);
                        playerobj1.SetTraversed(true);
                        playerobj2.SetTraversed(true);
                        
                        if (transform.tag == "Player1Move") { m_gamemanager.IncrimentPlayerOneScore(); Debug.DrawRay(transform.position, Direction * 30, Color.red , 600f); }
                        else { m_gamemanager.IncrimentPlayerTwoScore(); Debug.DrawRay(transform.position, Direction * 30, Color.blue, 600f); }

                    }
                }

           }
            SetTraversed(false);
        }
    }


    /* Depreciated */
    #region DepreciatedRayCastAngleAccurate
    public bool isRayCastAngleAccurate(float angle)
    {
        bool Raycastaccurate;

        switch (angle)
        {
            case float i when i > -5 && i < 5:
                Raycastaccurate = true;
                break;

            case float i when i > 35 && i < 55:
                Raycastaccurate = true;
                break;

            case float i when i > 85 && i < 95:
                Raycastaccurate = true;
                break;

            case float i when i > 125 && i < 140:
                Raycastaccurate = true;
                break;

            case float i when i > 170 && i < 190:
                Raycastaccurate = true;
                break;

            default:
                Raycastaccurate = false;
                break;
        }

        return Raycastaccurate;
    }
    #endregion

    public bool isRayCastAngleRecursive(float angle)
    {
        while(angle > 10)
            angle = angle - 45;


        if (angle < 10 || angle > -10) { return true; }
       

        return false;

    }


    /*Depreciated */
    #region DepreciatedAwfulGridChicks
    //Checks to see whether all the objects are on the same y axis or not
    private bool VerticalityCheck(PlayerMove p1, PlayerMove p2)
    {
        bool VerticalityCheck = false;

        /*This is a 2d grid reference [X,X,X
                                       X,X,X
                                       X,X,X]

        */
        if(m_layernumber.Equals(p1.m_layernumber) && p1.m_layernumber.Equals(p2.m_layernumber))
        {
            VerticalityCheck = true;
        }

        
        if(!m_layernumber.Equals(p1.m_layernumber) && !p1.m_layernumber.Equals(p2.m_layernumber))
        {
            VerticalityCheck = GridCheck(p1,p2);
        }


        return VerticalityCheck;
    }

    //This is an outlier, to see if the objects  are either all on the same x,z axis or there not
    //Replace with magic square.
    private bool GridCheck(PlayerMove p1, PlayerMove p2)
    {

        //Check that the object are either both on the same row and tile
        if(transform.position.x.Equals(p1.transform.position.x) && p1.transform.position.x.Equals(p2.transform.position.x) && p2.transform.position.x.Equals(transform.position.x) &&
           transform.position.z.Equals(p1.transform.position.z) && p1.transform.position.z.Equals(p2.transform.position.z) && p2.transform.position.z.Equals(transform.position.z) )
        {
            return true;
        }


        //Or Only Only on the same row in one axis
        if (!transform.position.x.Equals(p1.transform.position.x) && !p1.transform.position.x.Equals(p2.transform.position.x) && !p2.transform.position.x.Equals(transform.position.x)
          && transform.position.z.Equals(p1.transform.position.z) && p1.transform.position.z.Equals(p2.transform.position.z) && p2.transform.position.z.Equals(transform.position.z) ||

          transform.position.x.Equals(p1.transform.position.x) && p1.transform.position.x.Equals(p2.transform.position.x) && p2.transform.position.x.Equals(transform.position.x)
          && !transform.position.z.Equals(p1.transform.position.z) && !p1.transform.position.z.Equals(p2.transform.position.z) && !p2.transform.position.z.Equals(transform.position.z))
        {
            return true;
        }


        //Or not on any of the same row in any axis!
        if (!transform.position.x.Equals(p1.transform.position.x) && !p1.transform.position.x.Equals(p2.transform.position.x) && !p2.transform.position.x.Equals(transform.position.x) &&
          !transform.position.z.Equals(p1.transform.position.z) && !p1.transform.position.z.Equals(p2.transform.position.z) && !p2.transform.position.z.Equals(transform.position.z))
        {
            return true;
        }

        return false;

    }
    #endregion

    public bool GridCheckMagicNumber(PlayerMove p0, PlayerMove p1, PlayerMove p2)
    {
        Debug.Log("Magic Number Values:" + p0.GetMagicNumber().ToString() + "," + p1.GetMagicNumber().ToString() + "," + p2.GetMagicNumber().ToString());

        if(p0.GetMagicNumber() + p1.GetMagicNumber() + p2.GetMagicNumber() == 15 || 
           p0.GetMagicNumber().Equals(p1.GetMagicNumber()) && p1.GetMagicNumber().Equals(p2.GetMagicNumber()))
        {
            return true;
        }
       
        return false;
    }



    public void SetMagicNumberValue(int val) { m_magicnumbervalue = val; }
    public int GetMagicNumber() { return m_magicnumbervalue; }

    public void SetTraversed(bool val) { m_traversed = val; }
    public bool Traversed() { return m_traversed; }

}
