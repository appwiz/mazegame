using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace monogamekit;

public enum MazeSize
{
    Small = 8,
    Medium = 16,
    Large = 24
}

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _pixelTexture;
    
    // Maze: 0 = path, 1 = wall, 2 = end
    private int[,] _maze;
    
    private MazeSize _currentMazeSize = MazeSize.Medium; // Default to Medium
    private int _mazeWidth;
    private int _mazeHeight;
    private const int TileSize = 40;
    private int _maxSteps;
    private Vector2 _goalPosition;
    private readonly System.Random _random = new();
    private Vector2 _playerPosition; // Start position (set randomly)
    private KeyboardState _previousKeyboardState;
    private bool _gameWon = false;
    private bool _gameFailed = false;
    private int _stepCount = 0;
    private bool _trailModeEnabled = false;
    private readonly HashSet<(int, int)> _visitedTiles = new();

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
        // Initialize maze dimensions based on default size
        UpdateMazeSize();
    }

    protected override void Initialize()
    {
        GenerateMaze();
        _playerPosition = GetRandomStartPosition();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Create a 1x1 white pixel texture for drawing
        _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
        _pixelTexture.SetData(new[] { Color.White });
        
        // Add starting position to visited tiles
        _visitedTiles.Add(((int)_playerPosition.X, (int)_playerPosition.Y));
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboardState = Keyboard.GetState();

        if (keyboardState.IsKeyDown(Keys.Escape) || keyboardState.IsKeyDown(Keys.Q))
            Exit();
        
        // Toggle trail mode with T key
        if (keyboardState.IsKeyDown(Keys.T) && !_previousKeyboardState.IsKeyDown(Keys.T))
        {
            _trailModeEnabled = !_trailModeEnabled;
        }
        
        // Change maze size with S, M, L keys
        if (keyboardState.IsKeyDown(Keys.S) && !_previousKeyboardState.IsKeyDown(Keys.S))
        {
            _currentMazeSize = MazeSize.Small;
            UpdateMazeSize();
            GenerateMaze();
            _playerPosition = GetRandomStartPosition();
            _gameWon = false;
            _gameFailed = false;
            _stepCount = 0;
            _visitedTiles.Clear();
            _visitedTiles.Add(((int)_playerPosition.X, (int)_playerPosition.Y));
        }
        
        if (keyboardState.IsKeyDown(Keys.M) && !_previousKeyboardState.IsKeyDown(Keys.M))
        {
            _currentMazeSize = MazeSize.Medium;
            UpdateMazeSize();
            GenerateMaze();
            _playerPosition = GetRandomStartPosition();
            _gameWon = false;
            _gameFailed = false;
            _stepCount = 0;
            _visitedTiles.Clear();
            _visitedTiles.Add(((int)_playerPosition.X, (int)_playerPosition.Y));
        }
        
        if (keyboardState.IsKeyDown(Keys.L) && !_previousKeyboardState.IsKeyDown(Keys.L))
        {
            _currentMazeSize = MazeSize.Large;
            UpdateMazeSize();
            GenerateMaze();
            _playerPosition = GetRandomStartPosition();
            _gameWon = false;
            _gameFailed = false;
            _stepCount = 0;
            _visitedTiles.Clear();
            _visitedTiles.Add(((int)_playerPosition.X, (int)_playerPosition.Y));
        }
        
        // Press R to restart at any time (preserves current size)
        if (keyboardState.IsKeyDown(Keys.R) && !_previousKeyboardState.IsKeyDown(Keys.R))
        {
            GenerateMaze();
            _playerPosition = GetRandomStartPosition();
            _gameWon = false;
            _gameFailed = false;
            _stepCount = 0;
            _visitedTiles.Clear();
            _visitedTiles.Add(((int)_playerPosition.X, (int)_playerPosition.Y));
        }

        if (!_gameWon && !_gameFailed)
        {
            
            // Check for arrow key presses
            Vector2 newPosition = _playerPosition;
            bool moved = false;
            
            if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
            {
                newPosition.Y--;
                moved = true;
            }
            if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
            {
                newPosition.Y++;
                moved = true;
            }
            if (keyboardState.IsKeyDown(Keys.Left) && !_previousKeyboardState.IsKeyDown(Keys.Left))
            {
                newPosition.X--;
                moved = true;
            }
            if (keyboardState.IsKeyDown(Keys.Right) && !_previousKeyboardState.IsKeyDown(Keys.Right))
            {
                newPosition.X++;
                moved = true;
            }
            
            // Check collision with walls
            int x = (int)newPosition.X;
            int y = (int)newPosition.Y;
            
            if (x >= 0 && x < _maze.GetLength(1) && y >= 0 && y < _maze.GetLength(0))
            {
                if (_maze[y, x] != 1) // Not a wall
                {
                    _playerPosition = newPosition;
                    _visitedTiles.Add((x, y));
                    
                    // Increment step count only if actually moved
                    if (moved)
                    {
                        _stepCount++;
                        
                        // Check if exceeded max steps
                        if (_stepCount > _maxSteps)
                        {
                            _gameFailed = true;
                        }
                    }
                    
                    // Check if reached the end
                    if (_maze[y, x] == 2)
                    {
                        _gameWon = true;
                    }
                }
            }
            
            _previousKeyboardState = keyboardState;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        // Draw maze
        for (int y = 0; y < _maze.GetLength(0); y++)
        {
            for (int x = 0; x < _maze.GetLength(1); x++)
            {
                Rectangle rect = new(x * TileSize, y * TileSize, TileSize, TileSize);
                
                if (_maze[y, x] == 1) // Wall
                {
                    _spriteBatch.Draw(_pixelTexture, rect, Color.DarkBlue);
                }
                else if (_maze[y, x] == 2) // End
                {
                    _spriteBatch.Draw(_pixelTexture, rect, Color.Gold);
                }
                else // Path
                {
                    Color pathColor = Color.LightGray;
                    
                    // Show trail if enabled and this tile was visited
                    if (_trailModeEnabled && _visitedTiles.Contains((x, y)))
                    {
                        pathColor = Color.Cyan * 0.4f; // Faint cyan for visited tiles
                    }
                    
                    _spriteBatch.Draw(_pixelTexture, rect, pathColor);
                }
            }
        }
        
        // Draw player (smiley face)
        int playerX = (int)_playerPosition.X * TileSize;
        int playerY = (int)_playerPosition.Y * TileSize;
        int centerX = playerX + TileSize / 2;
        int centerY = playerY + TileSize / 2;
        int radius = TileSize / 3;
        
        // Face circle (yellow)
        DrawCircle(centerX, centerY, radius, Color.Yellow);
        
        // Eyes
        DrawCircle(centerX - radius / 3, centerY - radius / 3, radius / 5, Color.Black);
        DrawCircle(centerX + radius / 3, centerY - radius / 3, radius / 5, Color.Black);
        
        // Smile
        DrawArc(centerX, centerY + radius / 4, radius / 2, Color.Black);
        
        // Trail mode indicator - cyan dot in top-left
        if (_trailModeEnabled)
        {
            DrawCircle(20, 20, 8, Color.Cyan);
        }
        
        // Step counter - visual dots in top-right
        int dotsToShow = System.Math.Min(_stepCount, _maxSteps); // Cap at _maxSteps for display
        for (int i = 0; i < dotsToShow; i++)
        {
            int dotX = _graphics.PreferredBackBufferWidth - 25 - (i % 10) * 6;
            int dotY = 15 + (i / 10) * 12;
            Color dotColor = _stepCount > _maxSteps ? Color.Red : Color.White;
            DrawCircle(dotX, dotY, 2, dotColor);
        }
        
        // Win message - big gold circle
        if (_gameWon)
        {
            // Pulsing gold circle in center
            int pulseRadius = 80 + (int)(System.Math.Sin(gameTime.TotalGameTime.TotalSeconds * 3) * 10);
            DrawCircle(
                _graphics.PreferredBackBufferWidth / 2,
                _graphics.PreferredBackBufferHeight / 2,
                pulseRadius,
                Color.Gold * 0.8f
            );
            
            // Draw checkmark-like shape
            DrawCircle(
                _graphics.PreferredBackBufferWidth / 2,
                _graphics.PreferredBackBufferHeight / 2,
                50,
                Color.Green
            );
        }
        
        // Failure message - big red X
        if (_gameFailed)
        {
            // Pulsing red circle in center
            int pulseRadius = 80 + (int)(System.Math.Sin(gameTime.TotalGameTime.TotalSeconds * 3) * 10);
            DrawCircle(
                _graphics.PreferredBackBufferWidth / 2,
                _graphics.PreferredBackBufferHeight / 2,
                pulseRadius,
                Color.Red * 0.8f
            );
            
            // Draw X shape
            DrawCircle(
                _graphics.PreferredBackBufferWidth / 2,
                _graphics.PreferredBackBufferHeight / 2,
                50,
                Color.DarkRed
            );
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
    
    private void DrawCircle(int centerX, int centerY, int radius, Color color)
    {
        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                if (x * x + y * y <= radius * radius)
                {
                    Rectangle pixel = new Rectangle(centerX + x, centerY + y, 1, 1);
                    _spriteBatch.Draw(_pixelTexture, pixel, color);
                }
            }
        }
    }
    
    private void DrawArc(int centerX, int centerY, int radius, Color color)
    {
        // Draw a simple smile arc
        for (int x = -radius; x <= radius; x++)
        {
            int y = (int)(System.Math.Sqrt(radius * radius - x * x) / 2);
            Rectangle pixel = new Rectangle(centerX + x, centerY + y, 2, 2);
            _spriteBatch.Draw(_pixelTexture, pixel, color);
        }
    }
    
    private Vector2 GetRandomStartPosition()
    {
        // Find a random path tile that is at least 6 tiles away from the goal
        const int minDistance = 6;
        List<Vector2> validPositions = new List<Vector2>();
        
        for (int y = 0; y < _maze.GetLength(0); y++)
        {
            for (int x = 0; x < _maze.GetLength(1); x++)
            {
                // Check if it's a path tile (not wall or goal)
                if (_maze[y, x] == 0)
                {
                    // Calculate Manhattan distance to goal
                    int distance = System.Math.Abs(x - (int)_goalPosition.X) + 
                                  System.Math.Abs(y - (int)_goalPosition.Y);
                    
                    if (distance >= minDistance)
                    {
                        validPositions.Add(new Vector2(x, y));
                    }
                }
            }
        }
        
        // Return a random valid position
        if (validPositions.Count > 0)
        {
            return validPositions[_random.Next(validPositions.Count)];
        }
        
        // Fallback to (1, 1) if no valid positions found
        return new Vector2(1, 1);
    }
    
    private void GenerateMaze()
    {
        // Initialize maze with all walls
        _maze = new int[_mazeHeight, _mazeWidth];
        for (int y = 0; y < _mazeHeight; y++)
        {
            for (int x = 0; x < _mazeWidth; x++)
            {
                _maze[y, x] = 1; // All walls
            }
        }
        
        // Use recursive backtracking to carve paths
        // Start from position (1, 1)
        CarvePath(1, 1);
        
        // Find a valid path tile in the bottom-right area for the goal
        _goalPosition = FindGoalPosition();
        _maze[(int)_goalPosition.Y, (int)_goalPosition.X] = 2;
    }
    
    private Vector2 FindGoalPosition()
    {
        // Search for path tiles in bottom-right quadrant, starting from corner
        List<Vector2> validGoalPositions = new List<Vector2>();
        
        // Start from the bottom-right and work inward
        for (int y = _mazeHeight - 2; y >= _mazeHeight / 2; y--)
        {
            for (int x = _mazeWidth - 2; x >= _mazeWidth / 2; x--)
            {
                if (_maze[y, x] == 0) // Found a path tile
                {
                    validGoalPositions.Add(new Vector2(x, y));
                }
            }
        }
        
        // Return the closest to bottom-right corner, or random if multiple options
        if (validGoalPositions.Count > 0)
        {
            // Prefer positions closer to bottom-right by sorting
            validGoalPositions.Sort((a, b) =>
            {
                int distA = (_mazeWidth - 1 - (int)a.X) + (_mazeHeight - 1 - (int)a.Y);
                int distB = (_mazeWidth - 1 - (int)b.X) + (_mazeHeight - 1 - (int)b.Y);
                return distA.CompareTo(distB);
            });
            
            // Return the closest one (first in sorted list)
            return validGoalPositions[0];
        }
        
        // Fallback: find any path tile
        for (int y = 1; y < _mazeHeight - 1; y++)
        {
            for (int x = 1; x < _mazeWidth - 1; x++)
            {
                if (_maze[y, x] == 0)
                {
                    return new Vector2(x, y);
                }
            }
        }
        
        // Last resort fallback
        return new Vector2(_mazeWidth - 2, _mazeHeight - 2);
    }
    
    private void CarvePath(int x, int y)
    {
        // Mark current cell as path
        _maze[y, x] = 0;
        
        // Create array of directions (right, down, left, up)
        var directions = new[] { (2, 0), (0, 2), (-2, 0), (0, -2) };
        
        // Shuffle directions for randomness
        for (int i = directions.Length - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (directions[i], directions[j]) = (directions[j], directions[i]);
        }
        
        // Try each direction
        foreach (var (dx, dy) in directions)
        {
            int newX = x + dx;
            int newY = y + dy;
            
            // Check if the new position is valid and unvisited
            if (newX > 0 && newX < _mazeWidth - 1 && 
                newY > 0 && newY < _mazeHeight - 1 && 
                _maze[newY, newX] == 1)
            {
                // Carve path between current and new cell
                _maze[y + dy / 2, x + dx / 2] = 0;
                
                // Recursively carve from new cell
                CarvePath(newX, newY);
            }
        }
    }
    
    private void UpdateMazeSize()
    {
        _mazeWidth = (int)_currentMazeSize;
        _mazeHeight = (int)_currentMazeSize;
        
        // Set max steps based on maze size
        _maxSteps = _currentMazeSize switch
        {
            MazeSize.Small => 12,
            MazeSize.Medium => 24,
            MazeSize.Large => 48,
            _ => 24
        };
        
        // Update window size to fit the maze
        _graphics.PreferredBackBufferWidth = _mazeWidth * TileSize;
        _graphics.PreferredBackBufferHeight = _mazeHeight * TileSize;
        _graphics.ApplyChanges();
    }
}
