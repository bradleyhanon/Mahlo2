﻿using System.Collections.Generic;

namespace MahloService.Settings
{
  public interface IServiceSettings
  {
    string ServiceUrl { get; set; }

    int MaxSewinQueueRolls { get; set; }
    double MaxEndCheckRollPieceLength { get; set; }
    double MinSeamSpacing { get; set; }
    double SeamDetectIgnoreThreshold { get; set; }
    double MinRollLengthForLengthChecking { get; set; }
    double MinRollLengthForStyleAndRollCounting { get; set; }
    double SeamIndicatorKeepOnLength { get; set; }
    double RollTooLongFactor { get; set; }
    double RollTooShortFactor { get; set; }

    List<BackingCode> BackingCodes { get; }
    List<BackingSpec> BackingSpecs { get; }

    //bool ArchiveRollMaps { get; set; }
    //bool AutoCloseMahloDDEServer { get; set; }
    bool AutoSetRecipe { get; set; }
    int CheckAfterHowManyRolls { get; set; }
    int CheckAfterHowManyStyles { get; set; }
    //string DampeningAction { get; set; }
    //string DDEServername { get; set; }
    //string FlowDirection { get; set; }
    //string InstallFolder { get; set; }
    //int MainFormBackgroundColor { get; set; }
    //double MetersPerPixelFactor { get; set; }
    //bool PrintServerEnabled { get; set; }
    //string ProductImageRootFolder { get; set; }
    //int QueueRefreshRate { get; set; }
    int SeamDetectableThreshold { get; set; }
    //string SendEmailAlertsTo { get; set; }
    //string SqlServerMachine { get; set; 

    double Mahlo2SeamToBow { get; set; }
    double Mahlo2SeamToSkew { get; set; }
    double SeamToBowAndSkew { get; set; }
    double SeamToCutKnife { get; set; }

    BackingSpec GetBackingSpec(string backingCode);
  }
}