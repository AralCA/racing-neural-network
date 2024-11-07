using System;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNode
{
    private bool isFired = false;
    private float bias = 0;
    private float currentInput = 0;
    private int id;
    private NeuralLayer layer;
    private NeuralNetwork network;
    private int queueID;
    private HashSet<NeuralConnection> myConnections = new HashSet<NeuralConnection>();
    private HashSet<NeuralConnection> backConnections = new HashSet<NeuralConnection>();

    public NeuralNode(NeuralLayer layer, NeuralNetwork network, int queueID, float bias)
    {
        this.layer = layer;
        this.network = network;
        this.queueID = queueID;
        this.bias = bias;

        id = queueID + layer.GetLayerID()*100 + network.GetNetworkID()*10000;
    }

    public void PrintNode(){
        Debug.Log("NodeID: " + id + " LayerID: " + layer.GetLayerID() + " NetworkID: " + network.GetNetworkID());
    }

    public override string ToString()
    {

        return "NodeID: " + id + " LayerID: " + layer.GetLayerID() + " NetworkID: " + network.GetNetworkID();
    }

    public void SetFire(bool fire){
        this.isFired = fire;
    }

    public bool GetIsFired(){
        return isFired;
    }

    public float GetBias(){
        return bias;
    }

    public void SetBias(float bias){
        this.bias = bias;
    }

    public void SetConnections(HashSet<NeuralConnection> neuralConnections){
        myConnections = neuralConnections;
    }

    public HashSet<NeuralConnection> GetConnections(){
        return myConnections;
    }
    
    public int GetQueueID(){
        return queueID;
    }

    public int GetLayerID(){
        return layer.GetLayerID();
    }

    public float GetCurrentInput(){
        return currentInput;
    }

    public void SetCurrentInput(float currentInput){
        this.currentInput = currentInput;
    }

    public void AddToCurrentInput(float toAdd){
        this.currentInput += toAdd;
        currentInput=Math.Clamp(currentInput,-1,1);
    }

    public NeuralNetwork GetNetwork(){
        return network;
    }

    public float GetOutput(float weight){
        return currentInput*weight-bias;
    }

    public void AddToBackConnections(NeuralConnection connection){
        backConnections.Add(connection);
    }

    public HashSet<NeuralConnection> GetBackConnections(){
        return backConnections;
    }
}

