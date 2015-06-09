using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Cards
{
    /// <summary>
    /// Main class for View Model
    /// TODO: follow guidelines
    /// </summary>
    public class CardsViewModel
    {
        private readonly IDispatcher _dispatcher;
        public CardsViewModel(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }
    }
}
