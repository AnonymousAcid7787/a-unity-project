using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public struct OptionalField<T>
{
    [SerializeField] public bool enabled;
    [SerializeField] public T value;

    public bool Enabled => enabled;
    public T Value => value;
}
