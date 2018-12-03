namespace MahloService.Repository
{
  internal interface IProgramStateProvider
  {
    string GetProgramState();
    void SaveProgramState(string state);
  }
}
