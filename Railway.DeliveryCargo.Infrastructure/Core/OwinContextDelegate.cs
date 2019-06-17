using System.Collections.Generic;
using System.Threading.Tasks;

namespace Railway.DeliveryCargo.Infrastructure.Core
{
    public delegate Task AppFunc(IDictionary<string, object> env);
}