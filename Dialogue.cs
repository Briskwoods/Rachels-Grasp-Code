using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    // Use an array to allow for multiple names to be used in the dialogue box to simulate a conversation with multiple people.
    // If the array size is left to 1, then it will use that 1 name throughout the whole conversation
    // If the array size is over 3, do not leave the any value in the array  blank as it will show up as a blank in the name field of the dialogue box.
    // Multi-use innit?
    public string[] name;

    [TextArea(3, 10)]
    public string[] sentences;
}
