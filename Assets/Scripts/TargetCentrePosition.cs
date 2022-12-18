using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCentrePosition : MonoBehaviour
{
    [SerializeField]
    GameObject PlayerA;
    [SerializeField] GameObject PlayerB;

    [SerializeField]
    float farzoomModifier;
    [SerializeField]
    float closezoomModifier;
    [SerializeField]
    float yOffset;


    void Update()
    {
        //Move to midpoint between players if both are alive
        if(PlayerA.GetComponent<PlayerChar>().IsAlive() && PlayerB.GetComponent<PlayerChar>().IsAlive())
        {
            //Move camera
            transform.position = Vector3.Lerp(PlayerA.transform.position, PlayerB.transform.position, 0.5f);

        }
        else if (PlayerA.GetComponent<PlayerChar>().IsAlive())
        {
            transform.position = Vector3.Lerp(PlayerA.transform.position, Vector3.zero, 0.5f);
        }
        else if (PlayerB.GetComponent<PlayerChar>().IsAlive())
        {
            transform.position = Vector3.Lerp(Vector3.zero, PlayerB.transform.position, 0.5f);
        }
        else
        {
            transform.position = Vector3.zero;
        }

        //Offset camera slightly upwards
        Vector3 yAdjustedPosition = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);
        transform.position = yAdjustedPosition;

        //Always point at one player, so dynamic camera targets pivot around the centre as it moves
        Vector3 positionToLookAt = new Vector3(PlayerA.transform.position.x, transform.position.y, PlayerA.transform.position.z);
        transform.LookAt(positionToLookAt);

        //Zoom camera in when players are closer and zoom out, etc
        float playerDistance = Mathf.Abs((PlayerA.transform.position - PlayerB.transform.position).magnitude);
        float camZoom = Mathf.Lerp(closezoomModifier, farzoomModifier, Mathf.InverseLerp(0.5f, 12f, playerDistance));
        Vector3 camZoomVector = new Vector3(camZoom, camZoom, camZoom);
        transform.localScale = camZoomVector;
    }

}


