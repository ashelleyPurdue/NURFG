# NURFG: NUnit Runner For Godot

This is an editor plugin for Godot 3.4 mono.  As the name suggests, it locates
and runs NUnit tests.

![screenshot](screenshot.png "screenshot")

# The Catch
As of version 3.4, Godot does not support solutions with multiple .csproj files.
As a consequence, _you will need to put your tests in the same project as your
production code_.  This means that the code for your tests will end up shipping
with the final game.

The good news is that there are some open proposals that, if implemented, would
remove and/or alleviate this limitation.  Check them out!
* [Support multiple .NET5 SDK-style projects per Godot project #1141](https://github.com/godotengine/godot-proposals/issues/1141)
* [C#: Allow project source files to be separated into a sub project folder. #2646](https://github.com/godotengine/godot-proposals/issues/2646)
* [Make it possible to change the .sln and .csproj location in a project #1575](https://github.com/godotengine/godot-proposals/issues/2646)

It _is_ technically possible to have a separate NUnit project that has a reference
to a Godot project.  However, doing so will result in the tests crashing the
moment they try to reference a Godot class...which makes them rather useless.
For now, you're better off keeping them in the same project and running them
from within the Godot editor.

# Installation
First, you need to install the NUnit nuget package in your game's .csproj file.
Yes, you read that correctly.  NUnit will end up shipping with your game's
executable.  See [the catch](#the-catch).

After you've done that, clone this repo and copy/paste the "addons" folder into
your project.

Finally, enable the plugin from the project settings window.

# Project organization
As previously stated, your unit tests will reside in the same project as your
main game code.  However, this does not stop you from using namespaces!
I recommend having a separate namespace for your tests and your production code.
That way you can at least _pretend_ that they're in separate projects.