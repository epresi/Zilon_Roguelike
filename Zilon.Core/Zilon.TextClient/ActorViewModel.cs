﻿namespace Zilon.TextClient
{
    internal class ActorViewModel : IActorViewModel
    {
        public IActor Actor { get; set; }
        public object Item => Actor;
    }
}