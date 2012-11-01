﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IstLight.Settings
{
    public class SimulationPriceSetting : ISimulationSetting
    {
        public SimulationPriceSetting()
        {
            Type = SimulationPriceType.Open;
        }

        public SimulationPriceType Type { get; set; }

        public object Clone()
        {
            return new SimulationPriceSetting { Type = this.Type };
        }
    }
}
