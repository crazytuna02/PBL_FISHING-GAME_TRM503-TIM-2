using System;

public class NeuralNetwork
{
    private int[] layers; // Struktur layer
    private float[][] neurons; // Nilai setiap neuron
    private float[][][] weights; // Bobot koneksi antar neuron

    public NeuralNetwork(int[] layers)
    {
        this.layers = layers;
        InitializeNeurons();
        InitializeWeights();
    }

    private void InitializeNeurons()
    {
        neurons = new float[layers.Length][];
        for (int i = 0; i < layers.Length; i++)
        {
            neurons[i] = new float[layers[i]];
        }
    }

    private void InitializeWeights()
    {
        weights = new float[layers.Length - 1][][];
        for (int i = 0; i < layers.Length - 1; i++)
        {
            weights[i] = new float[layers[i]][];
            for (int j = 0; j < layers[i]; j++)
            {
                weights[i][j] = new float[layers[i + 1]];
                for (int k = 0; k < layers[i + 1]; k++)
                {
                    weights[i][j][k] = UnityEngine.Random.Range(-1f, 1f); // Inisialisasi bobot secara acak
                }
            }
        }
    }

    public float[] FeedForward(float[] inputs)
    {
        neurons[0] = inputs; // Masukkan input ke layer pertama
        for (int i = 1; i < layers.Length; i++)
        {
            for (int j = 0; j < layers[i]; j++)
            {
                float value = 0f;
                for (int k = 0; k < layers[i - 1]; k++)
                {
                    value += neurons[i - 1][k] * weights[i - 1][k][j];
                }
                neurons[i][j] = (float)Math.Tanh(value); // Aktivasi fungsi TanH
            }
        }
        return neurons[layers.Length - 1]; // Output layer
    }

    public void BackPropagate(float[] expected, float learningRate)
    {
        // Hitung error untuk output layer
        float[] outputErrors = new float[neurons[layers.Length - 1].Length];
        for (int i = 0; i < outputErrors.Length; i++)
        {
            float output = neurons[layers.Length - 1][i];
            outputErrors[i] = (expected[i] - output) * TanHDerivative(output);
        }

        // Hitung error untuk hidden layers
        for (int i = layers.Length - 2; i >= 0; i--)
        {
            float[] layerErrors = new float[neurons[i].Length];
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float error = 0f;
                for (int k = 0; k < neurons[i + 1].Length; k++)
                {
                    error += outputErrors[k] * weights[i][j][k];
                }
                layerErrors[j] = error * TanHDerivative(neurons[i][j]);
            }
            outputErrors = layerErrors;

            // Update bobot
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    weights[i][j][k] += neurons[i][j] * outputErrors[k] * learningRate;
                }
            }
        }
    }

    private float TanHDerivative(float value)
    {
        return 1 - value * value; // Turunan fungsi TanH
    }
}
