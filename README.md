# FBX Animation Clip Organizer
An AnimationClip organizer for imported .fbx files for Unity.

Original script by Uberlou (https://pastebin.com/YhxaKdUJ), adapted & modified by Xeraclom14 (https://twitter.com/Xeraclom14).

WARNING: MAKE SURE YOUR METADATA FILES ARE BACKED UP WITH VERSION CONTROL BEFORE USING!!
The script only changes the imported AnimationClip data from the .fbx model file, which is stored whitin the metadata.

Installation:
Copy the FBXAnimationClipOrganizer.cs script into Scripts\Editor.
 
Usage:
1) To open, go to Window -> FBX Animation Organizer
2) Drag and drop your .fbx model file into the "FBX File" field.
3) Reorder the Animation clips in the list to your liking, or alternatively sort them alphabetically.
4) Press "Save Changes" to update your changes.

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
