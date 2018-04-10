namespace Mahlo.AppSettings
{
  interface IOpcSettings
  {
    IMahloOpcSettings Mahlo { get; }
    IPlcSettings Seam { get; }
  }
}