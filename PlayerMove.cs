using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CameraShake;

public class PlayerMove : MonoBehaviour
{
    // Start is called before the first frame update

    private GameManager m_gamemanager; //This is to get a reference to the player turn
    private Rigidbody m_rigidbody; //This is to enable and disable physics on this object
    private bool m_traversed; //This is to see if this node has already been scored
    private bool m_alreadyscored; //This is set to false;

    public int m_layernumber; //This is used for the verticallity
    public int m_magicnumbervalue; //This is the magic number value;


    public GameObject[] m_playermoves;
   

    void Start()
    {
        m_gamemanager = GameObject.Find("GameManager").GetComponent<GameManager>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_traversed = false;
        m_alreadyscored = false;
      
        m_playermoves = GameObject.FindGameObjectsWithTag(gameObject.tag); //Return all other moves

        foreach (GameObject playermove in m_playermoves)
        {
            playermove.GetComponent<PlayerMove>().SetTraversed(false);
        }
        CameraShaker.Presets.ShortShake2D();
    }

    

    void Update()
    {
        
    }


    // Update is called once per frame
    private void OnCollisionEnter(Collision collision)
    {
        m_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        m_rigidbody.isKinematic = false;
        m_layernumber = (int)Mathf.Floor(transform.position.y);
       
        if(!m_alreadyscored)
        {
            RayCastCheckPoints();
            m_alreadyscored = true;
        }
        
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
        
        foreach (GameObject playermove in m_playermoves)
        {
          



            Vector3 Direction = transform.position - playermove.transform.position;
            float Angle = Vector3.Angle(Direction, transform.forward);
            Direction.Normalize();

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

                 
                    if (isRayCastAngleRecursive(Angle) && GridCheckMagicNumber(loopcomponent, playerobj1,playerobj2) && VerticalityCheck(loopcomponent, playerobj1, playerobj2))
                    {

                       

                        loopcomponent.SetTraversed(true);
                        playerobj1.SetTraversed(true);
                        playerobj2.SetTraversed(true);
                        
                        if (transform.tag == "Player1Move") { m_gamemanager.IncrimentPlayerOneScore(); Debug.DrawRay(playermove.transform.position, Direction * 50, Color.red , 600f); }
                        else { m_gamemanager.IncrimentPlayerTwoScore(); Debug.DrawRay(playermove.transform.position, Direction * 50, Color.blue, 600f); }

                        hits[0].transform.gameObject.GetComponent<Animator>().SetTrigger("PointScored");
                        hits[1].transform.gameObject.GetComponent<Animator>().SetTrigger("PointScored");
                        playermove.GetComponent<Animator>().SetTrigger("PointScored");

                        CameraShaker.Presets.Explosion3D();

                    }
                }

           }
            SetTraversed(false);
        }
    }


    //Check to see if the raycast is a straight line equal to or a multiple of 45 degrees
    public bool isRayCastAngleRecursive(float angle)
    {
        while(angle > 10)
            angle = angle - 45;


        if (angle < 5 || angle > -5) { Debug.Log(angle);  return true; }
       

        
        return false;

    }


    //Calculate the score if the raycast adds up to 15 a point is awarded
    public bool GridCheckMagicNumber(PlayerMove p0, PlayerMove p1, PlayerMove p2)
    {
        //Debug.Log("Magic Number Values:" + p0.GetMagicNumber().ToString() + "," + p1.GetMagicNumber().ToString() + "," + p2.GetMagicNumber().ToString());

        if(p0.GetMagicNumber() + p1.GetMagicNumber() + p2.GetMagicNumber() == 15 || 
           p0.GetMagicNumber().Equals(p1.GetMagicNumber()) && p1.GetMagicNumber().Equals(p2.GetMagicNumber()))
        {
            return true;
        }
       
        return false;
    }

    //Check to see that all the blocks are either in the same layer or different layers. (Layer refers to the Y Axis) 
    public bool VerticalityCheck(PlayerMove p0, PlayerMove p1, PlayerMove p2)
    {
       if(p0.GetLayNumber().Equals(p1.GetLayNumber()) && p1.GetLayNumber().Equals(p2.GetLayNumber())
          || !p0.GetLayNumber().Equals(p1.GetLayNumber()) && !p1.GetLayNumber().Equals(p2.GetLayNumber()))
       {
            return true;
       }


        return false;
    }

    public int GetLayNumber() { return m_layernumber; }

    public void SetMagicNumberValue(int val) { m_magicnumbervalue = val; }
    public int GetMagicNumber() { return m_magicnumbervalue; }

    public void SetTraversed(bool val) { m_traversed = val; }
    public bool Traversed() { return m_traversed; }

}
