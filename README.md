# MultiRun
Build and run one or more instances of your Unity game easily.

## Features
* Build and Run up to 4 instances of your game with just one click! _(technically two clicks; menu then menu item.  It could be one keystroke though, if you made a shortcut for it, which you can do easily)_
* A log file for each instance.
* Add command line arguments for all instances.
* Add command line arguments per instance.
* Create a build that runs the current scene first, without having to edit the build settings.
* Automatically resize and position running instances (wihtout having to change player settings).
* Automatically disable `Debug.Log`'s stack trace for instance logs.
* View log contents directly in Unity (very very alpha, could be a fool's errand).

For a full list of Unity command line arguments see:  https://docs.unity3d.com/Manual/PlayerCommandLineArguments.html




# Install
Install using the Package Manager and "Add package from git URL" using the the URL below:
```
https://github.com/ProfButch/MultiRun.git?path=Packages/MultiRun
```




# Setup
Configure a directory and file name to build to, using MultiRun->Settings and you're ready to go.




# Logviewer (alpha)
The Log Viewer is rough.  Unity evidently can't display more than  ~3k characters in a textbox?  [65k vertex limit](https://forum.unity.com/threads/ui-text-character-limit.359729/)?  It's an super fun juggling act of random file access and reading chunks of text and adding labels and appending to labels and finding new lines and removing new lines and other antics.  Looking for ideas, if you have one add an issue.

Also, everytime scripts get compiled the contents of the Log Viewer are cleared.




# Known Issues

## Instances not resized or debug stack trace not disabled
Some of the options in the settings are performed by MultiRun when the instance starts.  This usually happens automatically by the `RuntimeInitializeOnLoadMethod` method `MultiRun.Runner.InitializeOnLoad`.

Sometimes this doesn't get kicked off.  If that is the case, you can call this method directly (multiple calls do not do anything).  In your own `RuntimeInitializeOnLoadMethod` or the `Awake`/`Start` of your first scene call:

```c#
MultiRun.Runner.InitializeOnLoad();
```