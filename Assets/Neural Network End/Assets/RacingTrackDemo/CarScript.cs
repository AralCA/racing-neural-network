using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarScript : MonoBehaviour
{
    private Rigidbody rigidbody;
    private string logFinal;
    private int layerCount = 3;
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
        neuralNetwork = new NeuralNetwork(1,3,3,2,layerCount-2);
    }
    // Update is called once per frame
    void Update()
    {
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
            Logger("Straight Raycast ("+ hit.distance+ ") to input: " + Mathf.Clamp(hit.distance/10,-1,1));
        }
        if (Physics.Raycast(rayRight, out hit1)) {
            neuralNetwork.GetLayers().ElementAt(0).GetNodeByIndex(1).SetCurrentInput(Mathf.Clamp(hit1.distance/50,-1,1));
            //Debug.Log("Right Raycast ("+ hit1.distance+ ") to input: " + Mathf.Clamp(hit1.distance/50,-1,1));
            Logger("Right Raycast ("+ hit1.distance+ ") to input: " + Mathf.Clamp(hit1.distance/10,-1,1));
        }
        if (Physics.Raycast(rayLeft, out hit2)) {
            neuralNetwork.GetLayers().ElementAt(0).GetNodeByIndex(2).SetCurrentInput(Mathf.Clamp(hit2.distance/50,-1,1));
            //Debug.Log("Left Raycast ("+ hit2.distance+ ") to input: " + Mathf.Clamp(hit2.distance/50,-1,1));
            Logger("Left Raycast ("+ hit2.distance+ ") to input: " + Mathf.Clamp(hit2.distance/10,-1,1));
        }

        neuralNetwork.Process();  

        float GasUntouched = neuralNetwork.GetLayers().ElementAt(layerCount-1).GetNodeByIndex(0).GetCurrentInput();
        float SteeringUntouched = neuralNetwork.GetLayers().ElementAt(layerCount-1).GetNodeByIndex(1).GetCurrentInput();
        float GasFinal = GasUntouched*speed;

        foreach(WheelCollider collider in wheelColliders){
            collider.motorTorque = GasFinal;
            if (collider.gameObject.name[0].Equals("F")){
                collider.steerAngle = SteeringUntouched*30;
            }
        }

        ControlDebugCar(hit,hit1,hit2);

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
        float SteeringFinal = untouchedAngle*5;

        foreach(WheelCollider collider in wheelColliders){
            
            collider.motorTorque = 200;
            collider.brakeTorque = Mathf.Clamp((20-hitForward.distance)*Mathf.Clamp(rigidbody.velocity.magnitude-5,0,1000),0,100)/10;
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
 
}
