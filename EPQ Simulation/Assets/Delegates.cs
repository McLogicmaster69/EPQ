using EPQ.Colors;
using EPQ.Foodweb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ
{
    public delegate void ColorEvent(object source, ColorChangeArgs e);
    public delegate void TickEvent(object source, TickEventArgs e);
    public delegate void LoadMethod<T>(T file);
    public delegate void MoveEvent(object sender, MoveEventArgs e);
    public delegate void ScaleEvent(object sender, ScaleEventArgs e);
}