using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class NeuralLayer
{
    private NeuralNetwork network;
    private int layerID;
    private int layerSize;

    private HashSet<NeuralNode> nodes;
    private Dictionary<NeuralNode,HashSet<NeuralConnection>> connectionMap = new Dictionary<NeuralNode, HashSet<NeuralConnection>>();

    
    public NeuralLayer(int layerID, int layerSize, NeuralNetwork network){
        this.network = network;
        this.layerID = layerID;
        this.layerSize = layerSize;
        nodes = new HashSet<NeuralNode>();
        for(int i = 0; i<layerSize; i++){
            this.nodes.Add(new NeuralNode(this, network,i+1,UnityEngine.Random.Range(-1.0f,1.0f)));
        }
    }


    public void CreateConnections(NeuralLayer nextLayer){
        foreach(var node in this.nodes){
            HashSet<NeuralConnection> neuralConnections= new HashSet<NeuralConnection>();

            foreach(var nextNode in nextLayer.nodes){
                float weight = UnityEngine.Random.Range(-1.0f,1.0f);
                NeuralConnection connection = new NeuralConnection(node, nextNode, weight);
                neuralConnections.Add(connection);
                nextNode.AddToBackConnections(connection);
            }
            node.SetConnections(neuralConnections);
            connectionMap.Add(node, neuralConnections);
        }
        /*
        Debug.Log("Connectons");
        foreach(var node in connectionMap){
                foreach(var connection in node.Value){
                Debug.Log(connection.ToString());
            }
        }
        */


    }


    public void printLayer(){
        foreach(NeuralNode node in this.nodes){
            node.PrintNode();
        }
    }




    public NeuralLayer(HashSet<NeuralNode> nodes)
    {
        this.nodes = nodes;
    }

    public void AddNode(NeuralNode node)
    {
        nodes.Add(node);
    }

    public void RemoveNode(NeuralNode node)
    {
        nodes.Remove(node);
    }

    public NeuralNode GetNodeByReference(NeuralNode node)
    {
        foreach (var n in nodes)
        {
            if (n == node)
            {
                return n;
            }
        }
        return null; // or throw an exception if not found
    }

    public NeuralNode GetNodeByIndex(int index)
    {
        if (index >= 0 && index < nodes.Count)
        {
            //write
            return nodes.ElementAt(index);
        }
        else
        {
            throw new IndexOutOfRangeException("Index is out of range.");
        }
    }

    public int GetLayerID(){
        return this.layerID;
    }

    public HashSet<NeuralNode> GetNodes(){
        return this.nodes;
    }
}