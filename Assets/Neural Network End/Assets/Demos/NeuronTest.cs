using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NeuronTest : MonoBehaviour
{
    [SerializeField] private Material redMat;
    [SerializeField] private Material greenMat;
    [SerializeField] private TextMeshProUGUI textUI;
    bool state = true;
    NeuralNode neuralNode;

    private void Start() {
        GetComponent<Renderer>().material = greenMat;
    }
    private void LateUpdate() {
        if(neuralNode == null) return;
        if(neuralNode.GetCurrentInput()>0&&!state){
            GetComponent<Renderer>().material = greenMat;
            state = true;
        }else if(neuralNode.GetCurrentInput()<=0&&state){
            GetComponent<Renderer>().material = redMat;
            state = false;
        }
        string myText = "" +neuralNode.GetCurrentInput();
        if(myText.Length>4)
        myText = myText.Substring(0,4);
        string myText2 = neuralNode.GetBias().ToString();
        if(myText2.Length>4)
        myText2 = myText2.Substring(0,4);
        textUI.text = "" +myText + "\n" + myText2;
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

