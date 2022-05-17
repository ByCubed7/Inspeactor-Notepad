#if UNITY_EDITOR
using UnityEngine;
using System;
using System.IO;
using UnityEditor;

class Notepad
{
    private string contents = "";
    public string Contents
    {
        get { return contents; }
        set { contents = value; hasChanged = true; }
    }

    private string directory = "";
    public string Directory
    {
        get { return directory; }
        private set { directory = value; }
    }

    private bool hasChanged = false;
    public bool HasChanged
    {
        get { return hasChanged; }
        private set { hasChanged = value; }
    }

    // - - - - - - - - - - - - - - - - - - - - - - - - - -
    // Constructor

    public Notepad() {
        directory = Application.dataPath;
        directory = Path.Combine(directory, "ByCubed7");
        directory = Path.Combine(directory, "Editor");
        directory = Path.Combine(directory, "NotePad");
        directory = Path.Combine(directory, "Notes.txt");

        Load();
    }

    // - - - - - - - - - - - - - - - - - - - - - - - - - -
    // Public Methods

    public void Save() { File.WriteAllText(directory, contents); hasChanged = false; }
    public void Load() { contents = File.ReadAllText(directory); hasChanged = false; Debug.Log("Loaded"); }

}

#endif
