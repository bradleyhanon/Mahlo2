using System.ComponentModel;

namespace MahloService
{
  [RunInstaller(true)]
  public partial class ProjectInstaller : System.Configuration.Install.Installer
  {
    public ProjectInstaller()
    {
      this.InitializeComponent();
    }
  }
}
