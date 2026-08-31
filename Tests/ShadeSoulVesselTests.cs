using LegacyoftheAbyss.Shade;
using Xunit;

namespace LegacyoftheAbyss.Tests
{
    /// <summary>
    /// The Soul Vessel rules, stated as assertions. Every one of these is a rule that is otherwise
    /// only visible while playing with three vessels and a full meter, which is a long way into a
    /// run to find out that a piece of arithmetic was wrong.
    /// </summary>
    public class ShadeSoulVesselTests
    {
        [Theory]
        [InlineData(9, 0)]   // where a run starts
        [InlineData(10, 0)]  // one spool upgrade is not yet a vessel
        [InlineData(11, 1)]
        [InlineData(12, 1)]
        [InlineData(15, 3)]
        [InlineData(18, 3)]  // Hornet's own maximum; the vessels cap first
        [InlineData(99, 3)]
        [InlineData(0, 0)]   // before her save has been read at all
        public void VesselsAreEarnedEveryTwoSilkUpgrades(int silkMax, int expected)
        {
            Assert.Equal(expected, ShadeSoulVessels.VesselsFromSilkMax(silkMax));
        }

        [Fact]
        public void ThreeVesselsDoubleTheCarriedSoul()
        {
            Assert.Equal(99, ShadeSoulVessels.Capacity(ShadeSoulVessels.MaxVessels));
            Assert.Equal(0, ShadeSoulVessels.Capacity(0));
            Assert.Equal(99, ShadeSoulVessels.Capacity(5));
        }

        /// <summary>
        /// The invariant the whole single-number representation exists to guarantee: no soul sits in
        /// a higher vessel while a lower one has room.
        /// </summary>
        [Fact]
        public void VesselsFillFromTheBottom()
        {
            Assert.Equal(new[] { 20, 0, 0 }, Split(20));
            Assert.Equal(new[] { 33, 0, 0 }, Split(33));
            Assert.Equal(new[] { 33, 1, 0 }, Split(34));
            Assert.Equal(new[] { 33, 33, 33 }, Split(99));
            Assert.Equal(new[] { 0, 0, 0 }, Split(0));
        }

        [Fact]
        public void SoulFillsTheMeterBeforeTheVessels()
        {
            ShadeSoulVessels.Add(11, soul: 0, soulMax: 99, reserve: 0, capacity: 99, out int newSoul, out int newReserve);
            Assert.Equal(11, newSoul);
            Assert.Equal(0, newReserve);
        }

        [Fact]
        public void SoulPastAFullMeterOverflowsIntoTheVessels()
        {
            ShadeSoulVessels.Add(11, soul: 95, soulMax: 99, reserve: 0, capacity: 99, out int newSoul, out int newReserve);
            Assert.Equal(99, newSoul);
            Assert.Equal(7, newReserve);
        }

        [Fact]
        public void SoulPastBothIsLost()
        {
            ShadeSoulVessels.Add(50, soul: 99, soulMax: 99, reserve: 95, capacity: 99, out int newSoul, out int newReserve);
            Assert.Equal(99, newSoul);
            Assert.Equal(99, newReserve);
        }

        [Fact]
        public void SoulWithNoVesselsStopsAtTheMeter()
        {
            ShadeSoulVessels.Add(20, soul: 99, soulMax: 99, reserve: 0, capacity: 0, out int newSoul, out int newReserve);
            Assert.Equal(99, newSoul);
            Assert.Equal(0, newReserve);
        }

        /// <summary>
        /// A single point of soul has to be visible. The stage is a five-frame animation rather than
        /// a fill, so rounding a point down would make the first hit after a full meter look like
        /// nothing happened at all.
        /// </summary>
        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(8, 1)]
        [InlineData(9, 1)]
        [InlineData(17, 2)]
        [InlineData(25, 3)]
        [InlineData(32, 3)]
        [InlineData(33, 4)]
        public void AVesselDrawsTheStageItsContentsEarn(int held, int expectedStage)
        {
            Assert.Equal(expectedStage, ShadeSoulVessels.StageFor(held));
        }

        [Fact]
        public void OnlyAnExactlyFullVesselDrawsAsFull()
        {
            Assert.Equal(4, ShadeSoulVessels.StageFor(ShadeSoulVessels.SoulPerVessel));
            Assert.NotEqual(4, ShadeSoulVessels.StageFor(ShadeSoulVessels.SoulPerVessel - 1));
        }

        [Fact]
        public void DebugSpendingEmptiesTheMeterBeforeTheReserve()
        {
            ShadeSoulVessels.Spend(11, soul: 20, reserve: 66, out int newSoul, out int newReserve);
            Assert.Equal(9, newSoul);
            Assert.Equal(66, newReserve);

            ShadeSoulVessels.Spend(11, soul: 5, reserve: 66, out newSoul, out newReserve);
            Assert.Equal(0, newSoul);
            Assert.Equal(60, newReserve);

            ShadeSoulVessels.Spend(500, soul: 5, reserve: 66, out newSoul, out newReserve);
            Assert.Equal(0, newSoul);
            Assert.Equal(0, newReserve);
        }

        private static int[] Split(int reserve)
        {
            var held = new int[ShadeSoulVessels.MaxVessels];
            for (int i = 0; i < held.Length; i++)
            {
                held[i] = ShadeSoulVessels.HeldInVessel(reserve, i);
            }

            // Stated as an invariant rather than only as the expected numbers: a vessel above a
            // vessel that is not yet full is the one state this representation must never produce.
            for (int i = 1; i < held.Length; i++)
            {
                if (held[i] > 0)
                {
                    Assert.Equal(ShadeSoulVessels.SoulPerVessel, held[i - 1]);
                }
            }

            return held;
        }
    }
}
