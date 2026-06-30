# Changelog

All notable changes to this project will be documented in this file. The format is based on [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/) and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.1.0] - 2026-06-23

### fix
- Stopped coroutines by their stored handle instead of a fresh iterator that never matched the running coroutine.
- Restarted the clip-end coroutine on every `Play` so `OnClipEnded` keeps firing on reused handlers.
- Used `PlayDelayed` for second-based delays (`AudioSource.Play(ulong)` expects samples).
- Emitted `Stop` events with the real clip key before resetting it.
- Destroyed the `DontDestroyOnLoad` `AudioManagerHandler` on `Dispose` to fix a leak.
- Made music exclusive: playing a new track stops the previous one.
- Guarded `AudioLibrary.GetAudioClipByKey` against a missing key (was a `NullReferenceException`).
- `IsMusicPlay`/`IsSoundPlay` now check the handler is actually playing.

### perf
- Pooled `AudioSourceHandler` instances per channel instead of destroying and recreating GameObjects on every `Play`.

### refactor
- Created `AudioManagerHandler` through a Zenject `PlaceholderFactory` and injected its `AudioManagerConfig` via `[Inject]` instead of `new GameObject().AddComponent` and manual wiring.

### feat
- Added a `PlayId` token plus `Stop(int)`/`IsValid(int)` so callers can safely operate on a returned handle after it has been pooled and reused.
- Made mixer group names and exposed volume parameters configurable via `AudioManagerConfig`.
- Skipped adding an `AudioListener` when the scene already has one.

## [2.0.3] - 2023-08-31

### chore
- Updated dependencies to improve project stability and performance.
- Updated the publishing section to reflect new distribution channels.

## [2.0.0] - 2022-02-18

### feat
- Introduced constants for audio entities to standardize audio handling.
- Added a custom editor for managing constants more efficiently.
- Implemented functionality to export audio constants for external use.

### refactor
- Changed the type for audio keys in `IAudioManager` and `AudioManager` from `string` to `Enum` to enforce type safety.

### BREAKING CHANGE
- Removed the ability to use string keys for playing and stopping audio clips, enforcing the use of defined constants for better code clarity and maintainability.

## [1.0.0] - 2022-06-04

### feat
- Implemented basic initialization setup for the audio management system.
- Created `AudioManagerHandler` entity as `MonoBehaviour` to facilitate audio management in Unity.
- Modified `AudioManager` to no longer inherit from `MonoBehaviour`, streamlining its usage in non-Unity contexts.

## [0.0.1] - 2021-02-18

### feat
- Established the foundation of HephaestusCore Audio as a UnityPackage, providing basic audio management functionalities.

