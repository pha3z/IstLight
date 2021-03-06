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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IstLight.Settings;
using Xunit;
using Xunit.Extensions;

namespace IstLight.UnitTests.Settings
{
    public class SimulationSettingsTests
    {
        [Theory, PropertyData("AllISettingImplementationsTestProvider")]
        public void AllSettingsHaveDefaultCtor(Type settingType)
        {
            Assert.True(settingType.HasDefaultConstructor(),
                string.Format("{0} should have default ctor.", settingType.Name));
        }

        [Theory, PropertyData("AllISettingImplementationsTestProvider")]
        public void ctor_CanInstantiate(Type settingType)
        {
            Assert.DoesNotThrow(() => CreateSetting(settingType));
        }

        [Theory, PropertyData("AllISettingImplementationsTestProvider")]
        public void CloneCreatesNewObject(Type settingType)
        {
            var sut = CreateSetting(settingType);
            
            Assert.NotSame(sut, sut.Clone());
        }

        [Theory]
        [ClassData(typeof(SettingPropertyProvider))]
        public void Clone_AssignsAllAutoProperties(object originalValue, object clonedValue, PropertyInfo propertyInfo)
        {
            Assert.True(
                object.Equals(originalValue, clonedValue),
                string.Format("{0}->{1}: Property was not clonned.\r\nExpected: {2}\r\nActual: {3}",
                    propertyInfo.ReflectedType.Name,
                    propertyInfo.Name,
                    originalValue,
                    clonedValue));
        }

        [Theory]
        [ClassData(typeof(SettingPropertyProvider))]
        public void Clone_ClonedSettingPropertiesHaveDifferentReferences(object originalValue, object clonedValue, PropertyInfo propertyInfo)
        {
            Assert.False(
                object.ReferenceEquals(originalValue,clonedValue),
                string.Format("{0}->{1}: Property in clonned object has the same reference as in original object",
                    propertyInfo.ReflectedType.Name,
                    propertyInfo.Name));
        }

        [Theory, PropertyData("AllISettingImplementationsTestProvider")]
        public void SimulationSettings_Get_CanInitializeAllSettings(Type settingType)
        {
            var sut = new SimulationSettings();
            MethodInfo getMethod = typeof(SimulationSettings).GetMethod("Get");
            MethodInfo genericGetMethod = getMethod.MakeGenericMethod(settingType);
            
            Assert.NotNull(genericGetMethod.Invoke(sut, null));
        }

        [Theory, PropertyData("AllISettingImplementationsTestProvider")]
        public void SimulationSettings_Get_RemembersCreatedObject(Type settingType)
        {
            var sut = new SimulationSettings();
            MethodInfo getMethod = typeof(SimulationSettings).GetMethod("Get");
            MethodInfo genericGetMethod = getMethod.MakeGenericMethod(settingType);

            object fromFirstGet = genericGetMethod.Invoke(sut, null);
            object fromSecondGet = genericGetMethod.Invoke(sut, null);
            
            Assert.Same(fromFirstGet, fromSecondGet);
        }

        [Fact]
        public void SimulationSettings_Clone_WorksWhenContainerIsEmpty()
        {
            Assert.DoesNotThrow(() => new SimulationSettings().Clone());
        }

        [Fact]
        public void SimulationSettings_Clone_ClonesRememberedChilds()
        {
            var container = new SimulationSettings();
            var setting = container.Get<CloseAllOnLastBarSetting>();
            setting.Value = !setting.Value;

            var sut = (container.Clone() as SimulationSettings);
            
            Assert.Equal<bool>(setting.Value, sut.Get<CloseAllOnLastBarSetting>().Value);
        }

        [Theory, PropertyData("AllISettingImplementationsTestProvider")]
        public void SimulationSettings_Clone_InvokesCloneOnChilds(Type settingType)
        {
            var container = new SimulationSettings();
            MethodInfo getMethod = typeof(SimulationSettings).GetMethod("Get");
            MethodInfo genericGetMethod = getMethod.MakeGenericMethod(settingType);

            object fromGetOnOriginal = genericGetMethod.Invoke(container, null);
            var sut = (container.Clone() as SimulationSettings);
            object fromGetOnClone = genericGetMethod.Invoke(sut, null);

            Assert.NotSame(fromGetOnOriginal, fromGetOnClone);
        }

        public static IEnumerable<Type> AllISettingImplementations
        {
            get
            {
                Type interfaceType = typeof(ISimulationSetting);
                return
                    interfaceType.Assembly
                        .GetTypes()
                        .Where(t => interfaceType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);
            }
        }
        public static IEnumerable<object[]> AllISettingImplementationsTestProvider
        {
            get
            {
                return AllISettingImplementations.Select(t => new object[] { t });
            }
        }

        public static ISimulationSetting CreateSetting(Type settingType)
        {
            return Activator.CreateInstance(settingType) as ISimulationSetting;
        }
    }
}
