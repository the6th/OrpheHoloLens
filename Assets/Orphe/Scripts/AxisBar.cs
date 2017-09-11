using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisBar : MonoBehaviour {

    public enum AxisType { X,Y,Z};
    public AxisType axis;
    public float val = 10;
    Material mat;

    private Vector3 unitAxis;
    private Vector3 rotAxis;
    // Use this for initialization
    void Start () {

        mat = GetComponent<MeshRenderer>().material;
        switch (axis)
        {
            case AxisType.X:
                mat.SetColor("_Color", Color.red);
                rotAxis = Vector3.up.normalized;
                unitAxis = Vector3.up.normalized;
               
                break;
            case AxisType.Y:
                mat.SetColor("_Color", Color.green);
                rotAxis = Vector3.right.normalized;
                unitAxis = Vector3.forward.normalized;

                break;
            case AxisType.Z:
                mat.SetColor("_Color", Color.blue);
                rotAxis = Vector3.forward.normalized;
                unitAxis = Vector3.right.normalized;



                break;
            default:
                break;
                
        }
        transform.localEulerAngles = rotAxis * -90f;

    }
	
	// Update is called once per frame
	void Update () {
        transform.localPosition = unitAxis * val / 2;
        transform.localScale = new Vector3(0.1f, val, 0.1f);
	}
}
