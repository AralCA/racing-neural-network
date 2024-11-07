using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class NeuralNetwork{
    private HashSet<NeuralLayer> layers = new HashSet<NeuralLayer>();
    int id;

    public NeuralNetwork(int id, int middleLayerSize, int inputSize, int outputSize, int middleLayerCount){
        this.id = id;

        layers.Add(new NeuralLayer(1, inputSize, this));

        for(int i = 0; i < middleLayerCount; i++){
            layers.Add(new NeuralLayer(i+2, middleLayerSize, this));
        }

        layers.Add(new NeuralLayer(middleLayerCount + 2, outputSize, this));
        /*
        foreach(NeuralLayer layer in layers){
            layer.printLayer();
        }
        */
        for(int i = 0; i < layers.Count-1; i++){
            layers.ElementAt<NeuralLayer>(i).CreateConnections(layers.ElementAt<NeuralLayer>(i+1));
        }
    }

    public int GetNetworkID(){
        return id;
    }

    public HashSet<NeuralLayer> GetLayers(){
        return layers;
    }

    public void Process(){
        bool isInput = true;
        foreach(NeuralLayer layer in layers){
            
            if(isInput){
                isInput = false;
                continue;
            }
            
            foreach(NeuralNode node in layer.GetNodes()){
                node.SetCurrentInput(0);
                node.SetFire(false);
            }
        }
        /*
        foreach(NeuralLayer layer in layers){
            foreach(NeuralNode node in layer.GetNodes()){
                if(node.GetConnections()!=null){
                    foreach(NeuralConnection neuralConnection in node.GetConnections()){
                        NeuralNode targetNode = neuralConnection.N2;
                        targetNode.AddToCurrentInput(ReLU(node.GetOutput(neuralConnection.Weight)));
                    }
                }
            }
        }
        */

        //updated

        foreach(NeuralLayer layer in layers){
            foreach(NeuralNode node in layer.GetNodes()){
                foreach(NeuralConnection neuralConnection in node.GetBackConnections())
                    node.AddToCurrentInput(ReLU(neuralConnection.N1.GetCurrentInput()*neuralConnection.Weight-node.GetBias()));   
            }
        }
    }

    public void Tease(float factor){
        foreach(NeuralLayer layer in layers){
            foreach(NeuralNode node in layer.GetNodes()){
                node.SetBias(UnityEngine.Random.Range(node.GetBias()-0.1f*factor,node.GetBias()+0.1f*factor));
            }
        }
    }

    public static float sigmoid(float x){
        return 1/(1+Mathf.Exp(-x));
    }

    public static float dSigmoid(float x){
        return sigmoid(x) *(1-sigmoid(x));
    }

    public static float ReLU(float x){
        return Math.Max(x/10,x);
    }

    public static float dReLU(float x){
        return x > 0? 1 : 0;
    }

    public NeuralNetwork CloneNetwork(NeuralNetwork network){
        NeuralNetwork clone = new NeuralNetwork(network.id, network.layers.ElementAt<NeuralLayer>(1).GetNodes().Count, network.layers.ElementAt<NeuralLayer>(0).GetNodes().Count,network.layers.ElementAt<NeuralLayer>(network.layers.Count-1).GetNodes().Count, network.layers.Count-2);
        
        foreach(NeuralLayer layer in network.layers){
            foreach(NeuralNode node in layer.GetNodes()){
                NeuralNode cloneNode = clone.GetLayers().ElementAt<NeuralLayer>(layer.GetLayerID()-1).GetNodes().ElementAt(node.GetQueueID()-1);
                cloneNode.SetBias(node.GetBias());
            }
        }

        return clone;
    }

}