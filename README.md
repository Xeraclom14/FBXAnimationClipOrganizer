# FBX Animation Clip Organizer
An AnimationClip organizer for imported .fbx files for Unity.

![demo](https://github.com/Xeraclom14/FBXAnimationClipOrganizer/assets/58162281/2c8be5e6-c98f-438e-a478-4aa69c9dd724)

Original script by Uberlou (https://pastebin.com/YhxaKdUJ), adapted & modified by Xeraclom14 (https://twitter.com/Xeraclom14).

WARNING: MAKE SURE YOUR METADATA FILES ARE BACKED UP WITH VERSION CONTROL BEFORE USING!!
The script only changes the imported AnimationClip data from the .fbx model file, which is stored within the metadata.

Installation:
Copy the FBXAnimationClipOrganizer.cs script into Scripts\Editor.

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
