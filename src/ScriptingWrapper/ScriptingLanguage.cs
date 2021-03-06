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
using System.Reflection;
using ScriptingWrapper.Attributes;

namespace ScriptingWrapper
{
    /// <summary>
    /// Supported script languages.
    /// </summary>
    public enum ScriptingLanguage
    {
        [Description("Iron Python")]
        IronPython,
        [Description("Iron Ruby")]
        IronRuby,
        [Description("C#")]
        CSharp,
        [Description("VB.NET")]
        VBNet,
        [Description("JScript")]
        JScript
    }

    public static class ScriptLanguageExtensions
    {
        public static string GetDescription(this ScriptingLanguage language)
        {
            var type = typeof(ScriptingLanguage);
            FieldInfo field = type.GetField(language.ToString());

            DescriptionAttribute attribute
                    = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            return attribute == null ? language.ToString() : attribute.Description;
        }
    }
}
