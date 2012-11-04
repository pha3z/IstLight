﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IstLight.Services;
using Ninject.Modules;

namespace IstLight.WpfClient.Bootstrapper
{
    public class Bindings : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IAsyncLoadService<ITickerProvider>>().To<TickerProviderService>();
            Kernel.Bind<IAsyncLoadService<ITickerConverter>>().To<TickerConverterService>();
            Kernel.Bind<IAsyncLoadService<ITickerTransformer>>().To<TickerTransformerService>();
            Kernel.Bind<IAsyncLoadService<IResultAnalyzer>>().To<ResultAnalyzerService>();

            Kernel.Bind<IScriptLoadService>().To<ScriptsFromDirectory>()
                .WhenInjectedInto<TickerProviderService>().WithConstructorArgument("path", "scripts\\providers");
            Kernel.Bind<IScriptLoadService>().To<ScriptsFromDirectory>()
                .WhenInjectedInto<TickerConverterService>().WithConstructorArgument("path", "scripts\\converters");
            Kernel.Bind<IScriptLoadService>().To<ScriptsFromDirectory>()
                .WhenInjectedInto<TickerTransformerService>().WithConstructorArgument("path", "scripts\\transformers");
            Kernel.Bind<IScriptLoadService>().To<ScriptsFromDirectory>()
                .WhenInjectedInto<ResultAnalyzerService>().WithConstructorArgument("path", "scripts\\analyzers");
        }
    }
}
