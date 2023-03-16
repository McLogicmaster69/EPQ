using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Networks
{
    public class NeuralNetwork
    {
        private float[] Inputs;
        private List<NetworkLayer> Layers;
        private List<LinkLayer> Links;

        public float[] Outputs { get; private set; }

        public NeuralNetwork(int inputSize, int outputSize)
        {
            Inputs = new float[inputSize];
            Outputs = new float[outputSize];
            Layers = new List<NetworkLayer>();
            Links = new List<LinkLayer>();
            Links.Add(new LinkLayer());
        }
    }
    public class NetworkLayer
    {
        public List<float> Nodes;
        public NetworkLayer()
        {
            Nodes = new List<float>();
        }
    }
    public class LinkLayer
    {
        public List<Link> InnerLinks;
        public LinkLayer()
        {
            InnerLinks = new List<Link>();
        }
    }
    public class Link
    {

    }
}