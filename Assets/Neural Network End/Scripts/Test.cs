using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity;
using UnityEngine;

public class Test : MonoBehaviour {
    
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject neuronTestObject;
    [SerializeField] private GameObject connectionTestObject;


    private float learningRate = 0.1f;
    private NeuralNetwork testNetwork;
    private void Start() {
        Stopwatch stopwatch = new Stopwatch();
        NeuralNetwork network = new NeuralNetwork(1,2,2,1,1);
        testNetwork = network;
        int layerCount = 0;
        foreach(NeuralLayer neuralLayer in network.GetLayers()){
            int nodeCount = 0;
            foreach(NeuralNode neuralNode in neuralLayer.GetNodes()){
                DisplayNeuron(layerCount, nodeCount, neuralNode);
                nodeCount++;
            }
            layerCount++;
        }
        stopwatch.Stop();
        UnityEngine.Debug.Log("Time It Took To Display: " + stopwatch.ElapsedTicks+ " Ticks");
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

    private void Update() {
        BackProgationDemo();
        /*
        if(Input.GetMouseButtonDown(0)){
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out hit)) {
                Transform objectHit = hit.transform;
                //UnityEngine.Debug.Log("HITTT");
                if(objectHit.tag.Equals("Neuron")){
                    //objectHit.GetComponent<NeuronTest>().GetNeuralNode().SetFire(!objectHit.GetComponent<NeuronTest>().GetNeuralNode().GetIsFired());
                    if(objectHit.GetComponent<NeuronTest>().GetNeuralNode().GetCurrentInput()>0){
                        objectHit.GetComponent<NeuronTest>().GetNeuralNode().SetCurrentInput(-1);
                    }else{
                        objectHit.GetComponent<NeuronTest>().GetNeuralNode().SetCurrentInput(1);
                    }
                    
                    //UnityEngine.Debug.Log(objectHit.GetComponent<NeuronTest>().GetNeuralNode().GetIsFired());
                }
            }
        }
        Stopwatch stopwatch= Stopwatch.StartNew();
        testNetwork.Process();
        stopwatch.Stop();
        //UnityEngine.Debug.Log("Time It Took To Process: " + stopwatch.ElapsedTicks+ " Ticks");
        */
    }

    public void BackProgationDemo(){
        if(testNetwork==null) testNetwork =  new NeuralNetwork(1,2,2,1,1);

        float ReLU_Derivative = 1;

        float wantedOutput = 0.9f;

        List<float> inputs = new List<float>();
        inputs.Add(0.5f); inputs.Add(0.6f);

        testNetwork.GetLayers().ElementAt(0).GetNodes().ElementAt(0).SetCurrentInput(inputs.ElementAt(0));
        testNetwork.GetLayers().ElementAt(0).GetNodes().ElementAt(1).SetCurrentInput(inputs.ElementAt(1));

        testNetwork.Process();

        float error = wantedOutput - testNetwork.GetLayers().Last().GetNodes().Last().GetCurrentInput();
        float error_deriv = error*2;

        float outputGradient = error_deriv*ReLU_Derivative;

        foreach(NeuralNode node in testNetwork.GetLayers().Last().GetNodes()){
            foreach(NeuralConnection neuralConnection in node.GetBackConnections()){
                float deltaWeight = learningRate * outputGradient * neuralConnection.N1.GetCurrentInput();
                neuralConnection.Weight += deltaWeight;
                UnityEngine.Debug.Log(neuralConnection.Weight);
            }
        }

        UnityEngine.Debug.Log(testNetwork.GetLayers().Last().GetNodes().Last().GetCurrentInput());
        UnityEngine.Debug.Log(" Error: " + error + " Error Deriv: " + error_deriv + " Grad: " + outputGradient + " ");
        



    }
}