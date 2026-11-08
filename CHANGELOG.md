# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed
- Showing the already active screen no longer hides and re-shows it. `Show()` only resets its panels to the defaults

### Fixed
- A dialog re-shown with `hideOtherDialogs` while already open lost its buttons
- Stopping a widget had no effect, and a widget outliving its scene threw a `MissingReferenceException`
- Stopping a widget twice, or reading its `CancellationToken` after a stop, threw a `NullReferenceException`
- UI could be initialized twice, and looking up other UI from an `Initialize` override overflowed the stack
- Panels hidden along with their screen were still reported as active
- `Show<T>(additive: true)` closed the other panels when called on the active screen
- Widgets never expired at `Time.timeScale = 0`. `showTime` now always counts real seconds
- Showing an open dialog again kept its old callbacks and buttons and listed it twice. It now updates in place without replaying `ShowAsync`
- An exception or cancellation while showing a widget blocked the widget queue, and a stopped widget could stay on screen
- Exceptions from default panels at startup were only reported when the garbage collector ran
- A `CancellationToken` passed to a screen's `Show` or `Hide` never reached its show and hide animations
- Switching screens quickly with animated views could leave several screens visible
- Changing panels quickly with animated views could leave extra panels visible or missing from `ActivePanels`

## [2.0.2] - 2026-05-04

### Fixed
- A bug when adding a non-additive panel doesn't remove other panels

## [2.0.1] - 2026-05-04

### Removed
- The UniTask plugin. It needs to be added manually

## [2.0.0] - 2026-05-01

### Changed
- Replaced Tasks with UniTasks

## [1.4.0] - 2026-05-01

### Added
- Exposed active panels
- A method to check if a panel is active

## [1.3.0] - 2026-02-16

### Added
- A way to get current active elements
- Dependency for uGUI 2.0.0

### Changed
- Made the UIManagerBase.OnDestroy method virtual

## [1.2.5] - 2025-01-10

### Added

- CHANGELOG.md

### Fixed

- Clearing the widget queue on destroy