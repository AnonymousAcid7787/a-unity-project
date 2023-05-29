using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

public struct Utils
{
    //inline this method if possible
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] Flatten3DArray<T>(T[,,] array3D, int w, int h, int d) {
        T[] array = new T[w*h*d];
        
        for(int x = 0; x < w; x++){
            for(int y = 0; y < h; y++){
                for(int z = 0; z < d; z++){
                    array[x + h * (y + d * z)] = array3D[x, y, z];
                }
            }
        }

        return array;
    }
}
