﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AardvarkREST.Models
{
    public class WFRouteRepository : IWFRouteRepository
    {
        private WFContext _WFContext;

        public WFRouteRepository(WFContext existingContext)
        {
            _WFContext = existingContext;
        }
        public async Task<WFRoute> Get(string ChartName, string TaskFrom, string TaskTo)
        {
            WFRoute ret = await _WFContext.WFRouteGetByName( ChartName,  TaskFrom,  TaskTo);
            return ret;
        }

        public async Task<WFRoute> Save(WFRoute Route)
        {
            var ret = await _WFContext.WFRouteSave(Route);
            return ret;
        }

        public async Task DeleteByNames(string ChartName, string TaskNameFrom, string TaskNameTo)
        {
            await _WFContext.WFRouteDeleteByName(ChartName, TaskNameFrom, TaskNameTo);
        }

        public async Task<IEnumerable<WFRoute>> FindAll(string ChartName)
        {
            return await _WFContext.WFRouteFindAll(ChartName);
        }
    }
}
