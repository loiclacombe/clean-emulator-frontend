using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CleanEmulatorFrontend.Tests
{
    [TestClass]
    public class MainWindowTest
    {
        private MainWindow _mainWindow;
        private Mock<AppLoader> _appLoaderMock;
        private Mock<Random> _random;

        [TestMethod]
        public void Initialize()
        {
            _appLoaderMock = new Mock<AppLoader>();
            _random=new Mock<Random>();

            _mainWindow = new MainWindow(_appLoaderMock.Object, _random.Object);
        }

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
