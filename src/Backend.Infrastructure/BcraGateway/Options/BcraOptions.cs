using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Infrastructure.BcraGateway.Options
{
    public class BcraOptions
    {
        public const string SectionName = "Bcra";
        public string BaseUrl { get; init; } = "https://api.bcra.gob.ar/";
        public int TimeoutSeconds { get; init; } = 15;
    }
}