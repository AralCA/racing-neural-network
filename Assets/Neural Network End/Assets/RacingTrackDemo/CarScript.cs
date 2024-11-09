using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarScript : MonoBehaviour
{
    private Rigidbody rigidbody;
    private string logFinal;
    private int layerCount = 2;
    private bool isNNControlling = false;
    [SerializeField] private float speed;
    NeuralNetwork neuralNetwork;
    [SerializeField] List<GameObject> Wheels = new List<GameObject>();

    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject neuronTestObject;
    [SerializeField] private GameObject connectionTestObject;
    private List<WheelCollider> wheelColliders = new List<WheelCollider>();

    private void Start() {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.centerOfMass = new Vector3(0, -2,0);
        foreach(GameObject gameObject in Wheels){
            wheelColliders.Add(gameObject.GetComponent<WheelCollider>());
        }
        neuralNetwork = new NeuralNetwork(1,1,3,3,layerCount-2);

        int Count = 0;
        foreach(NeuralLayer neuralLayer in neuralNetwork.GetLayers()){
            int nodeCount = 0;
            foreach(NeuralNode neuralNode in neuralLayer.GetNodes()){
                DisplayNeuron(Count, nodeCount, neuralNode);
                nodeCount++;
            }
            Count++;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F)) isNNControlling= !isNNControlling;
        DisplayWheels();
        
    }

    void DisplayWheels(){
        foreach (GameObject wheel in Wheels){
            WheelCollider collider = wheel.GetComponent<WheelCollider>();
            Vector3 pos;
            Quaternion rot;
            Transform wheelDisp = collider.GetComponentsInChildren<Transform>()[1];
            collider.GetWorldPose(out pos, out rot);
            wheelDisp.position = pos;
            wheelDisp.rotation = rot;
            wheelDisp.eulerAngles+=Vector3.forward*90;
        }
        
    }


     private void FixedUpdate() {
        Ray rayForw = new Ray(transform.position, transform.forward);
        Ray rayRight = new Ray(transform.position, (transform.right+transform.forward*2).normalized);
        Ray rayLeft = new Ray(transform.position, (-1*transform.right+transform.forward*2).normalized);
        Debug.DrawRay(transform.position, transform.forward*50, Color.green);
        Debug.DrawRay(transform.position, (transform.right+transform.forward*2).normalized*50, Color.red);
        Debug.DrawRay(transform.position, (-1*transform.right+transform.forward*2).normalized*50, Color.red);

        RaycastHit hit;
        RaycastHit hit1;
        RaycastHit hit2;
        if (Physics.Raycast(rayForw, out hit)) {
            neuralNetwork.GetLayers().ElementAt(0).GetNodeByIndex(0).SetCurrentInput(Mathf.Clamp(hit.distance/50,-1,1));
            //Debug.Log("Straight Raycast ("+ hit.distance+ ") to input: " + Mathf.Clamp(hit.distance/50,-1,1));
            //Logger("Straight Raycast ("+ hit.distance+ ") to input: " + Mathf.Clamp(hit.distance/10,-1,1));
        }
        if (Physics.Raycast(rayRight, out hit1)) {
            neuralNetwork.GetLayers().ElementAt(0).GetNodeByIndex(1).SetCurrentInput(Mathf.Clamp(hit1.distance/50,-1,1));
            //Debug.Log("Right Raycast ("+ hit1.distance+ ") to input: " + Mathf.Clamp(hit1.distance/50,-1,1));
            //Logger("Right Raycast ("+ hit1.distance+ ") to input: " + Mathf.Clamp(hit1.distance/10,-1,1));
        }
        if (Physics.Raycast(rayLeft, out hit2)) {
            neuralNetwork.GetLayers().ElementAt(0).GetNodeByIndex(2).SetCurrentInput(Mathf.Clamp(hit2.distance/50,-1,1));
            //Debug.Log("Left Raycast ("+ hit2.distance+ ") to input: " + Mathf.Clamp(hit2.distance/50,-1,1));
            //Logger("Left Raycast ("+ hit2.distance+ ") to input: " + Mathf.Clamp(hit2.distance/10,-1,1));
        }

        neuralNetwork.Process();  

        
        
        if(isNNControlling){
            UnityEngine.Debug.Log("NNN");

            float GasUntouched = neuralNetwork.GetLayers().ElementAt(layerCount-1).GetNodeByIndex(1).GetCurrentInput();
            float SteeringUntouched = neuralNetwork.GetLayers().ElementAt(layerCount-1).GetNodeByIndex(0).GetCurrentInput();
            float BrakeUntouched = neuralNetwork.GetLayers().ElementAt(layerCount-1).GetNodeByIndex(2).GetCurrentInput();
            float GasFinal = 100;
            
            
            foreach(WheelCollider collider in wheelColliders){
                collider.motorTorque = GasFinal;
                if (collider.gameObject.name[0].Equals('F')){
                    collider.steerAngle = SteeringUntouched*30;
                }
            }
        }else{
            ControlDebugCar(hit,hit1,hit2);
            Train(1);
        }
        

        Logger(null);
    }

    public void Logger(string log){
        if(log!=null)logFinal+=log+"\n";
        else {
            Debug.Log(logFinal);
            logFinal = "";
        }
    }

    public void ControlDebugCar(RaycastHit hitForward, RaycastHit hitRight, RaycastHit hitLeft){
        
        float untouchedAngle = hitRight.distance-hitLeft.distance;
        float SteeringFinal = untouchedAngle*1;

        foreach(WheelCollider collider in wheelColliders){
            
            collider.motorTorque = 100;
            //collider.brakeTorque = Mathf.Clamp((20-hitForward.distance)*Mathf.Clamp(rigidbody.velocity.magnitude-5,0,1000),0,100)/10;
            //collider.brakeTorque = Mathf.Clamp(Mathf.Abs(SteeringFinal)-20, 0, 30);
            if (collider.gameObject.name[0].Equals('F')){
                collider.steerAngle = Mathf.Clamp(SteeringFinal,-30,30);
            }
        }
    }

    private void DisplayNeuron(int layer, int queueID, NeuralNode node){
        Vector3 neuronPosition = new Vector3(layer*2, queueID*1.2f, 0);

        GameObject neuron = Instantiate(neuronTestObject, neuronPosition, Quaternion.identity, this.transform);
        
        neuron.GetComponent<NeuronTest>().SetNeuralNode(node);
        if(node.GetConnections()==null) return;
        
        foreach(NeuralConnection neuralConnection in node.GetConnections()){
            connectionTestObject = Instantiate(connectionTestObject, neuron.transform.position, Quaternion.identity, this.transform);
            connectionTestObject.GetComponent<ConnectionTest>().setConnection(neuralConnection);
        }
        
    }

    private void Train(float factor){
        WheelCollider referenceWheel = wheelColliders.ElementAt(0);
        float steerAngle = referenceWheel.steerAngle;
        float throttle = referenceWheel.motorTorque/200;
        float brake = referenceWheel.brakeTorque;

        float steerPrediction = neuralNetwork.GetLayers().ElementAt(layerCount-1).GetNodeByIndex(0).GetCurrentInput();
        float throttlePrediction = neuralNetwork.GetLayers().ElementAt(layerCount-1).GetNodeByIndex(1).GetCurrentInput();
        float brakePrediction = neuralNetwork.GetLayers().ElementAt(layerCount-1).GetNodeByIndex(2).GetCurrentInput();

        float steerError = steerAngle/30 - steerPrediction;
        float throttleError = throttle - throttlePrediction;
        float brakeError = brake/100 - brakePrediction;

        UnityEngine.Debug.Log("Steering Er: " +  steerError + " Throttle Er: " + throttleError + " Brake Er: " + brakeError);

        float learningRate = 0.01f*factor;
        float biasLearningRate = 0.001f*factor;

        neuralNetwork.BackPropagate(neuralNetwork.GetLayers().ElementAt(layerCount-1).GetNodeByIndex(0), learningRate, biasLearningRate, steerError);
        //neuralNetwork.BackPropagate(neuralNetwork.GetLayers().ElementAt(layerCount-1).GetNodeByIndex(1), learningRate, biasLearningRate, throttleError);
        //neuralNetwork.BackPropagate(neuralNetwork.GetLayers().ElementAt(layerCount-1).GetNodeByIndex(2), learningRate, biasLearningRate, brakeError);
    }
 
}
