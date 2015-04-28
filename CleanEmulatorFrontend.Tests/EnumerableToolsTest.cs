using System.Collections.Generic;
using System.Linq;
using CleanEmulatorFrontend.GamesData;
using EnumerableUtils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CleanEmulatorFrontend.Tests
{
    [TestClass]
    public class EnumerableToolsTest
    {
        [TestMethod]
        public void RecurseFilterTest()
        {
            var nodes = new List<SystemNode>
            {
                new SystemNode
                {
                    Enabled = false,
                    Description = "0",
                    Items = new[]
                    {
                        new SystemNode
                        {
                            Enabled = true,
                            Description = "1"
                        }
                    }
                },
                new SystemNode
                {
                    Enabled = true,
                    Description = "2",
                    Items = new[]
                    {
                        new SystemNode
                        {
                            Enabled = true,
                            Description = "3"
                        },
                        new SystemNode
                        {
                            Enabled = true,
                            Description = "4"
                        }
                    }
                }
            };
            nodes.RecurseFilter(sn => sn.Items, sn => sn.Enabled);

            var systemNodes = nodes.Traverse(sn => sn.Items);
            systemNodes.Any(sn => sn.Description == "0").Should().BeFalse();
            systemNodes.Any(sn => sn.Description == "1").Should().BeFalse();
            systemNodes.Any(sn => sn.Description == "2").Should().BeTrue();
            systemNodes.Any(sn => sn.Description == "3").Should().BeTrue();
            systemNodes.Any(sn => sn.Description == "4").Should().BeTrue();
            systemNodes.Count().Should().Be(3);
        }
    }
}