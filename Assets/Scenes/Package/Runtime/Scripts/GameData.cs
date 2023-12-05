namespace mimiland.minigame
{
    public class GameData : IGameData
    {
        public float TimeTillNextState { get; set; }

        public GameData(float timeTillNextState)
        {
            TimeTillNextState = timeTillNextState;
        }
    }

    public interface IGameData
    {
    }
}