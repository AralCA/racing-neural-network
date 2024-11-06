using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] GameObject charPrefab;
    [SerializeField] int size = 10;

    private void Start() {
        for(int i = 0; i < size; i++){
            Instantiate(charPrefab, this.transform);
        }
    }

    public void Restart(NeuralNetwork neuralNetwork){

        foreach(Transform child in transform){
            neuralNetwork.Tease(0.5f);
            child.GetComponent<CharacterDemo>().neuralNetwork = neuralNetwork.CloneNetwork(neuralNetwork);

        }
    }
}
