/*******************

FBX AnimationClip Organizer 1.2
Original script by Uberlou (https://pastebin.com/YhxaKdUJ), adapted & modified by Xeraclom14 (https://twitter.com/Xeraclom14).

WARNING: MAKE SURE YOUR METADATA FILES ARE BACKED UP WITH VERSION CONTROL BEFORE USING!!
This script only changes the imported AnimationClip data from the .fbx model file, which is stored within the metadata.

Installation:
Copy this C# script into Scripts\Editor.

Usage:
1) To open, go to Window -> FBX Animation Organizer
2) Drag and drop your .fbx model file into the "FBX File" field.
3) Reorder the Animation clips in the list to your liking, or alternatively sort the list.
4) Press "Save Changes" to update your changes.

Changes in version 1.3:
- Fixed a bug that caused various elements from the rest of the layout UI text to be right-aligned.

Changes in version 1.2:
- Added label to show clip duration.
- Added functionality to sort A-Z, Z-A and by clip duration (ascending and descending order).

Changelog from old version by Uberlou:
- Input now only requires to drag and drop the .fbx file directly. (No longer requires inputting file path then manually search for metadata in dropdowns).
- Added button to reload file.
- Animation list is now displayed in a reorderable list instead of manually moving items up/down via buttons.
- Added a scrollbar to the animations list.
- Added functionality to sort Animation Clips alphabetically.
- Data is automatically re-imported on file load / save.
- Editor is automatically refreshed upon saving.
- Fixed performance issues regarding old input method.
- Fixed overwrite metadata bugs.
 
******************/

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditorInternal;

public class FBXAnimationClipOrganizer : EditorWindow
{
    Object fileObject;
    string filePath = "Assets\\";
    string metaPath = "";
    List<string> metadataHeader = new List<string>();
    List<string> metadata = new List<string>();
    List<List<string>> animationClips = new List<List<string>>();
    Vector2 scroll = Vector2.zero;
    ReorderableList reorderableList;

    [MenuItem("Window/FBX AnimationClip Organizer")]

    static void Init()
    {
        var window = GetWindow(typeof(FBXAnimationClipOrganizer));
        window.titleContent = new GUIContent("FBX AnimationClip Organizer");
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("", GUILayout.Height(10f));

        fileObject = EditorGUILayout.ObjectField("FBX File:", fileObject, typeof(Object), false);

        string oldFilePath = filePath;
        filePath = AssetDatabase.GetAssetPath(fileObject);

        metaPath = filePath + ".meta";

        if (oldFilePath != filePath)
        {
            ClearData();
        }

        if (filePath.ToLower().EndsWith(".fbx") && File.Exists(filePath) && File.Exists(metaPath))
        {
            if (oldFilePath != filePath || animationClips.Count == 0)
            {
                ImportData();
            }

            GUILayout.BeginHorizontal();

            if (animationClips.Count == 0)
            {
                GUILayout.Label("File doesn't contain animation clips!");

                GUILayout.EndHorizontal();
                return;
            }

            if (GUILayout.Button("Reload File", GUILayout.Height(30)))
            {
                filePath = "";
            }

            //Overwrite Metadata Button
            if (GUILayout.Button("Save Changes", GUILayout.Height(30)))
            {
                using StreamWriter sw = new StreamWriter(metaPath, false);
                //Copy in the animations
                for (int i = animationClips.Count() - 1; i >= 0; i--)
                {
                    foreach (string entry in animationClips[i])
                        metadata.Insert(0, entry);
                }
                //Copy in the header
                for (var i = metadataHeader.Count() - 1; i >= 0; i--)
                {
                    metadata.Insert(0, metadataHeader[i]);
                }
                //Write all the lines into the metadata file
                for (int i = 0; i < metadata.Count(); i++)
                {
                    sw.WriteLine(metadata[i]);
                }
                sw.Close();

                Debug.Log("Saved metadata file: " + metaPath);

                AssetDatabase.Refresh();
                ImportData();

            }
            GUILayout.EndHorizontal();
        }
        else
        {
            if (!File.Exists(filePath))
            {
                GUILayout.Label("Error: File does not exist!");
            }
            else if (filePath.ToLower().EndsWith(".blend"))
            {
                GUILayout.Label("This is a blender file, silly.");
            }
            else if (!filePath.ToLower().EndsWith(".fbx"))
            {
                GUILayout.Label("Error: Not an FBX file.");
            }
            else if (!File.Exists(metaPath))
            {
                GUILayout.Label("Error: File does not have metadata.");
            }

            return;
        }

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Sort A-Z", GUILayout.Height(30)))
        {
            SortAZ();
        }

        if (GUILayout.Button("Sort Z-A", GUILayout.Height(30)))
        {
            SortZA();
        }

        if (GUILayout.Button("Sort by duration \u2193", GUILayout.Height(30)))
        {
            SortAZ();
            SortByDuration();
        }

        if (GUILayout.Button("Sort by duration \u2191", GUILayout.Height(30)))
        {
            SortZA();
            SortByDuration();
            animationClips.Reverse();
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUIStyle style = new GUIStyle(GUI.skin.label);

        GUILayout.Label("Animation Clip Count: " + animationClips.Count, style);
        style.alignment = TextAnchor.MiddleRight;
        GUILayout.Label("Clip duration (in frames)   ", style);

        GUILayout.EndHorizontal();

        scroll = GUILayout.BeginScrollView(scroll);

        reorderableList.DoLayoutList();

        GUILayout.EndScrollView();
    }

    private void ImportData()
    {
        ClearData();
        //Read the input file into a list and copy them to another list for editing
        var logFile = File.ReadAllLines(filePath + ".meta");
        metadata = new List<string>(logFile);
        //Get the header of the metadata file
        int pTo = metadata.IndexOf("    clipAnimations:");
        for (int x = 0; x <= pTo; x++)
        {
            metadataHeader.Add(metadata[0]);
            metadata.RemoveAt(0);
        }
        //Get all the animation data
        for (int i = 0; i <= 1000; i++)
        {
            //Get the start and end points of the animation entry   
            int pFrom = metadata.IndexOf("    - serializedVersion: 16");
            pTo = metadata.IndexOf(metadata.Where(x => x.Contains("additiveReferencePoseFrame")).FirstOrDefault());
            //Exit when you get the last entry
            if (pFrom < 0) break;
            //Add the names and animation entries to the respective lists
            //Save the animation entry and remove from the edited metadata
            List<string> animData = new List<string>();
            for (int x = 0; x <= pTo - pFrom; x++)
            {
                animData.Insert(0, metadata[pFrom]);
                metadata.RemoveAt(pFrom);
            }
            animationClips.Add(animData);

        }

        reorderableList = new ReorderableList(animationClips, animationClips.GetType().GetGenericArguments().Single(), true, false, false, false);
        reorderableList.drawElementCallback = OnDrawListItem;
    }

    private void SortAZ()
    {
        for (int i = 0; i < animationClips.Count(); i++)
        {
            int firstIndex = i;
            string currentfirst = GetAnimationDataName(animationClips[i]);

            for (int j = i; j < animationClips.Count() - 1; j++)
            {
                string animName = GetAnimationDataName(animationClips[j + 1]);

                if (string.Compare(currentfirst, animName, false) > 0)
                {
                    currentfirst = animName;
                    firstIndex = j + 1;
                }
            }

            if (firstIndex > i)
            {
                // swap anims
                List<string> temp;
                temp = animationClips[i];
                animationClips[i] = animationClips[firstIndex];
                animationClips[firstIndex] = temp;
            }
        }
    }

    private void SortZA()
    {
        SortAZ();
        animationClips.Reverse();
    }

    private void SortByDuration()
    {
        for (int i = 0; i < animationClips.Count(); i++)
        {
            int lowestIndex = i;
            int lowest = GetAnimationDataDuration(animationClips[i]);

            for (int j = i; j < animationClips.Count() - 1; j++)
            {
                int duration = GetAnimationDataDuration(animationClips[j + 1]);

                if (duration < lowest)
                {
                    lowest = duration;
                    lowestIndex = j + 1;
                }
            }

            if (lowestIndex > i)
            {
                // swap anims
                List<string> temp;
                temp = animationClips[i];
                animationClips[i] = animationClips[lowestIndex];
                animationClips[lowestIndex] = temp;
            }
        }
    }

    //Resets all the data
    private void ClearData()
    {
        animationClips.Clear();
        metadataHeader.Clear();
        metadata.Clear();
    }

    private string GetAnimationDataName(List<string> animData)
    {
        return animData[animData.Count - 2].TrimStart().Replace("name: ", "");
    }

    private int GetAnimationDataDuration(List<string> animData)
    {
        int init = int.Parse(animData[animData.Count - 5].TrimStart().Replace("firstFrame: ", ""));
        int end = int.Parse(animData[animData.Count - 6].TrimStart().Replace("lastFrame: ", ""));

        return end - init;
    }

    private void OnDrawListItem(Rect rect, int index, bool isActive, bool isFocused)
    {
        string prefix = (index + ":").PadRight(6);

        GUIStyle style = EditorStyles.label;

        style.alignment = TextAnchor.MiddleRight;
        EditorGUI.LabelField(rect, GetAnimationDataDuration(animationClips[index]).ToString(), style);
        style.alignment = TextAnchor.MiddleLeft;
        EditorGUI.LabelField(rect, prefix + GetAnimationDataName(animationClips[index]), style);
    }
}