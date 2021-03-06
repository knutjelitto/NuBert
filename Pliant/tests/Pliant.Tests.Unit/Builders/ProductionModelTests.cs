﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders;
using System.Linq;

namespace Pliant.Tests.Unit.Builders
{
    [TestClass]
    public class ProductionModelTests
    {
        [TestMethod]
        public void ProductionModelToProductionsShouldReturnNullableProductionWhenRuleIsEmpty()
        {
            var E = ProductionModel.From("E");
            Assert.AreEqual(1, E.ToProductions().Count());
        }
    }
}
