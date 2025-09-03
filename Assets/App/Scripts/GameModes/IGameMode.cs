namespace DuolingoMusic.GameModes
{
    public interface IGameMode
    {
        void Initialize();
        void Update();
        void Cleanup();
        void OnInput(InputType inputType);
    }
    
    public enum InputType
    {
        Tap,
        Hold,
        Release
    }
}