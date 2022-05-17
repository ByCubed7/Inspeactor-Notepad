#if UNITY_EDITOR
using UnityEngine;
using System;
using System.IO;
using UnityEditor;

public class NotepadWindow : EditorWindow
{
    public int fontSize;
    public Texture icon;

    private static readonly string windowTitle = "Inspector Notepad";
    private static NotepadWindow window;

    private Notepad notepad;

    private Vector2 scroll;
    private GUIStyle style;


    [MenuItem("Window/Inspector Notepad")]
    static void Init(){
        window = (NotepadWindow) EditorWindow.GetWindow(typeof(NotepadWindow), false, windowTitle);
        //window.maxSize = new Vector2( 200, 100 );
        window.saveChangesMessage = "This window has unsaved changes. Would you like to save?";
        window.Show();
    }

    public void OnEnable() {
        notepad = new Notepad();
        //titleContent.text = windowTitle;
        titleContent.image = icon;
        titleContent.tooltip = "A place to write down notes for your project!";
    }

	//public void OnDestroy() { Save(); }
    //public void OnApplicationQuit() { Save(); }
    //public void OnLostFocus() { Save(); }
    //public void OnFocus() { Load(); }

    public void OnGUI() {
        //saveChangesMessage = EditorGUILayout.TextField(saveChangesMessage);
        EditorGUILayout.LabelField(notepad.HasChanged ? "I have changes!" : "No changes.", EditorStyles.wordWrappedLabel);

        // On Font Change, Edit the font
        fontSize = EditorGUILayout.IntSlider("Font Size", fontSize, 11, 18);
        style = new GUIStyle(GUI.skin.textArea);
        style.wordWrap = true;
        style.richText = true;
        style.fontSize = fontSize;

        // Create a scrollable textbox
        scroll = EditorGUILayout.BeginScrollView(scroll, false, false);

        // Track changes in the TextArea
        EditorGUI.BeginChangeCheck();

        // Editable Textbox
        string newContents = WithoutSelectAll(() => EditorGUILayout.TextArea(notepad.Contents, style, GUILayout.MaxHeight(position.height - 60)));

        // Stop tracking changes
        if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(this, "Change Note");
            notepad.Contents = newContents;
			EditorUtility.SetDirty(this);
		}

        // End the scrollable textbox
        EditorGUILayout.EndScrollView();

        // Only when the notepad has changed
        using (new EditorGUI.DisabledScope(!notepad.HasChanged))
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save")) notepad.Save();
            if (GUILayout.Button(notepad.HasChanged ? "Discard" : "Load")) {
                notepad.Load();
                GUI.FocusControl(null); // Contents has changed, unfocus
            }
            GUILayout.EndHorizontal();
        }

        // Sync changes
        hasUnsavedChanges = notepad.HasChanged;

    }

    public override void SaveChanges()
    {
        // Save to file
        notepad.Save();

        Debug.Log($"{this} saved successfully!");
        base.SaveChanges();
    }

    public override void DiscardChanges()
    {
        // Only discard if we actually have unsaved changes
        if (notepad.HasChanged) {
            // Load from last save file
            //notepad.Load();
        }

        Debug.Log($"{this} discarded savedata!");
        base.DiscardChanges();
    }

    // Internal Screaming
    // https://stackoverflow.com/questions/44097608/how-can-i-stop-immediate-gui-from-selecting-all-text-on-click
    // Thanks Thomas Hilbert for this great workaround!
    private T WithoutSelectAll<T>(Func<T> guiCall)
    {
        bool preventSelection = (Event.current.type == EventType.MouseDown);

        Color oldCursorColor = GUI.skin.settings.cursorColor;

        if (preventSelection)
            GUI.skin.settings.cursorColor = new Color(0, 0, 0, 0);

        T value = guiCall();

        if (preventSelection)
            GUI.skin.settings.cursorColor = oldCursorColor;

        return value;
    }


}

#endif
