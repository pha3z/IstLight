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
using System.Text;
using System.Windows.Input;
using IstLight.Excel;

namespace IstLight.ViewModels
{
    public class SimulationResultViewModel
    {
        public SimulationResultViewModel(IEnumerable<ISectionViewModel> sections, IExcelExporter<SimulationResultViewModel> excelExporter)
        {
            this.Sections = sections;
            this.ExportToExcelCommand = excelExporter.GetExportCommand(this);
        }

        public IEnumerable<ISectionViewModel> Sections { get; private set; }

        public ICommand ExportToExcelCommand { get; private set; }
    }
}
