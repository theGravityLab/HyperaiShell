using HyperaiShell.Foundation.Bots;
using System;
using System.Collections.Generic;

namespace HyperaiShell.App.Bots
{
    public class BotCollectionBuilder : IBotCollectionBuilder
    {
        private readonly List<IBotBuilder> botBuilders = new List<IBotBuilder>();

        IBotBuilder IBotCollectionBuilder.Add<TBot>()
        {
            BotBuilder builder = new BotBuilder(typeof(TBot));
            botBuilders.Add(builder);
            return builder;
        }

        public BotCollection Build(IServiceProvider provider)
        {
            BotCollection collection = new BotCollection();
            foreach (IBotBuilder builder in botBuilders)
            {
                collection.Add(builder.Build(provider));
            }
            return collection;
        }
    }
}