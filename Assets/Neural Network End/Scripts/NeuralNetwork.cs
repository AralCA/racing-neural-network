using System;
using System.Collections.Generic;
using System.Linq;

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

        foreach(NeuralLayer layer in layers){
            layer.printLayer();
        }

        for(int i = 0; i < layers.Count-1; i++){
            layers.ElementAt<NeuralLayer>(i).CreateConnections(layers.ElementAt<NeuralLayer>(i+1));
        }
    }

    public int GetNetworkID(){
        return id;
    }
}