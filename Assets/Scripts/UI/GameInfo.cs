namespace Assets.Scripts.UI
{
	public class GameInfo
	{
		static GameInfo(){}
		private GameInfo(){}
		public static GameInfo Instance { get; } = new GameInfo();

		// Info
		public float Time;
		public int   Boost;
		public int   Speed;
	}
}
