# Hephaestus Audio

Hephaestus Audio is a part of the Hephaestus Core framework. It provides a small,
Zenject-based audio system for Unity that plays music and sound clips by typed
`enum` keys, routes them through an `AudioMixer`, and pools its audio sources.

## Features

- Play music and sounds by strongly-typed `enum` keys (generated from a config).
- Exclusive music channel (a new track stops the previous one) and a polyphonic
  sounds channel.
- Per-channel pooling of `AudioSource` handlers — no GameObject churn at runtime.
- Volume control through exposed `AudioMixer` parameters.
- Configurable mixer group names and volume parameter names.

## Installation

The package is distributed via UPM. Add it to your `manifest.json` (it depends on
Extenject/Zenject):

```json
{
  "dependencies": {
    "com.wtfgames.hephaestus.audio": "2.1.0",
    "com.svermeulen.extenject": "9.2.0-stcf3"
  }
}
```

## Setup

### 1. Create the assets

Via the `Create > HephaestusMobile/Core/Audio` menu, create:

- **AudioLibraryConstants** — the list of audio keys.
- **AudioLibrary** — maps each key to an `AudioClip` (assign the
  `AudioLibraryConstants` asset to it).
- **AudioManagerConfig** — references the `AudioLibrary` and an `AudioMixer`.
- **HephaestusSOInstaller** — references the `AudioManagerConfig`.

The `AudioMixer` is expected to expose a music group, a sounds group, and two
exposed volume parameters. The default names are `Music` / `Sounds` and
`MusicVolume` / `SoundsVolume`; all four are configurable on `AudioManagerConfig`.

### 2. Define audio keys and export the enum

Select the `AudioLibraryConstants` asset:

1. Add keys (e.g. `GAMEPLAY_MUSIC`, `GEM_COLLECTED`).
2. Pick an output folder and click **Export to enum** to generate a
   `AudioLibraryConstants` enum.
3. On the `AudioLibrary` asset, bind each key to its `AudioClip`.

### 3. Register the installers

Add `HephaestusSOInstaller` to your `SceneContext`/`ProjectContext`, and install
the manager bindings from your own installer:

```csharp
public override void InstallBindings()
{
    HephaestusAudioInstaller.Install(Container);
}
```

This binds `IAudioManager` as a singleton.

## Usage

```csharp
public class GameAudio
{
    private readonly IAudioManager _audio;

    public GameAudio(IAudioManager audio) => _audio = audio;

    public void Run()
    {
        // Looping background music (exclusive — replaces any current track).
        _audio.PlayMusicClip(AudioLibraryConstants.GAMEPLAY_MUSIC, loopSound: true, volume: 0.5f);

        // One-shot sound effect.
        var handler = _audio.PlaySoundClip(AudioLibraryConstants.GEM_COLLECTED);

        // Volume control (0..1, mapped to dB on the mixer).
        _audio.SetMusicVolume(0.8f);
        _audio.SetSoundsVolume(1f);

        // Queries.
        bool playing = _audio.IsMusicPlay(AudioLibraryConstants.GAMEPLAY_MUSIC);

        // Stop by key.
        _audio.StopPlayingSound(AudioLibraryConstants.GEM_COLLECTED);
    }
}
```

### Safely controlling a returned handle

`PlaySoundClip`/`PlayMusicClip` return the `AudioSourceHandler`. Because handlers
are pooled and reused, capture the `PlayId` token and use the token-checked
overloads so you never affect a clip that has since been replaced:

```csharp
var handler = _audio.PlaySoundClip(AudioLibraryConstants.GEM_COLLECTED);
int token = handler.PlayId;

// Later — only acts if this is still the same clip on that handler.
if (handler.IsValid(token))
{
    handler.Stop(token);
}
```

## Sample

Import the **Audio and Music** sample from the Package Manager for a ready-made
scene, configs, and a test helper.
