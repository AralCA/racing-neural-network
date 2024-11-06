using System.Diagnostics;
using Unity;
using UnityEngine;

public class Test : MonoBehaviour {
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject neuronTestObject;
    [SerializeField] private GameObject connectionTestObject;

    private NeuralNetwork testNetwork;
    private void Start() {
        Stopwatch stopwatch = new Stopwatch();
        NeuralNetwork network = new NeuralNetwork(1,4,3,3,2);
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

        GameObject neuron = Instantiate(neuronTestObject, neuronPosition, Quaternion.identity, canvas.transform);
        
        neuron.GetComponent<NeuronTest>().SetNeuralNode(node);
        if(node.GetConnections()==null) return;
        
        foreach(NeuralConnection neuralConnection in node.GetConnections()){
            connectionTestObject = Instantiate(connectionTestObject, neuron.transform.position, Quaternion.identity, canvas.transform);
            connectionTestObject.GetComponent<ConnectionTest>().setConnection(neuralConnection);
        }
        
    }

    private void Update() {
        if(Input.GetMouseButtonDown(0)){
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out hit)) {
                Transform objectHit = hit.transform;
                UnityEngine.Debug.Log("HITTT");
                if(objectHit.tag.Equals("Neuron")){
                    objectHit.GetComponent<NeuronTest>().GetNeuralNode().SetFire(!objectHit.GetComponent<NeuronTest>().GetNeuralNode().GetIsFired());
                    UnityEngine.Debug.Log(objectHit.GetComponent<NeuronTest>().GetNeuralNode().GetIsFired());
                }
            }
        }
        Stopwatch stopwatch= Stopwatch.StartNew();
        testNetwork.Process();
        stopwatch.Stop();
        UnityEngine.Debug.Log("Time It Took To Process: " + stopwatch.ElapsedTicks+ " Ticks");
    }
}