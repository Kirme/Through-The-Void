using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class PlayerController : MonoBehaviour
{

    private SteamVR_Input_Sources rightHand = SteamVR_Input_Sources.RightHand;
    private SteamVR_Input_Sources leftHand = SteamVR_Input_Sources.LeftHand;

    public SteamVR_Action_Single squeezeAction;
    public SteamVR_Action_Boolean grabAction;
    public SteamVR_Action_Pose poseAction;
    public GameObject rightController, leftController, joystick, warningLight, faultDisplay, networker;

    public float maxSpeed = 25f, acceleration = 2.5f, maxTurnSpeed = 12.5f, turnAcceleration = 2.5f;

    public float maxHitpoints = 100f;
    private float hitpoints = 100f;

    private bool invulnerable = false;
    private float invulTimer = 3f;
    private float MaxWorkingSpeed = 7f;
    private float currentSpeedTimer = 10f;

    private StatsHandler statsHandler;

    audioManager sn;

    public UnityEvent<float> onDamageTaken;
    public UnityEvent onHitpointsZero;
    public UnityEvent<Vector3> originReset;

    public float xRotDeadzone = 10.0f, zRotDeadzone = 10.0f, yRotDeadzone = 10.0f;
    public float maxSteeringRot = 70.0f;
    
    private float speed = 0f, inputSpeed = 0f;
    private Vector3 rotationSpeed = new Vector3(0,0,0);

    private bool tutorialMode = true;

    public void SetTutorialMode(bool val)
    {
        tutorialMode = val;
    }

    public void Break(Fault fault, int numFaults)
    {
        statsHandler.AddFault(fault);

        warningLight.SetActive(true);
        warningLight.GetComponent<WarningLight>().speed = 2.5f + 0.5f * numFaults;
        faultDisplay.GetComponent<FaultDisplay>().SetNumFaults(numFaults);
    }

    public void Fix(Fault fault, int remainingFaults)
    {
        statsHandler.FixFault(fault);

        if (remainingFaults  == 0)
        {
            warningLight.SetActive(false);
        } else
        {
            warningLight.GetComponent<WarningLight>().speed = 2.5f + 0.5f * remainingFaults;
        }
        faultDisplay.GetComponent<FaultDisplay>().SetNumFaults(remainingFaults);
    }

    public void FixAll()
    {
        statsHandler.ResetFaults();

        warningLight.SetActive(false);
        faultDisplay.GetComponent<FaultDisplay>().SetNumFaults(0);
    }

    // Start is called before the first frame update
    void Start()
    {
        statsHandler = GetComponent<StatsHandler>();
        warningLight.SetActive(false);
        poseAction[rightHand].onTrackingChanged += OnTrackPadChanged;

        joystick.GetComponent<JoystickInteractable>().SetPlayer(this);

        sn = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<audioManager>();
        sn.Play("NASA");

        hitpoints = maxHitpoints;
    }

    // Update is called once per frame
    void Update()
    {
        // Only rotate if grabbing

        Vector3 joystickRotation = Vector3.zero;

        bool grabbing = false;
        JoystickInteractable joystickInteractable = joystick.GetComponent<JoystickInteractable>();
        if (joystickInteractable != null)
        {
            grabbing = joystickInteractable.interacting;

            if (grabbing)
            {
                joystickRotation = joystickInteractable.attitude;
            } 

        }

        rotationSpeed.x = Mathf.Lerp(rotationSpeed.x, (statsHandler.invertY ? 1 : -1) * maxTurnSpeed * joystickRotation.x * Time.deltaTime * statsHandler.maxTurnSpeedModifier, turnAcceleration * Time.deltaTime * statsHandler.maxTurnSpeedModifier * statsHandler.turnAccelerationModifier);
        rotationSpeed.y = Mathf.Lerp(rotationSpeed.y, (statsHandler.invertX ? 1 : -1) * maxTurnSpeed * joystickRotation.z * Time.deltaTime * statsHandler.maxTurnSpeedModifier, turnAcceleration * Time.deltaTime * statsHandler.maxTurnSpeedModifier * statsHandler.turnAccelerationModifier);
        rotationSpeed.z = Mathf.Lerp(rotationSpeed.z, (statsHandler.invertZ ? -1 : 1) * maxTurnSpeed/2*joystickRotation.y * Time.deltaTime * statsHandler.maxTurnSpeedModifier, turnAcceleration * Time.deltaTime * statsHandler.maxTurnSpeedModifier * statsHandler.turnAccelerationModifier);

        speed = Mathf.Lerp(speed, maxSpeed * (statsHandler.fullSpeed?1.0f:inputSpeed) * statsHandler.maxSpeedModifier, acceleration * Time.deltaTime * statsHandler.accelerationModifier * statsHandler.maxSpeedModifier);
        if(currentSpeedTimer < invulTimer){
            speed = Mathf.Min(speed, MaxWorkingSpeed + ((speed - MaxWorkingSpeed) * Mathf.Min(invulTimer, currentSpeedTimer) / invulTimer));
            currentSpeedTimer += Time.deltaTime;
        }
        transform.Rotate(rotationSpeed.x, rotationSpeed.y, rotationSpeed.z, Space.Self);
        transform.position += transform.forward * speed * Time.deltaTime;

        if(transform.position.magnitude > 1000)
        {

            Vector3 pos = transform.position;
            transform.position = new Vector3(0, 0, 0);

            originReset.Invoke(pos);
        }
    }

    public void SetSpeed(float speed)
    {

        const float speedDeadzone = 0.05f;

        if(Mathf.Abs(speed) < speedDeadzone)
        {
            speed = 0;
        } else
        {
            speed = (speed - speedDeadzone) / (1f - speedDeadzone);
        }

        if(speed < 0)
        {
            speed /= 10;
        }
        inputSpeed = speed;
    }

    public void OnTrackPadChanged(SteamVR_Action_Pose changedAction, SteamVR_Input_Sources changedSource, ETrackingResult trackingChanged)
    {

    }

    public void OnCollisionEnter(Collision col){
        if(col.gameObject.tag == "Asteroid"){
            if(!invulnerable){
                invulnerable = true;
                Quaternion rel = Quaternion.Inverse(gameObject.transform.rotation) * col.gameObject.transform.rotation;
                float xdiff = speed*transform.forward.x - 2*col.gameObject.transform.rotation.x;
                float ydiff = speed*transform.forward.y - 2*col.gameObject.transform.rotation.y;
                float zdiff = speed*transform.forward.z - 2*col.gameObject.transform.rotation.z;
                float speeddiff = Mathf.Sqrt(xdiff*xdiff + ydiff*ydiff + zdiff*zdiff);
                //Quaternion tr = col.gameObject.transform.rotation;
                TakeDamage(speeddiff);
                Debug.Log("DAMAGE");
                Debug.Log(speeddiff);
                StartCoroutine(CollisionEnumerator());
            }
            //Not perfect as added momentum isn't taken into account, but this 
            col.gameObject.GetComponent<Rigidbody>().AddForce(speed*transform.forward.x, speed*transform.forward.y, speed*transform.forward.z);
        }
    }

    public void OnCollisionStay(Collision col){
        currentSpeedTimer = 0f;
    }

    IEnumerator CollisionEnumerator()
    {
        yield return new WaitForSeconds(invulTimer);
        invulnerable = false;
        yield break;
    }

    public float GetHitpoints()
    {
        return this.hitpoints;
    }

    public void TakeDamage(float damage){

        if (tutorialMode)
        {
            return;
        }

        this.hitpoints -= damage;
        if(hitpoints <= 0)
        {
            onHitpointsZero.Invoke();
            hitpoints = maxHitpoints;
        }
        onDamageTaken.Invoke(damage);
        networker.GetComponent<FaultHandler>().UpdateSideText();
    }

    public bool GetRightGrab()
    {
        return grabAction.GetStateDown(rightHand);
    }

    public float GetRightSqueeze()
    {
        return squeezeAction.GetAxis(rightHand);
    }

    public float GetLeftSqueeze()
    {
        return squeezeAction.GetAxis(leftHand);
    }

    public Vector3 GetRightControllerPositon()
    {
        return rightController.transform.localPosition;
    }

    public Vector3 GetLeftControllerPositon()
    {
        return leftController.transform.localPosition;
    }

    public Quaternion GetRightControllerRotation()
    {
        return rightController.transform.localRotation;
    }

    public Quaternion GetLeftControllerRotation()
    {
        return leftController.transform.localRotation;
    }
}