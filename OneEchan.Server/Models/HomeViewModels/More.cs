﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OneEchan.Core.Models;

namespace OneEchan.Server.Models.HomeViewModels
{
    public class More
    {
        public PagedListResult<HomeListModel> Result { get; internal set; }
    }
}
