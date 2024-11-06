using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CubeDemo : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject neuronTestObject;
    [SerializeField] private GameObject connectionTestObject;

    [SerializeField] private float xBorder = 20;
    [SerializeField] private float yBorder = 20;

    private GameObject displayParent;
    int version = 0;
    int moveCount = 0;
    float avgMoveCount = 0;
    static float maxMoveCount = 0;

    int lastMoveCount=0;
    [SerializeField] float speed = 5;
    public NeuralNetwork neuralNetwork;

    int lastLayerIndex = 1;

    private void Start() {
        neuralNetwork = new NeuralNetwork(1, 4, 4, 4, 0);
    }

    private void Update() {

        if(transform.position.x<0||transform.position.x>xBorder||transform.position.y<0||transform.position.y>yBorder) {
            /*
            if(moveCount>25){
                neuralNetwork.Tease(1);
                maxMoveCount = Mathf.Max(maxMoveCount, moveCount);
                lastMoveCount = moveCount;
                moveCount = 0;
                transform.position = new Vector3(xBorder/2, yBorder/2, transform.position.z);
                return;
            }
            */
            neuralNetwork = new NeuralNetwork(1, 4, 4, 4, 0);
            transform.position = new Vector3(xBorder/2, yBorder/2, transform.position.z);
            avgMoveCount = moveCount + avgMoveCount*version;
            avgMoveCount /= version+1;
            version++;
            maxMoveCount = Mathf.Max(maxMoveCount, moveCount);
            lastMoveCount = moveCount;
            moveCount = 0;
            return;
        }
        moveCount++;
        /*
        float posX = transform.position.x * (255/20);
        float posY = transform.position.y * (255/20);
        */

        float posToLeft = (float)Math.Pow(transform.position.x/xBorder,3);
        float posToRight =  (float)Math.Pow((xBorder-transform.position.x)/xBorder,3);
        float posToDown =  (float)Math.Pow(transform.position.y/yBorder,3);
        float posToUp =  (float)Math.Pow((yBorder-transform.position.y)/yBorder,3);

        /*
        posX = Mathf.Clamp(posX, 0, 255);
        posY = Mathf.Clamp(posY, 0, 255);
        */

        neuralNetwork.GetLayers().ElementAt(0).GetNodeByIndex(0).SetCurrentInput(posToDown);
        neuralNetwork.GetLayers().ElementAt(0).GetNodeByIndex(1).SetCurrentInput(posToUp);
        neuralNetwork.GetLayers().ElementAt(0).GetNodeByIndex(2).SetCurrentInput(posToRight);
        neuralNetwork.GetLayers().ElementAt(0).GetNodeByIndex(3).SetCurrentInput(posToLeft);



        neuralNetwork.Process();
        if(neuralNetwork.GetLayers().ElementAt(lastLayerIndex).GetNodeByIndex(0).GetIsFired())
        transform.position+= Vector3.right * speed*0.01f;
        else//if(neuralNetwork.GetLayers().ElementAt(lastLayerIndex).GetNodeByIndex(1).GetIsFired())
        transform.position-= Vector3.right * speed*0.01f;

        if(neuralNetwork.GetLayers().ElementAt(lastLayerIndex).GetNodeByIndex(2).GetIsFired())
        transform.position+= Vector3.up * speed*0.01f;
        
        else //if(neuralNetwork.GetLayers().ElementAt(lastLayerIndex).GetNodeByIndex(3).GetIsFired()) 
        transform.position-= Vector3.up * speed*0.01f;


        Debug.Log("Version: " + version + " AVG: " +  avgMoveCount + " MAX: " + maxMoveCount + " LAST: " + lastMoveCount);
        

        if(displayParent!=null) Destroy(displayParent);
            displayParent = new GameObject("DisplayParent");

            int layerCount = 0;
            foreach(NeuralLayer neuralLayer in neuralNetwork.GetLayers()){
                int nodeCount = 0;
                foreach(NeuralNode neuralNode in neuralLayer.GetNodes()){
                    DisplayNeuron(layerCount, nodeCount, neuralNode);
                    nodeCount++;
                }
                layerCount++;
        }
    }


    private void DisplayNeuron(int layer, int queueID, NeuralNode node){
        Vector3 neuronPosition = new Vector3(layer*2, queueID*1.2f, transform.position.z);
        
        GameObject neuron = Instantiate(neuronTestObject, neuronPosition, Quaternion.identity, displayParent.transform);
        neuron.GetComponent<NeuronTest>().SetNeuralNode(node);
        if(node.GetConnections()==null) return;
        /*
        foreach(NeuralConnection neuralConnection in node.GetConnections()){
            connectionTestObject = Instantiate(connectionTestObject, neuron.transform.position, Quaternion.identity, displayParent.transform);
            connectionTestObject.GetComponent<ConnectionTest>().setConnection(neuralConnection);
        }
        */
        
    }
}
