using System.Collections;
using NUnit.Framework;
using Pully.Game;
using UnityEngine;
using UnityEngine.TestTools;

namespace Pully.Tests
{
    /// <summary>
    /// Tests for GameStateManager state transitions and timer behavior.
    /// </summary>
    public class GameStateManagerTests
    {
        private GameObject _stateGO;
        private GameStateManager _stateManager;
        private bool _stateChangedFired;
        private GameState _newState;
        private GameState _oldState;
        private bool _roundEndedFired;

        [SetUp]
        public void Setup()
        {
            _stateGO = new GameObject("StateManager");
            _stateManager = _stateGO.AddComponent<GameStateManager>();
            _stateChangedFired = false;
            _roundEndedFired = false;

            _stateManager.OnStateChanged += (newState, oldState) =>
            {
                _stateChangedFired = true;
                _newState = newState;
                _oldState = oldState;
            };

            _stateManager.OnRoundEnded += () =>
            {
                _roundEndedFired = true;
            };
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(_stateGO);
        }

        [Test]
        public void InitialState_IsInit()
        {
            Assert.AreEqual(GameState.INIT, _stateManager.CurrentState);
        }

        [Test]
        public void SetState_ToSameState_DoesNotFireEvent()
        {
            _stateManager.SetState(GameState.INIT);
            Assert.IsFalse(_stateChangedFired);
        }

        [Test]
        public void SetState_ToDifferentState_FiresEvent()
        {
            _stateManager.SetState(GameState.MAIN_MENU);
            Assert.IsTrue(_stateChangedFired);
            Assert.AreEqual(GameState.MAIN_MENU, _newState);
            Assert.AreEqual(GameState.INIT, _oldState);
        }

        [Test]
        public void SetState_UpdatesCurrentState()
        {
            _stateManager.SetState(GameState.GAMEPLAY);
            Assert.AreEqual(GameState.GAMEPLAY, _stateManager.CurrentState);
        }

        [Test]
        public void StartRound_SetsGameplayAndTimer()
        {
            _stateManager.StartRound(60f);
            Assert.AreEqual(GameState.GAMEPLAY, _stateManager.CurrentState);
            Assert.AreEqual(60f, _stateManager.RemainingSeconds);
        }

        [Test]
        public void StartRound_ClampsMinimumDuration()
        {
            _stateManager.StartRound(0f);
            Assert.AreEqual(1f, _stateManager.RemainingSeconds);

            _stateManager.StartRound(-5f);
            Assert.AreEqual(1f, _stateManager.RemainingSeconds);
        }

        [UnityTest]
        public IEnumerator GameplayTimer_CountsDown()
        {
            _stateManager.StartRound(5f);

            float initialTime = _stateManager.RemainingSeconds;
            yield return new WaitForSeconds(0.5f);

            Assert.Less(_stateManager.RemainingSeconds, initialTime);
        }

        [UnityTest]
        public IEnumerator GameplayTimer_ReachesZero_TriggersGameOver()
        {
            _stateManager.StartRound(0.1f);
            yield return new WaitForSeconds(0.15f);

            Assert.AreEqual(0f, _stateManager.RemainingSeconds, 0.01f);
            Assert.AreEqual(GameState.GAME_OVER, _stateManager.CurrentState);
        }

        [UnityTest]
        public IEnumerator GameplayTimer_ReachesZero_FiresRoundEnded()
        {
            _stateManager.StartRound(0.1f);
            yield return new WaitForSeconds(0.15f);

            Assert.IsTrue(_roundEndedFired);
        }

        [UnityTest]
        public IEnumerator GameplayTimer_DoesNotCount_InOtherStates()
        {
            _stateManager.StartRound(5f);
            yield return null;

            _stateManager.SetState(GameState.PAUSE);
            float timeAtPause = _stateManager.RemainingSeconds;

            yield return new WaitForSeconds(0.5f);

            // Timer should not have decreased in PAUSE state
            Assert.AreEqual(timeAtPause, _stateManager.RemainingSeconds);
        }

        [UnityTest]
        public IEnumerator StateTransition_INIT_ToMainMenu()
        {
            _stateManager.SetState(GameState.MAIN_MENU);
            yield return null;

            Assert.AreEqual(GameState.MAIN_MENU, _stateManager.CurrentState);
            Assert.IsTrue(_stateChangedFired);
        }

        [UnityTest]
        public IEnumerator StateTransition_MainMenu_ToGameplay()
        {
            _stateManager.SetState(GameState.MAIN_MENU);
            _stateManager.StartRound(60f);
            yield return null;

            Assert.AreEqual(GameState.GAMEPLAY, _stateManager.CurrentState);
        }

        [UnityTest]
        public IEnumerator StateTransition_Gameplay_ToPause()
        {
            _stateManager.StartRound(60f);
            _stateChangedFired = false;
            _newState = GameState.INIT;
            _oldState = GameState.INIT;

            _stateManager.SetState(GameState.PAUSE);
            yield return null;

            Assert.AreEqual(GameState.PAUSE, _stateManager.CurrentState);
            Assert.IsTrue(_stateChangedFired);
            Assert.AreEqual(GameState.PAUSE, _newState);
            Assert.AreEqual(GameState.GAMEPLAY, _oldState);
        }

        [UnityTest]
        public IEnumerator StateTransition_Pause_ToGameplay()
        {
            _stateManager.StartRound(60f);
            _stateManager.SetState(GameState.PAUSE);
            _stateChangedFired = false;

            _stateManager.SetState(GameState.GAMEPLAY);
            yield return null;

            Assert.AreEqual(GameState.GAMEPLAY, _stateManager.CurrentState);
            Assert.IsTrue(_stateChangedFired);
        }

        [UnityTest]
        public IEnumerator StateTransition_Gameplay_ToGameOver()
        {
            _stateManager.StartRound(60f);
            _stateManager.SetState(GameState.GAME_OVER);
            yield return null;

            Assert.AreEqual(GameState.GAME_OVER, _stateManager.CurrentState);
        }

        [UnityTest]
        public IEnumerator MultipleTransitions_EventsFireCorrectly()
        {
            int eventCount = 0;
            _stateManager.OnStateChanged += (newState, oldState) => eventCount++;

            _stateManager.SetState(GameState.MAIN_MENU);
            _stateManager.SetState(GameState.GAMEPLAY);
            _stateManager.SetState(GameState.PAUSE);
            _stateManager.SetState(GameState.GAME_OVER);
            yield return null;

            Assert.AreEqual(4, eventCount);
        }

        [UnityTest]
        public IEnumerator RemainingSeconds_StopsAtZero()
        {
            _stateManager.StartRound(0.5f);
            yield return new WaitForSeconds(1f);

            Assert.AreEqual(0f, _stateManager.RemainingSeconds, 0.01f);
            Assert.IsFalse(_stateManager.RemainingSeconds < 0f, "RemainingSeconds should not be negative");
        }
    }
}
