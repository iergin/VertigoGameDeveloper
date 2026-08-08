namespace Vertigo.Domain.Rewards
{
    public readonly struct RewardGrant
    {
        public readonly string RewardId;
        public readonly int Amount;

        public RewardGrant(string rewardId, int amount)
        {
            RewardId = rewardId;
            Amount = amount;
        }
    }
}

