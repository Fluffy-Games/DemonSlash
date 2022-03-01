public class GameManager : MonoSingleton<GameManager>
{
    public enum GameState 
    {
        Prepare,
        MainGame,
        Idle,
        Lose,
        Victory
    }
    private GameState _currentGameState;
    public GameState CurrentGameState
    {
        get => _currentGameState;
        set
        {
            switch (value)
            {
                case GameState.Prepare:
                    break;
                case GameState.MainGame:                       
                    break;
                case GameState.Idle:
                    break;
                case GameState.Lose:                       
                    break;
                case GameState.Victory:                        
                    break;
            }
            _currentGameState = value;
        }           
    }
}