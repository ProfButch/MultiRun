# MultiRun
Build and run one or more instances of your Unity game easily.

## Features
* Generate a Build then Run up to 4 instances of your game with just one click! _(technically two clicks; menu then menu item.  It could be one keystroke though, if you made a shortcut for it, which you can do easily)_
* A log file for each instance.
* Add command line arguments for each instance or all instances.
* Create a build that runs the current scene first, without having to edit the build settings.
* Automatically resize and position running instances (wihtout having to change player settings).
* Automatically disable `Debug.Log`'s stack trace for instance logs.
* Watch instance logs directly in Unity (just the end though, Unity hates text).

For a full list of Unity command line arguments see:  https://docs.unity3d.com/Manual/PlayerCommandLineArguments.html


## Install
Install using the Package Manager and "Add package from git URL" using the the URL below:
```
https://github.com/ProfButch/MultiRun.git?path=Packages/MultiRun
```
Once installed you'll see the `MultiRun` menu in the menu bar.


## Setup
Configure a directory and file name to build to (`MultiRun -> Settings`) and you're ready to go.

<br>
<br>
<br>
<br>

# Building and Running
It mostly works exactly how you'd expect, but there's some nuances.

## Building
There are two build paths, one which is stored at the Editor level which spans all projects, and another that you can set per proeject.  You must have at least one of these set to enable the menus.  The project build path takes precedence.

Once Set you can use `Build`, `Build Current Scene` or `Build and Run` to generate a build.

### Build Current Scene
`Build Current Scene` will generate a build with the current scene as the scene to be run first.  This only affects the build that is generated with this menu option.  The scene does not need to be in the Build Settings.

When using `Build Current Scene` use `Run -> [1-4]` menu item and NOT `Build and Run` since that will overwite the build.


## Running
You can build and run up to 4 instances in one step using `Build and Run -> [1-4]`.  If you already have a build (such as when using `Build Current Scene`) you can use `Run -> [1-4]` to launch up to 4 instaces of your game.

__Important__ Instances are launched with the `--logfile` argument, and the log file name is specific to each instance number.  This means that if you launch more instances while other instances are still running then they will write to the same log file.  For example, if you launch 4 instances, then launch one additional instance then instance 1 and 5 will be writing to the same log.


## Misc
All settings are saved in the `UserSettings/MultiRun.yaml`

<br>
<br>
<br>
<br>

# Log Watcher (alpha)
The Log Watcher allows you to monitor the end of each of the instance logs.  Unity does not like to display large amounts of text (or what others might consider "a not small, but by no means large" amount of text), so you will still need another tool to view logs in their entirety.  You can use `Multi Run -> Open build directory` to quickly get to your log files.

The Log Watcher has some amount of smarts in it to ignore the large dump of memory allocation stats that Unity leaves at the end of logs.  It mostly works, but it could be better.  It uses crazy magic.  If you notice that you have a `DontDestroyOnLoad` object called `MultiRunGameObject`, this is part of the magic to detect the application closing.

Due to a layout refresh issue, the Log Watcher is automatically closed everytime scripts are recompiled.  You can assign a keyboard shortcut to the Log Watcher menu item to make this less annoying.

<br>
<br>
<br>
<br>

# Known Issues

## Instances not resized or debug stack trace not disabled
Some of the options in the settings are performed by MultiRun when the instance starts.  This usually happens automatically by the `RuntimeInitializeOnLoadMethod` method `MultiRun.Runner.InitializeOnLoad`.

Sometimes this doesn't get kicked off.  If that is the case, you can call this method directly (multiple calls do not do anything).  In your own `RuntimeInitializeOnLoadMethod` or the `Awake`/`Start` of your first scene call:

```c#
MultiRun.Runner.InitializeOnLoad();
```