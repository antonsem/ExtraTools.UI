# ExtraTools.UI

A UI system I find very useful. An example implementation can be found here:
https://github.com/antonsem/extraTools.UI.Example.

## Summary

The primary purpose of this package is to separate visuals of the UI from the functionality
and the data, while providing an easy access to every screen. This package does not provide
any implementations, only abstract base classes. The basic functionality, such as showing
or hiding screens, is provided by the base classes.

## Documentation

### UI Manager

Provides an access to screens, dialogs and widgets. Internally handles the screen
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

In case `MainMenuScreen` needs additional data a `Show()` method override with required
parameters can be introduced in the implementation.

`UIManager` also provides access for dialogs and widgets.

| Public Members                          | Description                                                |
|-----------------------------------------|------------------------------------------------------------|
| `T GetScreen<T>() where T : ScreenBase` | Returns a screen of type T if registered. `null` otherwise |
| `T GetDialog<T>() where T : DialogBase` | Returns a dialog of type T if registered. `null` otherwise |
| `T GetWidget<T>() where T : WidgetBase` | Returns a widget of type T if registered. `null` otherwise |

| Internal Members                                                 | Description                                                                                                                                                       |
|------------------------------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `async Task ShowScreen(ScreenBase screen)`                       | Hides current screen and shows the one passed as the argument. Awaits the current screen's `HideAsync()` method, and new screen's `ShowAsync()` method            |
| `async Task HideScreen(ScreenBase screen)`                       | Hides the screen. Awaits screen's `HideAsync()` method                                                                                                            |
| `async void ShowDialog(DialogBase dialog, bool hideRest = true)` | Shows the dialog. If `hideRest` is true discards and hides all active dialogs. Awaits active dialogs' `HideAsync()` methods and new dialog's `ShowAsync()` method |
| `async Task HideAllDialogs()`                                    | Hides all active dialogs. Awaits open dialogs' `HideAsync()` methods                                                                                              |
| `void ShowWidget(WidgetTask task)`                               | Enques new a widget task to be shown when the ones before are dequed                                                                                              |
| `void HideAllWidgets()`                                          | Clears widget queue                                                                                                                                               |

| Protected Members      | Description                    |
|------------------------|--------------------------------|
| `virtual void Awake()` | Calls the `Initilize()` method |

| Private Members                            | Description                                                                          |
|--------------------------------------------|--------------------------------------------------------------------------------------|
| `void Initialize()`                        | Searches for and registers and initializes screens, dialogs, and widgets in children |
| `void SetUI()`                             | Editor-only helper method. Invokes similar methods on its children                   |
| `Dictionary<Type, ScreenBase> _screens`    | Registered screens                                                                   |
| `Dictionary<Type, DialogBase> _dialogs;`   | Registered dialogs                                                                   |
| `Dictionary<Type, WidgetBase> _widgets;`   | Registered widgets                                                                   |
| `ScreenBase _activeScreen`                 | Currently active screen                                                              |
| `readonly List<DialogBase> _activeDialogs` | Currently active dialogs                                                             |
| `readonly Queue<WidgetTask> _widgetQueue`  | Widget queue to be displayed in order                                                |
| `WidgetTask _activeWidget`                 | Currently active widget                                                              |

### Screen

Represents a single screen in the UI. A single screen can have multiple panels. Only
child classes can have access to panels.

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