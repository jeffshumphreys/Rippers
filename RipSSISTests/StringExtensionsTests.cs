using Xunit;
using ExtensionMethods;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ExtensionMethods.StringExtensions;

namespace ExtensionMethods.Tests
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("x", "x", new string[] { "x", "x" } )]
        [InlineData("x", "xxx", new string[] { "xx", "x" } )]
        [InlineData("x", "xyx", new string[] { "yx", "xy", "xy", "yx" } )]
        public void ReplaceRecurseAllAgainTest(string expectedString, string input, string[] replacepairs)
        {
            string actualResult = input.ReplaceRecurseAllAgain(replacepairs);

            Assert.Equal(expectedString, actualResult);
        }
    }
}