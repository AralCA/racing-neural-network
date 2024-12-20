using UnityEngine;

public class NeuralConnection
{
    private NeuralNode n1;
    private NeuralNode n2;
    private float weight;

    public NeuralNode N1
    {
        get { return n1; }
        set { n1 = value; }
    }

    public NeuralNode N2
    {
        get { return n2; }
        set { n2 = value; }
    }

    public float Weight
    {
        get { return weight; }
        set { weight = Mathf.Clamp(value,-1,1); }
    }

    public NeuralConnection(NeuralNode n1, NeuralNode n2, float weight){
        this.n1 = n1;
        this.n2 = n2;
        this.weight = weight;
    }
    public override string ToString(){
        return "Node1: " + n1 + " Node2: " + n2 + " Connection with weight: " + weight;

    }
}