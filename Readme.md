# LogiNumLock ![Logo](res/Logo.jpg)
[![Build status](https://ci.appveyor.com/api/projects/status/github/cschladetsch/LogiNumPad?svg=true)](https://ci.appveyor.com/project/cschladetsch/LogiNumPad)
[![CodeFactor](https://www.codefactor.io/repository/github/cschladetsch/LogiNumPad/badge)](https://www.codefactor.io/repository/github/cschladetsch/LogiNumPad)
[![License](https://img.shields.io/github/license/cschladetsch/LogiNumPad.svg?label=License&maxAge=86400)](./LICENSE)
![Release](https://img.shields.io/github/release/cschladetsch/LogiNumPad.svg?label=Release&maxAge=60)

This is a simple console application that changes the color of the numpad keys when the `NumLock` key is toggled. This was a real oversight by Logitech, but hey at least they gave us an Sdk to make fixes like this. Kudos for that at least.

## Important
Running this app seems to stop the G-HUB app from running. A simple work-around for the moment is to just close the `LogiNumLock` before running G-Hub and starting it after.

## Usage
It's a very simple app. Currently, you just run it as a console application, and press `Enter` to close it.

If there's any interest I could improve it:

* Use existing colors. This would require you supplying the config name used in G-HUB. There's no way I could see to either:
  * Get the current config name
  * Get the current state/color of a key (probably because they can animate)
* Make it a Windows Service rather than a console application.

I am not inclined to do these things because a) I already run a few other custom console apps and I don't care about another and b) I just hard-coded the colors I wanted.

Drop me a line or leave an Issue if you want.

There's a binary release as well.

