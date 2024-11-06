using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuronTest : MonoBehaviour
{
    bool state = true;
    NeuralNode neuralNode;

    private void LateUpdate() {
        if(neuralNode == null) return;
        //GetComponent<Renderer>().material.color = Color.green;
        if(neuralNode.GetIsFired()&&!state){
            //GetComponent<Renderer>().material.color = Color.green;
            GetComponent<Renderer>().enabled = true;
            state = true;
        }else if(!neuralNode.GetIsFired()&&state){
            //GetComponent<Renderer>().material.color= Color.red;
            GetComponent<Renderer>().enabled = false;
            state = false;
        }
    }

    public void SetNeuralNode(NeuralNode node){
        neuralNode = node;
    }

    public NeuralNode GetNeuralNode(){
        return neuralNode;
    }

    public void ToggleState(){
        state =!state;
    }
}

