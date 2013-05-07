﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NSubstitute;
using NUnit.Framework;

namespace RestaurantKata
{
    public class Cook : IOrderConsumer
    {
        private readonly IOrderConsumer nextStep;

        private readonly Dictionary<string, string[]> recipes =
            new Dictionary<string, string[]>()
                {
                    { "Sake", new[] { "Bottle of sake" } },
                    { "Sushi", new[] { "Sushi Rice", "Vinegar", "Sugar", "Salt", "Salmon", "Nori", "Wasabi", "Imitation roe" } },
                    { "Clean glass", new[] { "Clean glass" } },
                };

        public Cook(IOrderConsumer nextStep)
        {
            Name = Guid.NewGuid().ToString();
            this.nextStep = nextStep;
        }

        public string Name { get; set; }

        private void PrepareFood(Order order)
        {
            Thread.Sleep(new Random().Next(0, 1000));
            Console.WriteLine("Food prepared by: " + Name);
            order.CookTime = 500;
            foreach (var item in order.Items)
            {
                item.Ingredients = recipes[item.ItemDescription];
            }
        }

        public void Consume(Order order)
        {
            PrepareFood(order);
            nextStep.Consume(order);
        }
    }

    [TestFixture]
    public class CookTests
    {
        [Test]
        public void GivenAnOrder_TheNextStepShouldBeCalled()
        {
            var nextStep = Substitute.For<IOrderConsumer>();
            var cook = new Cook(nextStep);
            var order = Given.AnOrderForJapanese();
            
            cook.Consume(order);

            nextStep.Received(1).Consume(order);
        }

        [Test]
        public void GivenAnOrder_TheOrderShouldBeEnrichedWithIngredients()
        {
            var nextStep = Substitute.For<IOrderConsumer>();
            var cook = new Cook(nextStep);
            var order = Given.AnOrderForJapanese();
            
            cook.Consume(order);

            Assert.That(order.Items.All(x => x.Ingredients.Any()));
        }
    }
}