using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterDemo : MonoBehaviour
{

    private float initialDistance = 0;
    public NeuralNetwork neuralNetwork;

    [SerializeField] private int layerCount = 3;
    [SerializeField] private CharacterManager manager;

    [SerializeField] private GameObject target;
    [SerializeField] private float speed;
    [SerializeField] private bool rewireOn = true;

    private void Start() {
        initialDistance= Vector3.Distance(target.transform.position, transform.position);
        neuralNetwork = new NeuralNetwork(1, 4, 3, 3, layerCount-2);
    }
    /*
    private void Update() {
        float distanceDelta = initialDistance - Vector3.Distance(target.transform.position, transform.position);
        if(distanceDelta<0.0000001f) {
            if((distanceDelta==0||!(Vector3.Distance(target.transform.position, transform.position)<3))&&!rewireOn&&speed!=0) {
                Debug.Log("REWIRE_OFF REWIRE SPEED: " + speed);
                neuralNetwork = new NeuralNetwork(1, 4, 3, 2, layerCount-2);
            }
            if(rewireOn&&speed!=0){ 
                Debug.Log("REWIRE_ON REWIRE SPEED: " + speed);
                neuralNetwork = new NeuralNetwork(1, 4, 3, 2, layerCount-2);
            }
        }
        initialDistance = Vector3.Distance(target.transform.position, transform.position);
        neuralNetwork.Tease((float)Math.Pow(Vector3.Distance(target.transform.position, transform.position)/10,3));
        if(transform.position.x>50||transform.position.z>50||transform.position.z<-50|| transform.position.x<-50) {
            //neuralNetwork = new NeuralNetwork(1, 4, 3, 2, layerCount-2);
            transform.position = Vector3.zero;
        }

        if(Vector3.Distance(target.transform.position, transform.position)<3) {
            speed = 0;
        }else{
            speed = 100;
        }
        neuralNetwork.GetLayers().ElementAt(0).GetNodeByIndex(0).SetCurrentInput(Vector3.Distance(target.transform.position, transform.position)/100);
        neuralNetwork.GetLayers().ElementAt(0).GetNodeByIndex(1).SetCurrentInput(distanceDelta/speed*0.001f);

        

        neuralNetwork.Process();

        if(neuralNetwork.GetLayers().ElementAt(layerCount-1).GetNodeByIndex(0).GetIsFired())
        transform.Rotate(Vector3.up * speed*0.01f*neuralNetwork.GetLayers().ElementAt(layerCount-1).GetNodeByIndex(0).GetCurrentInput());
        if(neuralNetwork.GetLayers().ElementAt(layerCount-1).GetNodeByIndex(1).GetIsFired())
        transform.Translate(Vector3.forward * speed*0.001f);
    }
    */

    private void Update() {
        if(Vector3.Distance(target.transform.position, transform.position)<3) {
            speed = 0;
        }else{
            speed = 100;
        }

        Ray rayForw = new Ray(transform.position, transform.forward);
        Ray rayRight = new Ray(transform.position, transform.right);
        Ray rayLeft = new Ray(transform.position, -1*transform.right);

        RaycastHit hit;
        RaycastHit hit1;
        RaycastHit hit2;
        if (Physics.Raycast(rayForw, out hit)) {
            neuralNetwork.GetLayers().ElementAt(0).GetNodeByIndex(0).SetCurrentInput(Math.Clamp(hit.distance/50,-1,1));
            Debug.Log("Straight Raycast ("+ hit.distance+ ") to input: " + Math.Clamp(hit.distance/50,-1,1));
        }
        if (Physics.Raycast(rayRight, out hit1)) {
            neuralNetwork.GetLayers().ElementAt(0).GetNodeByIndex(1).SetCurrentInput(Math.Clamp(hit1.distance/50,-1,1));
            Debug.Log("Right Raycast ("+ hit1.distance+ ") to input: " + Math.Clamp(hit1.distance/50,-1,1));
        }
        if (Physics.Raycast(rayLeft, out hit2)) {
            neuralNetwork.GetLayers().ElementAt(0).GetNodeByIndex(2).SetCurrentInput(Math.Clamp(hit2.distance/50,-1,1));
            Debug.Log("Left Raycast ("+ hit2.distance+ ") to input: " + Math.Clamp(hit2.distance/50,-1,1));
        }


        neuralNetwork.Process();

        if(neuralNetwork.GetLayers().ElementAt(layerCount-1).GetNodeByIndex(0).GetIsFired())
        transform.Rotate(Vector3.up * speed*0.1f*neuralNetwork.GetLayers().ElementAt(layerCount-1).GetNodeByIndex(0).GetCurrentInput());
        //if(neuralNetwork.GetLayers().ElementAt(layerCount-1).GetNodeByIndex(1).GetIsFired())
        transform.Translate(Vector3.forward * speed*0.001f);
        if(neuralNetwork.GetLayers().ElementAt(layerCount-1).GetNodeByIndex(2).GetIsFired())
        transform.Rotate(Vector3.down * speed*0.1f*neuralNetwork.GetLayers().ElementAt(layerCount-1).GetNodeByIndex(2).GetCurrentInput());

        neuralNetwork.Tease(0.01f);

    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("WOW");
        if (other.gameObject.tag.Equals("Respawn")){
            transform.position = Vector3.zero;
            neuralNetwork = new NeuralNetwork(1, 4, 3, 3, layerCount-2);
        }
    }
        
    


}
