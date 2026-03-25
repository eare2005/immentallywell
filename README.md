# I'm Mentally Well

A Unity escape-room game that switches between two distinct play styles:

* **3D Exploration** – You wake up trapped inside your own home.  Wander the rooms in first-person, pick up notes, examine objects, and find the way out.
* **2D Computer Games** – Sitting down at the PC in the corner switches the view to a pixel-perfect 2D screen.  Play the mini-games installed on it to uncover secret codes hidden inside their game logic.

The codes you discover in the 2D games are the only way to unlock the combination locks, safes, and doors that stand between you and freedom.

---

## How It Works

```
3D Room  ──(sit at computer)──►  2D Desktop  ──(play mini-game)──►  win = code revealed
   ▲                                                                         │
   └─────────────(exit computer)──────────────────────────────────◄─────────┘
                                                                 (code stored in PuzzleManager)
                                                                         │
                                                         enter code at 3D keypad / safe
                                                                         │
                                                                  puzzle solved  ►  door unlocks
```

---

## Project Structure

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── GameManager.cs        – Game-mode state machine (3D ↔ 2D), scene loading
│   │   └── PuzzleManager.cs      – Stores discovered codes; validates entries at 3D puzzles
│   │
│   ├── ThreeD/
│   │   ├── PlayerController.cs   – First-person movement & E-key interaction
│   │   ├── InteractableObject.cs – Abstract base for all interactive room objects
│   │   ├── ComputerTerminal.cs   – The in-world PC; pressing E launches the 2D layer
│   │   ├── EscapeRoomPuzzle.cs   – Keypad / safe that accepts a code from PuzzleManager
│   │   └── Door.cs               – Lockable door; auto-unlocks when its puzzle is solved
│   │
│   ├── TwoD/
│   │   ├── MiniGameBase.cs       – Abstract base for 2D mini-games; handles start/stop/complete
│   │   ├── MiniGameController.cs – Scene controller for the 2D layer (desktop + game launcher)
│   │   ├── CodeGeneratorGame.cs  – Number-sequence memory game → reveals a 4-digit code
│   │   └── PongGame.cs           – Pong mini-game → reveals a code when the player wins
│   │
│   └── UI/
│       ├── TerminalUI.cs         – In-game desktop icons, notebook of discovered codes
│       ├── KeypadUI.cs           – On-screen number pad for entering codes at 3D puzzles
│       └── HUD.cs                – 3D-world HUD: interaction prompt, Tab-key notebook
│
├── Scenes/
│   ├── MainRoom.unity            – Primary 3D escape room scene  (create in Unity Editor)
│   └── ComputerScreen.unity      – Additive 2D computer layer     (create in Unity Editor)
│
└── Prefabs/                      – (add prefabs here once built in the Unity Editor)
```

---

## Getting Started

1. Open the project in **Unity 2022.3 LTS** or later.
2. Create two scenes: `MainRoom` and `ComputerScreen` (names must match the fields on `GameManager`).
3. In `MainRoom`, add:
   - A `GameManager` GameObject (with `GameManager` + `PuzzleManager` components).
   - A player capsule with `PlayerController` + `CharacterController`.
   - A desk with a `ComputerTerminal` component.
   - Locks / doors with `EscapeRoomPuzzle` and `Door` components.
   - A `KeypadUI` canvas.
4. In `ComputerScreen`, add:
   - A `MiniGameController` GameObject.
   - `CodeGeneratorGame` and/or `PongGame` child GameObjects (inactive by default).
   - A `TerminalUI` canvas.
5. Wire up the serialised fields in the Inspector and hit **Play**.

---

## Adding a New Mini-Game

1. Create a new C# class that extends `MiniGameBase`.
2. Override `OnGameStart()`, `OnGameStop()`, and call `CompleteGame(code)` when the player wins.
3. Set the `gameId`, `linkedPuzzleId`, and `displayName` fields in the Inspector.
4. Add the GameObject as a child of `MiniGameController` – `TerminalUI` will automatically create a desktop icon for it.

---

## Adding a New 3D Puzzle

1. Add an `EscapeRoomPuzzle` component to your prop and set its `puzzleId` to match the `linkedPuzzleId` of the mini-game that reveals its code.
2. Hook the `onSolved` UnityEvent to whatever should happen when the puzzle is solved (play animation, call `Door.Unlock()`, etc.).
