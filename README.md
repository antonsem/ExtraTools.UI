# ExtraTools.UI

A UI system I find very useful. An example implementation can be found here:
https://github.com/antonsem/extraTools.UI.Example. And here is a blog explaining
my thought process: https://www.anton.website/ui-manager-a-better-one

## Summary

The primary purpose of this package is to separate the visuals of the UI from the functionality
and the data while providing easy access to every screen. This package does not provide
any implementations, only abstract base classes. The basic functionality, such as showing
or hiding screens, is provided by the base classes.

## Documentation

### UI Manager

Provides access to screens, dialogs, and widgets. Internally handles the screen
switching.

Example:

```csharp

[SerializedField] private UIManager _uiManager;

private void Start()
{
    MainMenuScreen mainMenuScreen = _uiManager.GetScreen<MainMenuScreen>();
    mainMenuScreen.Show();
}
```

In case `MainMenuScreen` needs additional data a `Show()` method overrides with required
parameters can be introduced in the implementation.

`UIManager` also provides access to dialogs and widgets.

### Screen

Represents a single screen in the UI. A single screen can have multiple panels. Only
child classes can have access to panels.

### Screen UI

Responsible for relaying input to the screen and visual presentation of a screen

### Panel

Represents a part of a screen. Not mandatory, but useful if only a part of the screen needs
to change. An example can be a settings screen with multiple tabs for different settings.

### Dialog

Represents a pop-up dialog window. Useful to inform the user or to ask an immediate
question (e.g. "Are you sure you want to quit?"). Dialogs can stack up or be shown one at
a time.

Requires a text and an answer option to set up.

### Widget

Represents a notification or a short message. Can be clicked on and has expiration time.
Widget notifications can be stacked but will be shown sequentially.

### Automatic Setup

It is assumed that a `Screen` class lives next to its UI counterpart, and the UI
counterpart is named `ScreenUI`.

**Example**:
```csharp
using ExtraTools.UI.Screen;

namespace Project.UI.Screens.MainMenuScreen
{
	public class MainMenuScreen : ScreenBase
	{
	}
}
```

```csharp
using ExtraTools.UI.Screen;

namespace Project.UI.Screens.MainMenuScreen
{
	public class MainMenuScreenUI : ScreenUIBase
	{
	}
}
```

If so, clicking on the `Set Screen` button on the `Screen` component will
automatically

1. Add the UI component
2. Set UI's `Canvas` field
3. Set the UI component to the `_screenUI` field

The same goes for all components which require a UI counterpart. Note that this
is not mandatory and will not throw an error if not set as described.