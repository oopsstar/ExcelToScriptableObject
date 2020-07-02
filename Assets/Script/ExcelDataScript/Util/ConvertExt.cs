using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class ConvertExt
{
    /// <summary>
    /// 
    /// </summary>
    static public object[] Split(string value)
    {
        const char DELIMITER = ',';
        string str = value as string; 
      
        // remove whitespace between each of element
        str = new string(str.ToCharArray()
                            .Where(ch => !Char.IsWhiteSpace(ch))
                            .ToArray());

        // remove ',', if it is found at the end.
        char[] charToTrim = { ',', ' ' };
        str = str.TrimEnd(charToTrim);

        // split by ','
        object[] temp = str.Split(DELIMITER);
        return temp;
    }

    /// <summary>
    /// Convert the given string to array of float. 
    /// Note the string should contain ',' to separate each of array element.
    /// </summary>
    public static float[] ToSingleArray(string value)
    {
        object[] temp = Split(value);
        float[] result = temp.Select(e => Convert.ChangeType(e, typeof(float)))
                             .Select(e => (float)e).ToArray();
        //ERROR: InvalidCastException: Cannot cast from source type to destination type.
        //float[] result = temp.Select(e => (float)e).ToArray();
        return result;
    }

    /// <summary>
    /// Convert the given string to array of double. 
    /// </summary>
    public static double[] ToDoubleArray(string value)
    {
        object[] temp = Split(value);
        double[] result = temp.Select(e => Convert.ChangeType(e, typeof(double)))
                              .Select(e => (double)e).ToArray();
        return result;
    }

    /// <summary>
    /// Convert the given string to array of short. 
    /// </summary>
    public static short[] ToInt16Array(string value)
    {
        object[] temp = Split(value);
        short[] result = temp.Select(e => Convert.ChangeType(e, typeof(short)))
                             .Select(e => (short)e).ToArray();
        return result;
    }

    /// <summary>
    /// Convert the given string to array of int. 
    /// </summary>
    public static int[] ToInt32Array(string value)
    {
        object[] temp = Split(value);
        int[] result = temp.Select(e => Convert.ChangeType(e, typeof(int)))
                            .Select(e => (int)e).ToArray();
        return result;
    }

    /// <summary>
    /// Convert the given string to array of long. 
    /// </summary>
    public static long[] ToInt64Array(string value)
    {
        object[] temp = Split(value);
        long[] result = temp.Select(e => Convert.ChangeType(e, typeof(long)))
                            .Select(e => (long)e).ToArray();          
        return result;
    }

    /// <summary>
    /// Convert the given string to array of long. 
    /// </summary>
    public static string[] ToStringArray(string value)
    {
        object[] temp = Split(value);
        string[] result = temp.Select(e => Convert.ChangeType(e, typeof(string)))
                            .Select(e => (string)e).ToArray();
        return result;
    }


    //Generic
    public static List<float> ToSingleList(string value)
    {
        object[] temp = Split(value);
        List<float> result = temp.Select(e => Convert.ChangeType(e, typeof(float)))
                            .Select(e => (float)e).ToList();
        return result;
    }

    public static List<double> ToDoubleList(string value)
    {
        object[] temp = Split(value);
        List<double> result = temp.Select(e => Convert.ChangeType(e, typeof(double)))
                            .Select(e => (double)e).ToList();
        return result;
    }

    public static List<short> ToInt16List(string value)
    {
        object[] temp = Split(value);
        List<short> result = temp.Select(e => Convert.ChangeType(e, typeof(short)))
                            .Select(e => (short)e).ToList();
        return result;
    }

    public static List<int> ToInt32List(string value)
    {
        object[] temp = Split(value);
        List<int> result = temp.Select(e => Convert.ChangeType(e, typeof(int)))
                            .Select(e => (int)e).ToList();
        return result;
    }

    public static List<long> ToInt64List(string value)
    {
        object[] temp = Split(value);
        List<long> result = temp.Select(e => Convert.ChangeType(e, typeof(long)))
                            .Select(e => (long)e).ToList();
        return result;
    }

    public static List<string> ToStringList(string value)
    {
        object[] temp = Split(value);
        List<string> result = temp.Select(e => Convert.ChangeType(e, typeof(string)))
                            .Select(e => (string)e).ToList();
        return result;
    }

}