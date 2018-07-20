using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MahloService.Settings
{
  internal class ServiceSettings : Westwind.Utilities.Configuration.AppConfiguration, IServiceSettings
  {
    public const double DEFAULT_METERS_PER_PIXEL = 0.00109d;

    //private double metersPerPixelFactor = DEFAULT_METERS_PER_PIXEL;

    public ServiceSettings()
    {
      this.Initialize();
    }

    public string ServiceUrl { get; set; } = "http://+:80/mahlo/";

    public double MaxEndCheckRollPieceLength { get; set; } = 10;
    public double MinSeamSpacing { get; set; } = 4;
    public double SeamDetectIgnoreThreshold { get; set; } = 0.5;
    public double MinRollLengthForLengthChecking { get; set; } = 100;
    public double MinRollLengthForStyleAndRollCounting { get; set; } = 50;
    public double SeamIndicatorKeepOnLength { get; set; } = 10;
    public double RollTooLongFactor { get; set; } = 1.1;
    public double RollTooShortFactor { get; set; } = 0.9;

    public List<BackingSpec> BackingSpecs { get; set; } = new List<BackingSpec>
    {
      new BackingSpec { Backing = "SA", MaxBow = 0.5, MaxSkew = 1.25, DlotSpec = 0.0100 },
      new BackingSpec { Backing = "IR", MaxBow = 0.25, MaxSkew = 0.75, DlotSpec = 0.0075 },
      new BackingSpec { Backing = "HL", MaxBow = 0.25, MaxSkew = 0.75, DlotSpec = 0.0075 },
    };

    //public int MainFormBackgroundColor { get; set; } = 0xECF2F2;
    //public string DDEServername { get; set; } = "BowAndSkew";
    //public bool AutoCloseMahloDDEServer { get; set; } = false;
    public bool AutoSetRecipe { get; set; } = false;
    //public bool PrintServerEnabled { get; set; } = false;
    //public bool ArchiveRollMaps { get; set; } = false;
    //public string InstallFolder { get; set; } = Application.StartupPath;
    //public string SqlServerMachine { get; set; } = "CalSql1";
    //public string ProductImageRootFolder { get; set; } = @"\\calmy2\ProductImages\Commercial\Carpet\Styles";
    //public string FlowDirection { get; set; } = "Left to Right";
    //public int QueueRefreshRate { get; set; } = 20;
    //public string DampeningAction { get; set; } = "Average last 10 values";
    //public double MetersPerPixelFactor
    //{
    //  get => this.metersPerPixelFactor;
    //  set => this.metersPerPixelFactor = Math.Max(value, DEFAULT_METERS_PER_PIXEL);
    //}

    public double Mahlo2SeamToBow { get; set; } = 9.0 + 10.5 / 12.0;    // 9' 10 1/2"
    public double Mahlo2SeamToSkew { get; set; } = 14.0 + 1.5 / 12.0;   // 14' 1 1/2"
    public double SeamToBowAndSkew { get; set; } = 4.0 + 7.75 / 12.0;   // 7' 7 3/4"
    public double SeamToCutKnife { get; set; } = 15.0 + 2.5 / 12.0;     // 15' 2 1/2"


    public int SeamDetectableThreshold { get; set; } = 5; // Distance within which seam detects are ignored
    public int CheckAfterHowManyRolls { get; set; } = 10;
    public int CheckAfterHowManyStyles { get; set; } = 3;
    //public string SendEmailAlertsTo { get; set; } = "Calhoun.Mahlo.Alerts";

    public BackingSpec GetBackingSpec(string backingCode)
    {
      var result = this.BackingSpecs.FirstOrDefault(item => item.Backing == backingCode) ??
        this.BackingSpecs.First(item => item.Backing != "SA");

      return result;
    }
  }
}
