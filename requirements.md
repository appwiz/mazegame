# Maze Game - Requirements Specification

## Overview
A 2D maze navigation game where the player must reach a goal tile within a limited number of steps. The game features a smiley face character navigating through a fixed maze layout with walls, paths, and an end goal.

## Game Objective
Navigate from the starting position to the gold end tile in 20 steps or fewer.

## Technical Specifications

### Platform
- Framework: MonoGame (DesktopGL)
- Language: C# (.NET 9.0)
- Window Size: 560x560 pixels (14x14 tiles at 40 pixels per tile)

### Maze Layout
- Grid-based maze: 14 columns × 14 rows
- Tile size: 40×40 pixels
- Three tile types:
  - **Walls (1)**: Dark blue impassable barriers
  - **Paths (0)**: Light gray walkable areas
  - **End goal (2)**: Gold-colored target tile at position (12, 12)
- Starting position: Random path tile at least 6 tiles away from goal (Manhattan distance)

## Game Mechanics

### Starting Position
- Randomly selected at game start and on restart
- Must be a valid path tile (not wall or goal)
- Minimum distance from goal: 6 tiles (Manhattan distance)
- Ensures fair difficulty on each playthrough

### Player Movement
- Control method: Arrow keys (Up, Down, Left, Right)
- Movement: One tile per key press (grid-locked)
- Collision detection: Player cannot move through walls or out of bounds
- Step counting: Each valid move increments the step counter

### Win Condition
- Player reaches the end goal tile (gold tile)
- Must be achieved within 20 steps
- Success triggers victory state

### Failure Condition
- Player exceeds 20 steps without reaching the goal
- Triggers failure state

### Game States
1. **Playing**: Active gameplay with movement and step counting
2. **Victory**: Player reached goal within step limit
3. **Failure**: Player exceeded 20 steps without reaching goal

## User Interface

### Visual Elements

#### Maze Rendering
- **Walls**: Dark blue solid blocks
- **Paths**: Light gray tiles
- **End goal**: Gold-colored tile
- **Player**: Yellow smiley face with:
  - Circular yellow face
  - Two black eyes
  - Arc-shaped smile

#### HUD Elements
- **Trail Mode Indicator** (top-left):
  - Cyan dot displayed when trail mode is active
  
- **Step Counter** (top-right):
  - Visual representation using small dots
  - White dots: Within step limit
  - Red dots: Step limit exceeded
  - Maximum 20 dots displayed (2 rows of 10)

#### Victory Screen
- Pulsing gold circle in center (80±10 pixel radius)
- Green checkmark circle (50 pixel radius)
- Pulse animation at 3 Hz

#### Failure Screen
- Pulsing red circle in center (80±10 pixel radius)
- Dark red X circle (50 pixel radius)
- Pulse animation at 3 Hz

### Controls

| Key | Function |
|-----|----------|
| Arrow Keys | Move player (Up/Down/Left/Right) |
| T | Toggle trail mode on/off |
| R | Restart game (when won or failed) |
| Escape | Exit game |

## Features

### Trail Mode
- **Activation**: Press T key to toggle
- **Visual indicator**: Cyan dot in top-left corner when active
- **Effect**: Previously visited tiles display as faint cyan (40% opacity)
- **Purpose**: Help players track their path through the maze

### Game Reset
- Available in both victory and failure states
- Press R to restart
- Resets:
  - Player position to a new random valid location (at least 6 tiles from goal)
  - Step counter to 0
  - Visited tiles tracking
  - Game state flags
  - Trail mode remains at current setting

## Constraints & Rules

### Step Limit
- Maximum allowed steps: 20
- Steps are counted only for successful moves
- Attempting to move into a wall does not count as a step
- Step counter visible via HUD dots

### Movement Rules
- Player can only move one tile at a time
- Diagonal movement not allowed
- Cannot move through walls
- Cannot move outside maze boundaries
- Movement requires key release and re-press (no continuous movement)

## Visual Style
- Minimalist pixel art aesthetic
- Procedurally drawn graphics (no external sprite assets)
- Color palette:
  - Black background
  - Dark blue walls
  - Light gray paths
  - Gold goal tile
  - Yellow player character
  - Cyan trail indicators
  - White/red step counter dots
  - Gold/green victory elements
  - Red/dark red failure elements

## Technical Implementation Notes

### Rendering
- Uses SpriteBatch with 1×1 white pixel texture for all graphics
- Circles drawn procedurally using pixel-by-pixel rendering
- Arc drawn using mathematical curve calculation

### State Management
- Boolean flags for game states (won/failed)
- Integer step counter
- HashSet for tracking visited tiles
- Keyboard state tracking for single-press detection
- Random number generator for start position selection
- Goal position stored as constant Vector2

### Performance
- Fixed 14×14 maze grid
- Minimal real-time calculations
- Efficient collision detection using array bounds checking
- Start position calculated once per game/restart using Manhattan distance formula
