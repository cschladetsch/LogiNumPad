# LogiNumLock ![Logo](res/Logo.jpg)

This is a simple console application that changes the color of the numpad keys when the `NumLock` key is toggled. This was a real oversight by Logitech, but hey at least they gave us an Sdk to make fixes like this. Kudos for that at least.

It's a very simple app. Currently, you just run it as a console application, and press `Enter` to close it.

If there's any interest I could improve it:

* Use existing colors. This would require you supplying the config name used in G-HUB. There's no way I could see to either:
  * Get the current config name
  * Get the current state/color of a key (probably because they can animate)
* Make it a Windows Service rather than a console application.

I am not inclined to do these things because a) I already run a few other custom console apps and I don't care about another and b) I just hard-coded the colors I wanted.

Drop me a line or leave an Issue if you want.

There's a binary release as well.

