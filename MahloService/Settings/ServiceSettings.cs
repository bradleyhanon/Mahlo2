using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MahloService.Settings
{
  internal class ServiceSettings : Westwind.Utilities.Configuration.AppConfiguration, IServiceSettings
  {
    public const double DEFAULT_METERS_PER_PIXEL = 0.00109d;
    private const string Vinyl = "Vinyl";

    //private double metersPerPixelFactor = DEFAULT_METERS_PER_PIXEL;

    public ServiceSettings()
    {
      this.Initialize();
      this.ValidateBackingSpecs();
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

    public List<BackingCode> BackingCodes { get; set; } = new List<BackingCode>
    {
      new BackingCode { Code = "BL", Backing = "Latex" },
      new BackingCode { Code = "HB", Backing = "Latex" },
      new BackingCode { Code = "HL", Backing = "Vinyl" },
      new BackingCode { Code = "HR", Backing = "Urethane" },
      new BackingCode { Code = "IR", Backing = "Vinyl" },
      new BackingCode { Code = "LP", Backing = "Latex" },
      new BackingCode { Code = "SA", Backing = "Latex" },
      new BackingCode { Code = "SI", Backing = "Latex" },
      new BackingCode { Code = "TP", Backing = "Latex" },
      new BackingCode { Code = "UL", Backing = "Latex" },
    };

    public List<BackingSpec> BackingSpecs { get; set; } = new List<BackingSpec>
    {
      new BackingSpec { Backing = "Latex", MaxBow = 0.5, MaxSkew = 1.25, MaxElongation = 0.01, DlotSpec = 0.0100 },
      new BackingSpec { Backing = "Vinyl", MaxBow = 0.25, MaxSkew = 0.75, MaxElongation = 0.0067, DlotSpec = 0.0075 },
      new BackingSpec { Backing = "Urethane", MaxBow = 0.25, MaxSkew = 0.75, MaxElongation = 0.0067, DlotSpec = 0.0075 },
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
      string backing = this.BackingCodes.FirstOrDefault(item => item.Code.Equals(backingCode, StringComparison.OrdinalIgnoreCase))?.Backing;
      if (backing == null)
      {
        this.BackingCodes.Add(new BackingCode { Code = backingCode, Backing = Vinyl });
      }

      BackingSpec spec = this.BackingSpecs.FirstOrDefault(item => item.Backing.Equals(backing, StringComparison.OrdinalIgnoreCase));
      if (spec == null)
      {
        spec = this.BackingSpecs.FirstOrDefault(item => item.Backing.Equals(Vinyl, StringComparison.OrdinalIgnoreCase));
        if (spec == null)
        {
          throw new Exception($"Unable to find specifications for Vinyl backing.");
        }
      }

      return spec;
    }

    private void ValidateBackingSpecs()
    {
      StringBuilder builder = new StringBuilder();

      var dupCodes = this.BackingCodes
        .GroupBy(item => item.Code, StringComparer.OrdinalIgnoreCase)
        .Where(item => item.Count() > 1)
        .Select(item => item.Key)
        .ToArray();

      AppendMessage("Duplicate BackingCodes: ", dupCodes);


      var dupSpecs = this.BackingSpecs
        .GroupBy(item => item.Backing, StringComparer.OrdinalIgnoreCase)
        .Where(item => item.Count() > 1)
        .Select(item => item.Key)
        .ToArray();

      AppendMessage("Duplicate BackingSpecs: ", dupSpecs);

      var missingSpecs = this.BackingCodes
        .Where(code => this.BackingSpecs
           .FirstOrDefault(spec => code.Backing.Equals(spec.Backing, StringComparison.OrdinalIgnoreCase)) == null)
        .Select(code => code.Backing);

      if (!this.BackingCodes.Any(item => item.Backing.Equals(Vinyl, StringComparison.OrdinalIgnoreCase)))
      {
        missingSpecs = missingSpecs
          .Concat(Enumerable.Repeat(Vinyl, 1))
          .Distinct(StringComparer.OrdinalIgnoreCase);
      }

      AppendMessage("BackingSpecs not found for: ", missingSpecs);

      if (builder.Length > 0)
      {
        throw new ApplicationException(builder.ToString());
      }

      void AppendMessage(string message, IEnumerable<string> values)
      {
        if (values.Any())
        {
          if (builder.Length == 0)
          {
            builder.AppendLine("Errors found in MahloService.exe.config");
          }

          builder.Append(message);
          values.ForEach(item => builder.Append($"'{item}', "));
          builder.Length -= 2;
          builder.AppendLine();
        }
      }
    }
  }
}
