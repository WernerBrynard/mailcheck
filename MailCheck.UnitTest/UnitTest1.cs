using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MailCheck.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        [TestMethod]
        public void SuggestAddress()
        {  
            string input = "werner.brynard@gmal.com";
            string expectedOutput = "werner.brynard@gmail.com";

            MailCheck mailCheck = new MailCheck();

            string result = mailCheck.Run(input, null, null);

            Assert.AreEqual(expectedOutput, result, "Email fixed");
        }

    }
}
