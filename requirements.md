# Maze Game - Requirements Specification

## Overview
A 2D maze navigation game where the player must reach a goal tile within a limited number of steps. The game features a smiley face character navigating through a fixed maze layout with walls, paths, and an end goal.

## Game Objective
Navigate from the starting position to the gold end tile within the step limit:
- Small maze (8x8): 12 steps or fewer
- Medium maze (16x16): 24 steps or fewer
- Large maze (24x24): 48 steps or fewer

## Technical Specifications

### Platform
- Framework: MonoGame (DesktopGL)
- Language: C# (.NET 9.0)
- Window Size: Dynamic based on selected maze size
  - Small: 320x320 pixels
  - Medium: 640x640 pixels (default)
  - Large: 960x960 pixels

### Maze Layout
- **Three available sizes**:
  - **Small (S)**: 8 columns × 8 rows (320x320 pixels)
  - **Medium (M)**: 16 columns × 16 rows (640x640 pixels) - **Default**
  - **Large (L)**: 24 columns × 24 rows (960x960 pixels)
- Tile size: 40×40 pixels
- **Randomly generated** using recursive backtracking algorithm
- Generation ensures connectivity between all path tiles
- Three tile types:
  - **Walls (1)**: Dark blue impassable barriers
  - **Paths (0)**: Light gray walkable areas
  - **End goal (2)**: Gold-colored target tile in bottom-right quadrant
- Starting position: Random path tile at least 6 tiles away from goal (Manhattan distance)
- New maze generated on each game start, restart, and size change
- Selected size is remembered across restarts (R key)

## Game Mechanics

### Maze Size Selection
- Three preset sizes: Small (8x8), Medium (16x16), Large (24x24)
- Default size on startup: Medium (16x16)
- Change size anytime by pressing S, M, or L keys
- Changing size generates a new maze and resets game state
- Window dynamically resizes to fit selected maze
- Current size is preserved when restarting with R key

### Maze Generation
- Uses recursive backtracking algorithm for procedural generation
- Starts with a grid filled with walls
- Carves connected paths through the maze
- Guarantees at least one path exists from any tile to any other tile
- Goal placed at fixed position (12, 12) in bottom-right area
- Fresh maze generated on startup and every restart

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
- Must be achieved within the step limit for the current maze size
- Success triggers victory state

### Failure Condition
- Player exceeds the step limit without reaching the goal
- Step limits vary by maze size:
  - **Small (8x8)**: 12 steps
  - **Medium (16x16)**: 24 steps
  - **Large (24x24)**: 48 steps
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
  - Maximum dots displayed matches current maze size limit (12, 24, or 48)

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
| S | Switch to Small maze (8x8) |
| M | Switch to Medium maze (16x16) |
| L | Switch to Large maze (24x24) |
| R | Restart game (preserves current size) |
| Q / Escape | Exit game |

## Features

### Trail Mode
- **Activation**: Press T key to toggle
- **Visual indicator**: Cyan dot in top-left corner when active
- **Effect**: Previously visited tiles display as faint cyan (40% opacity)
- **Purpose**: Help players track their path through the maze

### Game Reset
- Available at any time by pressing R
- Resets:
  - **Generates entirely new maze** with different layout
  - **Preserves current maze size** (S/M/L selection)
  - Player position to a new random valid location (at least 6 tiles from goal)
  - Step counter to 0
  - Visited tiles tracking
  - Game state flags
  - Trail mode remains at current setting

## Constraints & Rules

### Step Limit
- Maximum allowed steps varies by maze size:
  - Small (8x8): 12 steps
  - Medium (16x16): 24 steps
  - Large (24x24): 48 steps
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

### Maze Generation Algorithm
- **Recursive backtracking**: Depth-first search approach
- Initialization: Fill grid with walls (size depends on selected maze size)
- Starting point: (1, 1)
- Process:
  1. Mark current cell as path
  2. Randomly shuffle adjacent directions (up, down, left, right)
  3. For each direction, check if two cells away is unvisited
  4. If valid, carve path between cells and recursively continue
- Result: Guaranteed connected maze with single solution paths
- Goal placed dynamically in bottom-right quadrant after generation complete

### State Management
- Boolean flags for game states (won/failed)
- Integer step counter
- HashSet for tracking visited tiles
- Keyboard state tracking for single-press detection
- Random number generator for start position selection
- Goal position stored as constant Vector2

### Performance
- Grid size: 8x8, 16x16, or 24x24 (user selectable)
- Maze generation occurs once per game/restart/size change (not per frame)
- Window resizing handled through graphics device manager
- Minimal real-time calculations during gameplay
- Efficient collision detection using array bounds checking
- Start position calculated once per game/restart using Manhattan distance formula
