namespace MahloService.Opc
{
  internal class OpcServerControllerSim : IOpcServerController
  {
    public void Start()
    {
      // We don't need to start the OPC server when we are simulating
    }
  }
}
