using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace monogamekit;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _pixelTexture;
    
    // Maze: 0 = path, 1 = wall, 2 = end
    private readonly int[,] _maze = new int[,]
    {
        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        {1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1},
        {1, 0, 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1, 1},
        {1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1},
        {1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1},
        {1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1},
        {1, 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1},
        {1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1},
        {1, 0, 1, 1, 1, 1, 1, 0, 1, 0, 1, 0, 1, 1},
        {1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        {1, 1, 1, 0, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1},
        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        {1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 2, 1},
        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
    };
    
    private const int TileSize = 40;
    private const int MaxSteps = 20;
    private readonly Vector2 _goalPosition = new(12, 12);
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
        
        // Set window size to fit the maze
        _graphics.PreferredBackBufferWidth = _maze.GetLength(1) * TileSize;
        _graphics.PreferredBackBufferHeight = _maze.GetLength(0) * TileSize;
    }

    protected override void Initialize()
    {
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
        
        if (keyboardState.IsKeyDown(Keys.Escape))
            Exit();
        
        // Toggle trail mode with T key
        if (keyboardState.IsKeyDown(Keys.T) && !_previousKeyboardState.IsKeyDown(Keys.T))
        {
            _trailModeEnabled = !_trailModeEnabled;
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
                        if (_stepCount > MaxSteps)
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
        else
        {
            // Press R to restart
            if (keyboardState.IsKeyDown(Keys.R))
            {
                _playerPosition = GetRandomStartPosition();
                _gameWon = false;
                _gameFailed = false;
                _stepCount = 0;
                _visitedTiles.Clear();
                _visitedTiles.Add(((int)_playerPosition.X, (int)_playerPosition.Y));
            }
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
                Rectangle rect = new Rectangle(x * TileSize, y * TileSize, TileSize, TileSize);
                
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
        int dotsToShow = System.Math.Min(_stepCount, MaxSteps); // Cap at MaxSteps for display
        for (int i = 0; i < dotsToShow; i++)
        {
            int dotX = _graphics.PreferredBackBufferWidth - 25 - (i % 10) * 6;
            int dotY = 15 + (i / 10) * 12;
            Color dotColor = _stepCount > MaxSteps ? Color.Red : Color.White;
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
}
