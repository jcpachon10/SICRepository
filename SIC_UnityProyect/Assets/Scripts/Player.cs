using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //------------------------
    //VARIABLES
    //------------------------
    //left view
    public SnapShot SnapCam;
    //Door view
    public SnapShot SnapCam2;
    //Top view 
    public SnapShot SnapCam3;
    //Right view
    public SnapShot SnapCam4;

    //Call SnapShot of each cam
    public void TomarSnapShot()
    {

        SnapCam.callTakeSnapShot();
        SnapCam2.callTakeSnapShot();
        SnapCam3.callTakeSnapShot();
        SnapCam4.callTakeSnapShot();
    }

}
