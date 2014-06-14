SkypeManager
============

This project is basically a BOT for Skype. Commands sent on a chat with/to the person running the BOT will trigger it.

A command is a method in a "*.Provider.dll" file which should be located in "Providers" folder on the application's folder.

    SkypeManager.exe
    Providers
    |- DefaultProvider.dll

To trigger a command you must send the message with the command preffix set on SkypeCommands class followed by the method name. Example:

    //Youtube

This input will trigger an event on the BOT which will load all "*Provider.dll" files and search for the method "Youtube" and then execute it in a new thread.

To create new commands you can modify one of the existing providers in this solution and add a new method, for example:

    public void WhatTimeIsIt(Command cmd)
    {
        cmd.ReplyChat(DateTime.Now.ToString("HH:mm:ss"));
    }

Or you can create a new provider by creating a new Class Library (its name must end with "Provider") and add a reference to SkypeCommander project. Your provider class must inherit from `CommandProvider`. After compiling remember to copy the DLL to the "Providers" folder, or go to the project settings > build > and change the outpath to the providers folder directly.