using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.Kinect;
using System.Text;

public class KinectManager : MonoBehaviour
{
    public Text DetectTextGameObject;
    public Text Gesture1Text;
    public Text Gesture2Text;
    public Text Gesture3Text;
    public GameObject Player;
    //private Turning turnScript;
    private VGB_Clapping clappingScript;

    public readonly string GestureClapping = "Clapping";
    public readonly string GestureHandShake_Right = "HandShake_Right";
    public readonly string GestureLeftRotation_Right = "LeftRotation_Right";
    // Kinect 
    private KinectSensor kinectSensor;

    // color frame and data 
    private ColorFrameReader colorFrameReader;
    private byte[] colorData;
    private Texture2D colorTexture;

    private BodyFrameReader bodyFrameReader;
    private int bodyCount;
    private Body[] bodies;

    //private string leanLeftGestureName = "Lean_Left";
    //private string leanRightGestureName = "Lean_Right";

    // GUI output
    private UnityEngine.Color[] bodyColors;
    //private string[] bodyText;

    /// <summary> List of gesture detectors, there will be one detector created for each potential body (max of 6) </summary>
    private List<GestureDetector> gestureDetectorList = null;

    // Use this for initialization
    void Start()
    {
        //turnScript = Player.GetComponent<Turning>();
        clappingScript = Player.GetComponent<VGB_Clapping>();
        // get the sensor object

        this.kinectSensor = KinectSensor.GetDefault();

        if (this.kinectSensor != null)
        {
            this.bodyCount = this.kinectSensor.BodyFrameSource.BodyCount;

            // color reader
            this.colorFrameReader = this.kinectSensor.ColorFrameSource.OpenReader();

            // create buffer from RGBA frame description
            var desc = this.kinectSensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Rgba);


            // body data
            this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();

            // body frame to use
            this.bodies = new Body[this.bodyCount];

            // initialize the gesture detection objects for our gestures
            this.gestureDetectorList = new List<GestureDetector>();
            for (int bodyIndex = 0; bodyIndex < this.bodyCount; bodyIndex++)
            {
                //PUT UPDATED UI STUFF HERE FOR NO GESTURE
                DetectTextGameObject.text = "none";
                //this.bodyText[bodyIndex] = "none";
                //Create gesture detectors and add them to gestureDetectorList
                this.gestureDetectorList.Add(new GestureDetector(this.kinectSensor));
            }

            // start getting data from runtime
            this.kinectSensor.Open();
        }
        else
        {
            //kinect sensor not connected
            DetectTextGameObject.text = "not Connected";
        }
    }

    // Update is called once per frame
    void Update()
    {

        // process bodies
        bool newBodyData = false;
        using (BodyFrame bodyFrame = this.bodyFrameReader.AcquireLatestFrame())
        {
            if (bodyFrame != null)
            {
                bodyFrame.GetAndRefreshBodyData(this.bodies);
                newBodyData = true;
            }
        }

        if (newBodyData)
        {
            // update gesture detectors with the correct tracking id
            for (int bodyIndex = 0; bodyIndex < this.bodyCount; bodyIndex++)
            {
                var body = this.bodies[bodyIndex];
                if (body != null)
                {
                    var trackingId = body.TrackingId;

                    // if the current body TrackingId changed, update the corresponding gesture detector with the new value
                    if (trackingId != this.gestureDetectorList[bodyIndex].TrackingId)
                    {
                        //DetectTextGameObject.text = "Detected :none ";
                        //this.bodyText[bodyIndex] = "none";
                        this.gestureDetectorList[bodyIndex].TrackingId = trackingId;
                        // if the current body is tracked, unpause its detector to get VisualGestureBuilderFrameArrived events
                        // if the current body is not tracked, pause its detector so we don't waste resources trying to get invalid gesture results
                        this.gestureDetectorList[bodyIndex].IsPaused = (trackingId == 0);
                        this.gestureDetectorList[bodyIndex].OnGestureDetected += CreateOnGestureHandler(bodyIndex);
                    }
                }
                else
                {
                    //DetectTextGameObject.text = "can't tracking " + this.bodyCount.ToString();
                }
            }
        }

    }

    private EventHandler<GestureEventArgs> CreateOnGestureHandler(int bodyIndex)
    {
        //GestureTextGameObject.text = "Gesture Detected " + e.GestureID;
        return (object sender, GestureEventArgs e) => OnGestureDetected(sender, e, bodyIndex);
    }

    private void OnGestureDetected(object sender, GestureEventArgs e, int bodyIndex)
    {
        Debug.Log("Detected!");
        
        var isDetected = e.IsBodyTrackingIdValid && e.IsGestureDetected;
        DetectTextGameObject.text = "Detected : " + isDetected;
       
        if (e.GestureID == GestureClapping)
        {
           
            if (e.DetectionConfidence > 0.65f)
            {
                Gesture1Text.text = GestureClapping + " : true  " + e.DetectionConfidence.ToString();
                clappingScript.bClapping = true;
            }
            else
            {
                Gesture1Text.text = GestureClapping + " : false " + e.DetectionConfidence.ToString();
                clappingScript.bClapping = false;
            }
        }
        if (e.GestureID == GestureHandShake_Right)
        {
           
            if (e.DetectionConfidence > 0.65f)
            {
                Gesture2Text.text = GestureHandShake_Right + " : true  " + e.DetectionConfidence.ToString();
                //clappingScript.bClapping = true;
            }
            else
            {
                Gesture2Text.text = GestureHandShake_Right + " : false " + e.DetectionConfidence.ToString();
                //clappingScript.bClapping = false;
            }
        }
        if (e.GestureID == GestureHandShake_Right)
        {

            if (e.DetectionConfidence > 0.65f)
            {
                Gesture2Text.text = GestureHandShake_Right + " : true  " + e.DetectionConfidence.ToString();
                //clappingScript.bClapping = true;
            }
            else
            {
                Gesture2Text.text = GestureHandShake_Right + " : false " + e.DetectionConfidence.ToString();
                //clappingScript.bClapping = false;
            }
        }
        if (e.GestureID == GestureLeftRotation_Right)
        {

            if (e.DetectionConfidence > 0.65f)
            {
                Gesture3Text.text = GestureLeftRotation_Right + " : true  " + e.DetectionConfidence.ToString();
                //clappingScript.bClapping = true;
            }
            else
            {
                Gesture3Text.text = GestureLeftRotation_Right + " : false " + e.DetectionConfidence.ToString();
                //clappingScript.bClapping = false;
            }
        }
        //this.bodyText[bodyIndex] = text.ToString();
    }

   
    void OnApplicationQuit()
    {
        if (this.colorFrameReader != null)
        {
            this.colorFrameReader.Dispose();
            this.colorFrameReader = null;
        }

        if (this.bodyFrameReader != null)
        {
            this.bodyFrameReader.Dispose();
            this.bodyFrameReader = null;
        }

        if (this.kinectSensor != null)
        {
            if (this.kinectSensor.IsOpen)
            {
                this.kinectSensor.Close();
            }

            this.kinectSensor = null;
        }
    }

}
