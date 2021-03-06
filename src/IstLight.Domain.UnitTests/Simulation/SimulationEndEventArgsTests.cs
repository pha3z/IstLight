﻿// Copyright 2012 Jakub Niemyjski
//
// This file is part of IstLight.
//
// IstLight is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// IstLight is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with IstLight.  If not, see <http://www.gnu.org/licenses/>.

using System;
using IstLight.Simulation;
using Xunit;

namespace IstLight.UnitTests.Simulation
{
    public class SimulationEndEventArgsTests
    {
        [Fact]
        public void HasDefaultCtor()
        {
            Assert.True(typeof(SimulationEndEventArgs).HasDefaultConstructor());
        }

        [Fact]
        public void EndReason_CanBeSet()
        {
            var sut = CreateSut();
            var endReason = SimulationEndReason.Completion;
            sut.EndReason = endReason;

            Assert.Equal<SimulationEndReason>(endReason, sut.EndReason);
        }

        [Fact]
        public void Result_CanBeSet()
        {
            var sut = CreateSut();
            var result = SimulationResultTests.CreateSut().Item1;
            sut.Result = result;

            Assert.Equal<SimulationResult>(result, sut.Result);
        }

        [Fact]
        public void Error_CanBeSet()
        {
            var sut = CreateSut();
            var error = Guid.NewGuid().ToString();
            sut.Error = error;

            Assert.Equal<string>(error, sut.Error);
        }

        public static SimulationEndEventArgs CreateSut()
        {
            return new SimulationEndEventArgs();
        }
    }
}
