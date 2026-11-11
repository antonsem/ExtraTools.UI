# ExtraTools.UI

A UI system I find very useful. An example implementation can be found here:
https://github.com/antonsem/extraTools.UI.Example. And here is a blog explaining
my thought process: https://www.anton.website/ui-manager-a-better-one

## Summary

The primary purpose of this package is to separate the visuals of the UI from the functionality
and the data while providing easy access to every screen. This package does not provide
any implementations, only abstract base classes. The basic functionality, such as showing
or hiding screens, is provided by the base classes.

## Dependencies
This package depends on [UniTask 2.5.10](https://github.com/Cysharp/UniTask/tree/2.5.10#install-via-git-url)

## Documentation

### UI Manager

Provides access to screens, dialogs, and widgets. Internally handles the screen
switching.

Example:

```csharp

[SerializeField] private UIManager _uiManager;

private void Start()
{
    MainMenuScreen mainMenuScreen = _uiManager.GetScreen<MainMenuScreen>();
    mainMenuScreen.Show().Forget();
}
```

In case `MainMenuScreen` needs additional data a `Show()` method overrides with required
parameters can be introduced in the implementation.

`UIManager` also provides access to dialogs and widgets.

### Screen

Represents a single screen in the UI. A single screen can have multiple panels. Only
child classes can have access to panels.

Screen transitions run one at a time, so the last `Show()` or `Hide()` always decides what
is left on screen. Do not await another screen's `Show()` or `Hide()` from a `ShowAsync` or
`HideAsync` override, because it would wait for itself forever. Use `.Forget()` there instead.

### Screen UI

Responsible for relaying input to the screen and visual presentation of a screen

### Panel

Represents a part of a screen. Not mandatory, but useful if only a part of the screen needs
to change. An example can be a settings screen with multiple tabs for different settings.

Panel changes on a screen run one at a time. Do not await another panel change on the same
screen from a panel's `ShowAsync` or `HideAsync` override, because it would wait for itself
forever. Use `.Forget()` there instead.

### Dialog

Represents a pop-up dialog window. Useful to inform the user or to ask an immediate
question (e.g. "Are you sure you want to quit?"). Dialogs can stack up or be shown one at
a time.

Requires a text and an answer option to set up.

A dialog closes when one of its answers is clicked. To close it from code, call `Hide`.

### Widget

Represents a notification or a short message. Can be clicked on and has expiration time.
Widget notifications can be stacked but will be shown sequentially.

A widget goes away by itself after its show time. `Show` completes once the widget is
visible. To close it early, call `Hide`, which also drops that widget's queued entries and
completes once the widget has hidden.

### Controller support

Every view gets a `CanvasGroup`, added at runtime when missing. The manager uses it so that
only the top dialog, or the active screen with its active panels and the active widget, can
be navigated to. Buttons on covered views show their Disabled state.

Focus is handled for you:

- A dialog selects its default answer when it opens. Mark one with
  `new DialogAnswer("No", isDefault: true)`, otherwise the first answer is used.
  Closing the dialog returns focus to what was selected before.
- Assign `FirstSelected` on a screen or panel view to give it focus when it shows. It
  is only used while nothing usable is selected, so a panel opening next to a focused
  button does not take focus away from it.

The package reads no input. To support a back or cancel button, call
`UIManager.Cancel()` from your own input code, for example from the Cancel action of
`InputSystemUIInputModule`. It goes to the top dialog, which clicks its default answer,
or ignores Cancel when no answer is marked default. With no dialog open it goes to the
active screen. Screens ignore Cancel unless you override `OnCancel`:

```csharp
protected override bool OnCancel()
{
	UIManager.GetScreen<MainMenuScreen>().Show().Forget();
	return true;
}
```

`Cancel()` returns `false` when nothing used it, so you can do something else then,
like opening a pause menu.

For the best results with a mouse, untick "Deselect On Background Click" on the input
module, so clicking empty space does not leave nothing selected.

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
3. Set the UI component to the `ScreenUI` field

The same goes for all components which require a UI counterpart. Note that this
is not mandatory and will not throw an error if not set as described.

Setup keeps what is already assigned. A UI component set by hand, even one on another
GameObject, is not replaced, and a screen keeps its default panels as long as they are
still its children. Each button press can be undone in one step.