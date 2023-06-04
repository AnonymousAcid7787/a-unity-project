using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;

public struct Utils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Flat2DArrayIndex(int arrayHeight, int x, int y) {
        return x + arrayHeight*y;
    }
    

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


    /** <summary>
    * Creates a new flat version of the inputted <paramref name="array"/>.
    * </summary> 
    */
    public static void Flatten2DArray<T>(T[,] array, Allocator allocator, out NativeArray<T> flatArray2D) where T : unmanaged {
        int width = array.GetLength(0);
        int height = array.GetLength(1);
        
        flatArray2D = new NativeArray<T>(width*height, allocator);

        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                flatArray2D[x + height*y] = array[x, y];
            }
        }
    }
}

/** <summary>
* Unmanaged struct that acts as a 3D array, but is a flat NativeArray.<br />
* Use it like a regular 3d array (Ex. array[1, 2, 5])<br />
* This can't be used in components, so this might be deleted eventually
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