using Vertigo.Domain.Rewards;

namespace Vertigo.Domain.Spin
{
    public readonly struct SpinResult
    {
        public readonly int SliceIndex;
        public readonly bool IsBomb;
        public readonly RewardGrant Reward;

        public SpinResult(int sliceIndex, bool isBomb, RewardGrant reward)
        {
            SliceIndex = sliceIndex;
            IsBomb = isBomb;
            Reward = reward;
        }
    }
}

