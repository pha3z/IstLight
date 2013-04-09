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
using System.Reflection;
using System.Linq;
using ScriptingWrapper.Attributes;
using System.IO;

namespace ScriptingWrapper
{
    /// <summary>
    /// Creates script engine wrappers
    /// </summary>
    public static class ScriptEngineFactory
    {
        private static Dictionary<ScriptingLanguage, Type> languageType;
        private static Dictionary<string, ScriptingLanguage> languageExtensions;

        private static void AssignSyntaxHighlighting(ScriptEngineBase engine)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName =
                assembly.GetManifestResourceNames()
                    .Where(x => x.EndsWith("xshd") && x.Contains(engine.Language.ToString()))
                    .Single();

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var streamReader = new StreamReader(stream))
                engine.SyntaxHighlightingRules = streamReader.ReadToEnd();
        }

        static ScriptEngineFactory()
        {
            languageType = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.IsClass && !x.IsAbstract && typeof(ScriptEngineBase).IsAssignableFrom(x))
                .ToDictionary(x => ((LanguageAttribute)x.GetCustomAttributes(typeof(LanguageAttribute), true).First()).Language);
            languageExtensions = languageType.ToDictionary(x =>
                ((AllowedExtensionAttribute)x.Value
                    .GetCustomAttributes(typeof(AllowedExtensionAttribute), true)
                    .Single())
                .Extension, x => x.Key);
        }

        

        /// <summary>
        /// Creates script engine according to passed language argument.
        /// </summary>
        /// <param name="language">Script engine type to create.</param>
        /// <returns>Script engine wrapper.</returns>
        public static ScriptEngineBase CreateEngine(ScriptingLanguage language)
        {
            var engine = (ScriptEngineBase)Activator.CreateInstance(languageType[language]);
            AssignSyntaxHighlighting(engine);
            return engine;
        }

        

        /// <summary>
        /// Creates script engine according to passed script language extension (i.e. 'py' for python)
        /// </summary>
        /// <param name="scriptExtension">Script language extension.</param>
        /// <returns>Script engine wrapper.</returns>
        /// <exception cref="ArgumentNullException">Thrown if scriptExtension argument is null.</exception>
        public static ScriptEngineBase TryCreateEngine(string scriptExtension)
        {
            if (scriptExtension == null) throw new ArgumentNullException("scriptExtension");

            string ext = scriptExtension.Replace(".", "").ToLower();
            if (!languageExtensions.ContainsKey(ext))
                return null;
            else
                return CreateEngine(languageExtensions[ext]);
        }
        
        public static IEnumerable<KeyValuePair<string, ScriptingLanguage>> ExtensionToLanguageMap
        {
            get
            {
                return languageExtensions;
            }
        }

        /// <summary>
        /// Supported scipt language extensions.
        /// </summary>
        public static IEnumerable<string> AllowedExtensions { get { return languageExtensions.Keys; } }
    }
}
