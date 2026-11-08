# CLAUDE.md

## Project overview
Unity UI package (`com.extratools.ui`, display name ExtraTools.UI), distributed through the Package Manager. Current version is in `package.json`.
Only abstract base classes, no concrete implementations. The point of the package is to separate UI visuals from functionality and data, and to give easy access to every screen, dialog and widget through one manager.
The consuming project subclasses everything. An example implementation lives in a separate repo (`antonsem/extraTools.UI.Example`).
Dependencies: `com.unity.ugui` (declared in `package.json`, also provides TextMeshPro) and UniTask (referenced by GUID in `ExtraTools.UI.asmdef` but not declared in `package.json`, consumers install it manually, see `README.md`).
Must stay self-contained: don't reference any game project code.

## Package structure
This repo is the package root. Unity treats it as `Packages/com.extratools.ui/`. There are no scenes, prefabs or test projects here.
- `.git/`, `.idea/`: irrelevant folders, ignore these
- `*.meta`: Unity generated. Never hand-edit, create or delete them (see Things to avoid)
- `Base/`: `UIManagerBase`. Finds all screens, dialogs and widgets under it, initializes them, hands them out via `GetScreen<T>` / `GetDialog<T>` / `GetWidget<T>`. Owns screen switching, the dialog stack and the widget queue
- `Screen/`: `ScreenBase` (presenter, owns panels) and `ScreenUIBase` (view, owns the `Canvas`)
- `Panel/`: `PanelBase` and `PanelUIBase`. Optional part of a screen, e.g. a settings tab
- `Dialog/`: `DialogBase`, `DialogUIBase` and the `DialogAnswer` struct. Pop-ups, they can stack or be shown one at a time
- `Widget/`: `WidgetBase`, `WidgetUIBase` and `WidgetTask`. Timed notifications, shown sequentially from a queue
- `Editor/`: custom inspectors with the "Set Screen" style setup buttons, plus `Utils`. Own asmdef (`ExtraTools.UI.Editor`, Editor platform only)
- `README.md`: user-facing docs. `CHANGELOG.md`: Keep a Changelog format

Root namespace `ExtraTools.UI`, one sub-namespace per folder (`ExtraTools.UI.Screen`, `.Panel`, `.Dialog`, `.Widget`, `.Base`, and `.Editor` for editor code). Two asmdefs: runtime and editor. The runtime asmdef must never reference the editor one.

## Architecture rules
- Every feature is a presenter (`XBase`) plus a view (`XUIBase`). The presenter holds logic and state, the view holds the `Canvas` and visuals. Input goes view → presenter, never the other way around through scene lookups.
- `UIManagerBase` is the only entry point. Screens, dialogs and widgets are registered by their exact runtime `Type`, so look-ups with a base class or interface will fail.
- Showing and hiding is UniTask based (`ShowAsync` / `HideAsync`, and `Show` / `Hide` on `ScreenUIBase`) so consumers can override with animations. The default view implementations essentially just toggle `Canvas.enabled`. Keep new lifecycle methods UniTask based in the same way, not `Task`.
- Lifecycle hooks are `protected internal virtual`. The exceptions are the panel methods on `ScreenBase` (`ShowPanelAsync`, `HidePanelAsync`, `HidePanelsAsync`) and `DialogBase.OnClicked`, which are `protected virtual`, and `DialogBase.Initialize`, which is `internal` and can't be overridden. Public API (`Show`, `Hide`, `GetX<T>`) is what consuming projects call. Keep the surface small. Anything a consumer should not call stays `internal`.
- Consumers add `Show(...)` overloads with their own parameters when a screen needs data. Don't add data-passing to the base classes.
- `README.md` states: only child classes can access panels. Panel references (`_panels`, `GetPanel<T>`, `ActivePanels`) stay non-public. Outside callers can only pick which panel is shown or hidden through `ScreenBase.Show<T>` / `Hide<T>`. Don't expose more than that.

## Automatic setup (editor)
The inspector setup buttons (e.g. "Set Screen" on `ScreenBase`) rely on conventions. Breaking them silently breaks the workflow for every consumer.
- A presenter class and its view live next to each other, and the view is named `<PresenterName>UI` in the same namespace and assembly (`MainMenuScreen` → `MainMenuScreenUI`). `Utils.TryAddType` finds it by that name.
- The editor assigns serialized fields **by name through reflection** (`Utils.AssignFiled`): `_canvas`, `_screenUI`, `_panels`, `_defaultPanels`, `_panelUI`, `_dialogUI`, `_widgetUI` and others. Renaming any of these fields in the runtime code requires updating the matching editor script in the same change.
- Setup must not throw when the convention isn't followed. Missing UI counterpart means "do nothing", as the README promises.

## Coding conventions
- Serialized fields are `[SerializeField] private` by default. `protected` is used where subclasses in consuming projects need the reference (`_screenUI`, `_panels`, `_canvas`, ...). Do not make them public.
- No `GameObject.Find` or `FindObjectOfType`. The manager discovers its children with `GetComponentsInChildren` during initialization.
- Cache component references in `Awake` / `Initialize`, not `Update`.
- Keep `MonoBehaviour` classes focused. Move pure logic into plain C# classes (like `WidgetTask`) so it can be unit tested.
- This is a public package: think about consumers. Changing a `public`, `protected` or `protected internal` signature is a breaking change.
- Comments explain why, not what.
- No `.editorconfig` in this repo. Match the formatting and naming of the existing files.
- Rules that are easy to miss:
  - No `var`. Always use explicit types; use target-typed `new()` when the type is already on the left (`List<string> names = new();`).
  - Private fields are `_camelCase`. Existing constants in `UIManagerBase` are `PascalCase` inside a `#region Constants`. Keep that style for constants in this package.
  - Indent with tabs.
  - Namespace blocks use braces (not file-scoped).

## Versioning and changelog
- Semantic Versioning. Version lives in `package.json`.
- Every user-visible change gets an entry in `CHANGELOG.md` under Added / Changed / Fixed / Removed, newest first, dated.
- Bump the version in `package.json` together with the changelog, but only when asked to prepare a release.
- Update `README.md` when public behaviour or setup conventions change.

## Things to avoid
- Don't touch `.meta` files. If a file is added, renamed or moved, tell me and I'll let Unity generate the metadata.
- Don't modify `package.json` (version, dependencies, name) without asking first.
- Don't add new package dependencies or asmdef references without asking.
- Don't rename or remove public/protected API without asking. It breaks consumers.
- Don't add concrete implementations, prefabs or sample content to the package. Samples belong in the example repo.
- Don't rewrite working systems when asked for a small change. Make the smallest change that works.
- Don't try to push or commit changes to git repository.

## Working style
- When brainstorming a new component or API, give two or three options with trade-offs before writing code.
- For implementation, start with the smallest testable slice.
- Before large edits, state the plan and which files will change.
- If something depends on scene setup, prefab hierarchy, `Canvas` configuration or editor-only configuration, say so explicitly rather than assuming.
- This package has no test project. If a change can't be verified here, say how I can verify it in a consuming project.

## Debugging
- When investigating a bug, read the related scripts first and state the likely cause before proposing a fix.
- Ask for the console or Player log if a runtime error is involved.
- Most issues come from setup, not logic: a missing or duplicated component type under the manager (duplicate types throw on registration), unassigned `_canvas` / `_screenUI` fields, or a view not named `<Presenter>UI`. Check those first.
