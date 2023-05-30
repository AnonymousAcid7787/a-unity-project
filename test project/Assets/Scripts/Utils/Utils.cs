using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
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

/** <summary>
* Unmanaged struct that acts as a 3D array, but is a flat NativeArray.
* Use it like a regular 3d array (Ex. array[1, 2, 5])
* this can't be used in components :(
* </summary> 
*/
public struct Flat3DArrayUnmanaged<T> where T : unmanaged {
    NativeArray<T> flatArray;
    int width;
    int height;
    int depth;

    public Flat3DArrayUnmanaged(int width, int height, int depth, Allocator allocator) {

        this.width = width;
        this.height = height;
        this.depth = depth;
        
        flatArray = new NativeArray<T>(width*height*depth, allocator);
    }

    public T this[int x, int y, int z] {

        get {
            return flatArray[x + height * (y + depth * z)];
        }
        set {
            flatArray[x + height * (y + depth * z)] = value;
        }

    }
}