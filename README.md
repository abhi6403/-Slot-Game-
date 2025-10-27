# \# Slot Machine Game

# 

# \## Overview

# 

# This Unity project implements a modular slot machine game with customizable spinning mechanics, symbol data management, win evaluation, and bonus logic (free spins + RTP tracking).

# It uses event-based communication between the SlotMachine, SlotGameManager, and MachineController systems.

# 

# ---

# 

# \## Setup Instructions

# 

# 1\. Create a new Unity scene and add the following GameObjects:

# 

# &nbsp;  \* MachineController (root)

# &nbsp;  \* SlotMachine (child or referenced)

# &nbsp;  \* SlotGameManager (for UI and logic)

# 

# 2\. Assign references in the Inspector:

# 

# &nbsp;  \* MachineController:

# 

# &nbsp;    \* Assign SlotMachine reference.

# &nbsp;    \* Assign SymbolsMap ScriptableObject.

# &nbsp;    \* Set grid dimension (e.g., 3x5).

# &nbsp;    \* SlotMachine:

# 

# &nbsp;    \* Assign MachineConfig (Create via: Right-click → Create → SlotMachine → new config).

# &nbsp;    \* Assign SpinButton, Slot prefab, ViewTweener, and ViewFades.

# &nbsp;    \* SlotGameManager:

# 

# &nbsp;    \* Assign SlotMachine reference.

# &nbsp;    \* Assign TMP\_Text fields for Credits, Bet, and Win.

# 

# 3\. Symbol Setup:

# 

# &nbsp;  \* Create a SymbolsMap asset:

# &nbsp;    Right-click → Create → SlotMachine → Symbols → new symbols map

# &nbsp;  \* Add SymbolData entries with:

# 

# &nbsp;    \* type

# &nbsp;    \* name

# &nbsp;    \* sprite

# &nbsp;    \* value

# 

# 4\. UI Setup:

# 

# &nbsp;  \* Add TextMeshPro UI elements for Credits, Bet, and Win.

# &nbsp;  \* Add a Spin button and connect it to SpinButton script.

# 

# ---

# 

# \## Code Flow

# 

# 1\. MachineController (Entry Point)

# 

# &nbsp;  \* Initializes the SlotMachine and loads all symbols.

# &nbsp;  \* Generates the slot grid after loading.

# &nbsp;  \* Updates the machine every frame using Tick().

# 

# 2\. SlotMachine

# 

# &nbsp;  \* Handles slot generation and spin sequence.

# &nbsp;  \* Manages columns, spin delays, and stop logic.

# &nbsp;  \* Triggers OnSpinCompleted after all columns stop.

# 

# 3\. Column

# 

# &nbsp;  \* Controls vertical spinning of slots.

# &nbsp;  \* Uses tweening for smooth stop transitions.

# &nbsp;  \* Handles slot symbol swapping during spins.

# 

# 4\. Slot

# 

# &nbsp;  \* Represents a single slot cell.

# &nbsp;  \* Stores CurrentSymbol and updates its sprite.

# &nbsp;  \* Randomizes symbols when spinning.

# 

# 5\. SlotScoreEvaluator

# 

# &nbsp;  \* Calculates the total win after a spin.

# &nbsp;  \* Checks each row for consecutive matching symbols.

# &nbsp;  \* Supports wild symbols and streak multipliers.

# 

# &nbsp;  Streak-based scoring:

# 

# &nbsp;  \* 2 in a row → base value

# &nbsp;  \* 3 or 4 in a row → 2x multiplier

# &nbsp;  \* 5 or more → 3x multiplier

# 

# 6\. SlotGameManager

# 

# &nbsp;  \* Tracks credits, bet amount, wins, and free spins.

# &nbsp;  \* Subscribes to OnSpinCompleted from SlotMachine.

# &nbsp;  \* Updates RTP and logs spin data as JSON.

# 

# ---

# 

# \## Bonus Logic

# 

# Free Spins:

# 

# \* If 3 or more SCATTER symbols appear in a spin, player gets +3 free spins.

# \* During free spins, no credits are deducted.

# \* Free spin count decreases per spin until it reaches 0.

# 

# RTP (Return To Player):

# 

# \* RTP = (Total Payout / Total Bets) × 100

# \* Clamped between 95% and 96% for consistency.

# \* Logged after every spin for analytics.

# 

# Example JSON log:

# {

# "eventType": "spin\_result",

# "win": 200,

# "free\_spins\_remaining": 2,

# "rtp": 95.87,

# "symbols": \["FIRE", "WATER", "SCATTER", "EARTH", "WILD"]

# }

# 

# ---

# 

# \## Configuration

# 

# MachineConfig settings:

# 

# \* speed → Spin speed

# \* spinningDurationRange → Random spin duration range

# \* individuallyDelay → Delay between column spins

# \* autoSpinDelay → Delay before next auto-spin

# \* endSpinEase → Easing curve for stop animation

# \* endSpinYOffset → Stop offset for braking effect

# \* duration → Stop tween duration

# 

# ---

# 

# \## Design Notes

# 

# \* BaseMonoBehaviour ensures all custom behaviours release references properly.

# \* Uses Inspector-based dependency injection and event-driven logic.

# \* Extendable with:

# 

# &nbsp; \* New symbols (add to SymbolType enum and SymbolsMap)

# &nbsp; \* New win patterns (edit SlotScoreEvaluator)

# &nbsp; \* New animations or VFX with ViewTweener/ViewFades

# 

# ---

# 

# \## Summary

# 

# Gameplay loop:

# 

# 1\. Player presses Spin → columns start spinning.

# 2\. Columns stop sequentially.

# 3\. SlotScoreEvaluator calculates total win.

# 4\. Free spin or RTP logic is applied.

# 5\. SlotGameManager updates credits and UI.

# 

# ---



