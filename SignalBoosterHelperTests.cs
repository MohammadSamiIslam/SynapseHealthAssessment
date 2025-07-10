using Microsoft.VisualStudio.TestTools.UnitTesting;
using Synapse.SignalBoosterExample;

namespace Synapse.SignalBoosterExample.Tests
{
    [TestClass]
    public class SignalBoosterHelperTests
    {
        [TestMethod]
        public void ExtractDeviceType_ReturnsCPAP_WhenNoteMentionsCPAP()
        {
            // Arrange
            string note = "Patient needs a CPAP with full face mask.";

            // Act
            string result = SignalBoosterHelper.ExtractDeviceType(note);

            // Assert
            Assert.AreEqual("CPAP", result);
        }

        [TestMethod]
        public void ExtractDeviceType_ReturnsOxygenTank_WhenNoteMentionsOxygen()
        {
            string note = "Patient requires oxygen at night.";
            string result = SignalBoosterHelper.ExtractDeviceType(note);
            Assert.AreEqual("Oxygen Tank", result);
        }

        [TestMethod]
        public void ExtractDeviceType_ReturnsUnknown_WhenNoteMentionsNothing()
        {
            string note = "No device needed.";
            string result = SignalBoosterHelper.ExtractDeviceType(note);
            Assert.AreEqual("Unknown", result);
        }
    }
}