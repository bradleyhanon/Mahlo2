using System.Collections.Generic;
using MahloService.Models;

namespace MahloService.Repository
{
  interface IDbLocal
  {
    IDbConnectionFactory ConnectionFactory { get; }

    IEnumerable<GreigeRoll> GetGreigeRolls();

    void AddGreigeRoll(GreigeRoll roll);

    void UpdateGreigeRoll(GreigeRoll roll);

    void DeleteGreigeRoll(GreigeRoll roll);

    string GetProgramState();
  }
}