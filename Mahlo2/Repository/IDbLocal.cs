using System.Collections.Generic;
using Mahlo.Models;

namespace Mahlo.Repository
{
  interface IDbLocal
  {
    IDbConnectionFactory ConnectionFactory { get; }

    IEnumerable<CarpetRoll> GetCarpetRolls();

    void AddCarpetRoll(CarpetRoll roll);

    void UpdateCarpetRoll(CarpetRoll roll);

    void DeleteCarpetRoll(CarpetRoll roll);

    string GetProgramState();
  }
}