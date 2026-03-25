using System.Collections.Generic;
using UnityEngine;

namespace ImMentallyWell.Core
{
    /// <summary>
    /// Tracks every code that the player has discovered in the 2D mini-games
    /// and exposes them so that 3D puzzles can check whether they are solved.
    /// </summary>
    public class PuzzleManager : MonoBehaviour
    {
        public static PuzzleManager Instance { get; private set; }

        // Maps a puzzle ID (e.g. "safe_code") to the code string the player found.
        private readonly Dictionary<string, string> _discoveredCodes = new();

        // Tracks which 3D puzzles have been solved.
        private readonly HashSet<string> _solvedPuzzles = new();

        /// <summary>Raised when a new code is discovered.</summary>
        public event System.Action<string, string> OnCodeDiscovered;

        /// <summary>Raised when a 3D puzzle is successfully solved.</summary>
        public event System.Action<string> OnPuzzleSolved;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // ─── Code discovery (from 2D mini-games) ─────────────────────────────

        /// <summary>
        /// Register a code that the player found while playing a 2D mini-game.
        /// </summary>
        /// <param name="puzzleId">Unique identifier for the puzzle this code unlocks.</param>
        /// <param name="code">The code string (e.g. "7491").</param>
        public void DiscoverCode(string puzzleId, string code)
        {
            if (_discoveredCodes.ContainsKey(puzzleId))
                return; // already found

            _discoveredCodes[puzzleId] = code;
            Debug.Log($"[PuzzleManager] Code discovered for '{puzzleId}': {code}");
            OnCodeDiscovered?.Invoke(puzzleId, code);
        }

        /// <summary>Returns true if the player has found the code for this puzzle.</summary>
        public bool HasCode(string puzzleId) => _discoveredCodes.ContainsKey(puzzleId);

        /// <summary>
        /// Attempts to retrieve the discovered code for a puzzle.
        /// Returns true and sets <paramref name="code"/> if found.
        /// </summary>
        public bool TryGetCode(string puzzleId, out string code) =>
            _discoveredCodes.TryGetValue(puzzleId, out code);

        // ─── 3D puzzle solving ────────────────────────────────────────────────

        /// <summary>
        /// Try to solve a 3D puzzle by entering a code.
        /// Returns true if the code matches what was discovered in the 2D game.
        /// </summary>
        public bool TrySolvePuzzle(string puzzleId, string enteredCode)
        {
            if (_solvedPuzzles.Contains(puzzleId))
                return true; // already solved

            if (!_discoveredCodes.TryGetValue(puzzleId, out string correctCode))
            {
                Debug.Log($"[PuzzleManager] No code discovered yet for '{puzzleId}'.");
                return false;
            }

            if (enteredCode == correctCode)
            {
                _solvedPuzzles.Add(puzzleId);
                Debug.Log($"[PuzzleManager] Puzzle '{puzzleId}' solved!");
                OnPuzzleSolved?.Invoke(puzzleId);
                return true;
            }

            Debug.Log($"[PuzzleManager] Wrong code for '{puzzleId}'.");
            return false;
        }

        /// <summary>Returns true if the specified 3D puzzle has been solved.</summary>
        public bool IsPuzzleSolved(string puzzleId) => _solvedPuzzles.Contains(puzzleId);

        // ─── Reset ────────────────────────────────────────────────────────────

        /// <summary>Clear all discovered codes and solved puzzles (for a new game).</summary>
        public void ResetAllPuzzles()
        {
            _discoveredCodes.Clear();
            _solvedPuzzles.Clear();
            Debug.Log("[PuzzleManager] All puzzles reset.");
        }
    }
}
