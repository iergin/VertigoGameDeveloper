namespace Vertigo.Domain.Spin
{
    public readonly struct SliceModel
    {
        public readonly string RewardId;

        public readonly int BaseAmount;

        public readonly float Weight;

        public readonly bool IsBomb;

        public SliceModel(string rewardId, int baseAmount, float weight, bool isBomb)
        {
            RewardId = rewardId;
            BaseAmount = baseAmount;
            Weight = weight;
            IsBomb = isBomb;
        }

        public static SliceModel Bomb(float weight)
            => new SliceModel(null, 0, weight, true);

        public static SliceModel Reward(string rewardId, int baseAmount, float weight)
            => new SliceModel(rewardId, baseAmount, weight, false);
    }
}

