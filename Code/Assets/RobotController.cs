using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class RobotController : MonoBehaviour
{
    [SerializeField] private WheelCollider FLC;
    [SerializeField] private WheelCollider FRC;
    [SerializeField] private WheelCollider RLC;
    [SerializeField] private WheelCollider RRC;

    [SerializeField] private Transform FLT;
    [SerializeField] private Transform FRT;
    [SerializeField] private Transform RLT;
    [SerializeField] private Transform RRT;

    [SerializeField] private Transform FRS;
    [SerializeField] private Transform L1S;
    [SerializeField] private Transform L2S;
    [SerializeField] private Transform L3S;
    [SerializeField] private Transform R1S;
    [SerializeField] private Transform R2S;
    [SerializeField] private Transform R3S;
    [SerializeField] private Transform ORS;


    public float sensorlimit = 5f;
    private float CMF;
    private float CSA = 0f;
    private bool completed = false;


    private void Start() {
        Rigidbody rb = GetComponent<Rigidbody>();
        GetComponent<Rigidbody>().drag = 1f;

        L1S.transform.Rotate(0, 0, -75);
        R1S.transform.Rotate(0, 0, 75);
        FRS.transform.Rotate(30, 0, 0);
      
    }
    private void FixedUpdate() {
        if (!completed) {
            ObstacleAdjust();
            StreeringControl();
            hasReachedtheEnd();
        }
    }
    private void ObstacleAdjust() {
        RaycastHit LFHIT, RGHIT;
        RaycastHit ORSHIT;
        RaycastHit L1B, L2B;
        RaycastHit R1B, R2B;

        bool BLH1= Physics.Raycast(L2S.position, L2S.forward, out L1B,15);
        bool BRH1 = Physics.Raycast(R2S.position, R2S.forward, out R1B,15);
        bool BLH2 = Physics.Raycast(L3S.position, L3S.forward, out L2B,15);
        bool BRH2 = Physics.Raycast(R3S.position, R3S.forward, out R2B,15);
        bool LH = Physics.Raycast(L1S.position, -L1S.up, out LFHIT,sensorlimit);
        bool RH = Physics.Raycast(R1S.position, -R1S.up, out RGHIT,sensorlimit);
        bool orsHit = Physics.Raycast(ORS.position, ORS.forward, out ORSHIT,5f); 


        Debug.DrawRay(L2S.position, L2S.forward *sensorlimit, BLH1? Color.yellow : Color.red);
        Debug.DrawRay(R2S.position, R2S.forward *sensorlimit, BRH1 ? Color.yellow : Color.red);
        Debug.DrawRay(L1S.position, -L1S.up *sensorlimit, LH ? Color.yellow : Color.red);
        Debug.DrawRay(R1S.position, -R1S.up *sensorlimit, RH ? Color.yellow : Color.red);
        Debug.DrawRay(ORS.position, ORS.forward *5f, orsHit ? Color.green : Color.red);
      

        if (BRH1 && R1B.collider.gameObject.layer == LayerMask.NameToLayer("Obs")) { 
            CSA = -30f;
        }
        else if (BLH1&& L1B.collider.gameObject.layer == LayerMask.NameToLayer("Obs")) { 
            CSA = 30f;
        }
        else if (BRH2 && R2B.collider.gameObject.layer == LayerMask.NameToLayer("Obs")) { 
            CSA = -30f;
        }
        else if (BLH2 && L2B.collider.gameObject.layer == LayerMask.NameToLayer("Obs")) { 
            CSA = 30f;
        }
        else if (RH && RGHIT.collider.gameObject.layer != LayerMask.NameToLayer("Road")) { 
            CSA = -30f;
        }
        else if (LH && LFHIT.collider.gameObject.layer != LayerMask.NameToLayer("Road")) {
            CSA = 30f;
        }
        else {
            CSA = 0f;
        }
        if (orsHit) { 
            CMF = 500f;
        }
        else {
            CMF = 300000f;
        }
    }

    void hasReachedtheEnd() {
        RaycastHit frnt;
        bool hit_the_front = Physics.Raycast(FRS.position, FRS.forward, out frnt,sensorlimit);
        Debug.DrawRay(FRS.position, FRS.forward *15, hit_the_front ? Color.green : Color.red);

        if (!hit_the_front) {
            Debug.Log("hasReachedtheEnd");
            Time.timeScale = 0;
            completed = true;
        }

    }
    private void StreeringControl() {
        RLC.motorTorque = CMF * Time.deltaTime;
        RRC.motorTorque = CMF * Time.deltaTime;
        FLC.steerAngle = CSA;
        FRC.steerAngle = CSA;

        WheelsUpdate(FLC, FLT);
        WheelsUpdate(FRC, FRT);
        WheelsUpdate(RLC, RLT);
        WheelsUpdate(RRC, RRT);
    }

    private void WheelsUpdate(WheelCollider col, Transform whltrns) {
        Vector3 position;
        Quaternion rotation;

        col.GetWorldPose(out position, out rotation);
        whltrns.position = position;
        whltrns.rotation = rotation;
    }
}