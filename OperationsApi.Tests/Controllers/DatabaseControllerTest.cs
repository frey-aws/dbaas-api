using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OperationsApi.BusinessLogic.Command;

namespace OperationsApi.BusinessLogic.Tests
{
    [TestClass]
    public class AmazonRdsCommandTest
    {
        AmazonRdsCommand amazonRdsCommand;

        private AmazonRdsCommandTest()
        {
            amazonRdsCommand = new AmazonRdsCommand();
        }

        [TestMethod]
        public void Index()
        {
            // Assert
            // Assert.IsNotNull(result);
            // Assert.AreEqual("Home Page", result.ViewBag.Title);
        }
    }
}
