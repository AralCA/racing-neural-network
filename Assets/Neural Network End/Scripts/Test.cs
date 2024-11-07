using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity;
using UnityEngine;

public class Test : MonoBehaviour {
    //i literally wrote progation before    
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject neuronTestObject;
    [SerializeField] private GameObject connectionTestObject;

    private int currentIndex =0;
    private int currentCount =0;
    private float learningRate = 0.1f;
    private float biasLearningRate = 0.001f;

    private bool learn = true;
    private NeuralNetwork testNetwork;
    private void Start() {
        Stopwatch stopwatch = new Stopwatch();
        NeuralNetwork network = new NeuralNetwork(1,3,2,1,1);
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

    private void FixedUpdate() {

            demo();
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
        List<float> outputs = new List<float>();
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
                float deltaBias = learningRate * outputGradient;
                neuralConnection.Weight += deltaWeight;
                node.SetBias(node.GetBias()+deltaBias);
                UnityEngine.Debug.Log(neuralConnection.Weight);
            }
        }

        UnityEngine.Debug.Log(testNetwork.GetLayers().Last().GetNodes().Last().GetCurrentInput());
        UnityEngine.Debug.Log(" Error: " + error + " Error Deriv: " + error_deriv + " Grad: " + outputGradient + " ");
        



    }

    public void demo(){
        
        if(testNetwork==null) testNetwork =  new NeuralNetwork(1,3,2,1,1);

        
        List<List<float>> inputs = new List<List<float>>();
        inputs.Add(new List<float>());
        inputs.First().Add(0.5f);
        inputs.First().Add(0.4f);
        
        inputs.Add(new List<float>());
        inputs[1].Add(0.6f);
        inputs[1].Add(0.2f);


        List<List<float>> outputs = new List<List<float>>();
        outputs.Add(new List<float>());
        outputs.First().Add(0.9f);

        outputs.Add(new List<float>());
        outputs[1].Add(0.8f);

        int index = 0;
        foreach(NeuralNode neuralNode in testNetwork.GetLayers().ElementAt(0).GetNodes()){
            testNetwork.GetLayers().ElementAt(0).GetNodes().ElementAt(index).SetCurrentInput(inputs[currentIndex].ElementAt(index));
            index++;
        }

        testNetwork.Process();

        int Count = 0;
        foreach(NeuralNode node in testNetwork.GetLayers().Last().GetNodes()){
            float error = outputs[currentIndex][Count] - node.GetCurrentInput();
            if(learn) BackPropagate(node, learningRate, error);
            Count++;
        }
        currentCount++;
        if(currentCount == 1000){
            if(currentIndex==1){
                if(learn) learn=false;
                currentIndex--;
            }
            else{
                currentIndex++;
            }

            currentCount = 0;
        }

        

            
    }

    public void BackPropagate(NeuralNode node, float learningRate, float error) {
        if(node == null){return;}

        float error_deriv = error*2;
        float ReLU_Derivative = node.GetCurrentInput() > 0 ? 1 : 0;
        
        float outputGradient = error_deriv * ReLU_Derivative;

        foreach(NeuralConnection neuralConnection in node.GetBackConnections()){
            float deltaWeight = learningRate * outputGradient * neuralConnection.N1.GetCurrentInput();

            float deltaBias = biasLearningRate * outputGradient;
            neuralConnection.Weight += deltaWeight;
            node.SetBias(node.GetBias()-deltaBias);

            float newError = error * neuralConnection.Weight;

            BackPropagate(neuralConnection.N1,learningRate, newError);
        }
        UnityEngine.Debug.Log(testNetwork.GetLayers().Last().GetNodes().Last().GetCurrentInput());
        UnityEngine.Debug.Log("Output: " + testNetwork.GetLayers().Last().GetNodes().Last().GetCurrentInput() + " Error: " + error + " Error Deriv: " + error_deriv + " Grad: " + outputGradient + " ");
    }
}