using System.Collections.Generic;
using MahloService.Models;

namespace MahloService.Repository
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