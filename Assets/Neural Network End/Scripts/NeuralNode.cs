using System;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNode
{
    private int id;
    private NeuralLayer layer;
    private NeuralNetwork network;

    public NeuralNode(NeuralLayer layer, NeuralNetwork network, int queueID)
    {
        this.layer = layer;
        this.network = network;

        id = queueID + layer.GetLayerID()*100 + network.GetNetworkID()*10000;
    }

    public void PrintNode(){
        Debug.Log("NodeID: " + id + " LayerID: " + layer.GetLayerID() + " NetworkID: " + network.GetNetworkID());
    }

    public override string ToString()
    {

        return "NodeID: " + id + " LayerID: " + layer.GetLayerID() + " NetworkID: " + network.GetNetworkID();
    }
}

