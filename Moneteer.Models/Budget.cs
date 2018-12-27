﻿using Moneteer.Models.Validation;
using System;

namespace Moneteer.Models
{
    public class Budget : INamedModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public Currency Currency { get; set; }
        public decimal Available { get; set; }
        public SymbolLocation CurrencySymbolLocation { get; set;}
        public CurrencyFormat CurrencyFormat { get; set; }
        public string DateFormat { get; set; }
    }
}
