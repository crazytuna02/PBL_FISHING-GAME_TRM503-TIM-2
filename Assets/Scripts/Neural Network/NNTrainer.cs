using System;
using UnityEngine;

public class NNTrainer : MonoBehaviour
{
    private NeuralNetwork neuralNetwork;

    // Data pelatihan: {input1, input2, input3} => {output}
    private float[][] trainingInputs = new float[][]
    {
        new float[] { 0.1f, 0.5f, 0 }, // Dekat umpan, lambat, bukan tuna
        new float[] { 0.9f, 0.8f, 1 }, // Jauh dari umpan, cepat, tuna
        new float[] { 0.2f, 0.3f, 0 }, // Dekat, lambat, bukan tuna
        new float[] { 0.7f, 0.7f, 1 }, // Jauh, cepat, tuna
    };

    private float[][] trainingOutputs = new float[][]
    {
        new float[] { 1 }, // Dekati umpan
        new float[] { 0 }, // Abaikan umpan
        new float[] { 1 }, // Dekati umpan
        new float[] { 0 }, // Abaikan umpan
    };

    void Start()
    {
        // Neural Network dengan 3 input, 1 hidden layer (5 neuron), dan 1 output
        neuralNetwork = new NeuralNetwork(new int[] { 3, 5, 1 });

        // Latih Neural Network
        TrainNeuralNetwork();
    }

    private void TrainNeuralNetwork()
    {
        int epochs = 10000; // Jumlah iterasi pelatihan
        float learningRate = 0.1f;

        for (int i = 0; i < epochs; i++)
        {
            for (int j = 0; j < trainingInputs.Length; j++)
            {
                float[] output = neuralNetwork.FeedForward(trainingInputs[j]);
                neuralNetwork.BackPropagate(trainingOutputs[j], learningRate);
            }
        }

        Debug.Log("Pelatihan selesai.");
    }

    public void TestNeuralNetwork(float[] inputs)
    {
        float[] output = neuralNetwork.FeedForward(inputs);
        Debug.Log($"Hasil untuk input {string.Join(", ", inputs)}: {output[0]}");
    }
}
