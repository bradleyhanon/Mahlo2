using System.Collections.Generic;
using Mahlo.Models;

namespace Mahlo.Repository
{
  interface IDbLocal
  {
    IDbConnectionFactory ConnectionFactory { get; }

    IEnumerable<GreigeRoll> GetGreigeRolls();

    void AddGreigeRoll(GreigeRoll roll);

    void UpdateGreigeRoll(GreigeRoll roll);

    void DeleteGreigeRoll(GreigeRoll roll);
  }
}