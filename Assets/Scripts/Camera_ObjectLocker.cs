using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class Camera_ObjectLocker : MonoBehaviour {

    public bool UseChildrenAverageLocation;

    Vector3 TempLocation;
    int ChildCount;

    public GameObject ObjectToLockOn;


	public void Lock () {

        if (UseChildrenAverageLocation)
        {
            foreach (Transform Child in ObjectToLockOn.transform)
            {
                TempLocation += Child.position;
                ChildCount ++;
            }
            TempLocation /= ChildCount;
            transform.position = TempLocation;
        }

        else if (ObjectToLockOn.GetComponent<Renderer>())
        {
            transform.position = ObjectToLockOn.GetComponent<Renderer>().bounds.center;
        }

        else
        {
            transform.position = ObjectToLockOn.transform.position; 
        }

        
		
	}

}
