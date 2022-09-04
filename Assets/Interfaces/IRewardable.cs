namespace Interfaces
{
	public interface IRewardable
	{
		public long Reward { get; set; }

		public void GetReward(long newReward);
	}
}
