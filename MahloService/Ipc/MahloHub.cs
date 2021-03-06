﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using MahloService.Logic;
using MahloService.Models;
using MahloService.Repository;
using MahloService.Settings;
using MahloService.Utilities;
using Microsoft.AspNet.SignalR;

namespace MahloService.Ipc
{
  public class MahloHub : Hub
  {
    private readonly IMahloServer mahloServer;
    private readonly IDbMfg dbMfg;

    public MahloHub()
    {
      this.mahloServer = Program.Container.GetInstance<IMahloServer>();
      this.dbMfg = Program.Container.GetInstance<IDbMfg>();
    }

    public void RefreshAll()
    {
      this.mahloServer.RefreshAll(this.Context.ConnectionId);
    }

    public void MoveToPriorRoll(string name)
    {
      TaskUtilities.RunOnMainThreadAsync(() => GetMeterLogicInstance(name).MoveToPriorRoll()).NoWait();
    }

    public void MoveToNextRoll(string name, int currentRollLength)
    {
      TaskUtilities.RunOnMainThreadAsync(() => GetMeterLogicInstance(name).MoveToNextRoll(currentRollLength)).NoWait();
    }

    public void WaitForSeam(string name)
    {
      TaskUtilities.RunOnMainThreadAsync(() => GetMeterLogicInstance(name).WaitForSeam()).NoWait();
    }

    public void DisableSystem(string name)
    {
      TaskUtilities.RunOnMainThreadAsync(() => GetMeterLogicInstance(name).DisableSystem()).NoWait();
    }

    [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Not compatible with old clients")]
    public Task<(string message, string caption)> BasSetRecipe(string rollNo, string styleCode, string recipeName, bool isManualMode, RecipeApplyToEnum applyTo)
    {
      var tcs = new TaskCompletionSource<(string message, string caption)>();

      TaskUtilities.RunOnMainThreadAsync(async () =>
      {
        try
        {
          IBowAndSkewLogic bas = Program.Container.GetInstance<IBowAndSkewLogic>();
          ISewinQueue sewinQueue = Program.Container.GetInstance<ISewinQueue>();
          bool shouldApply = false;
          if (applyTo == RecipeApplyToEnum.Roll)
          {
            if (rollNo == bas.CurrentRoll.RollNo)
            {
              shouldApply = true;
            }
            else
            {
              // Make sure the selected roll isn't already passed.
              var selectedRoll = sewinQueue.Rolls.FirstOrDefault(roll => roll.RollNo == rollNo);
              if ((selectedRoll?.Id ?? 0) < bas.CurrentRoll.Id)
              {
                tcs.SetResult(($"Roll #{rollNo} is no longer the current roll. The recipe has not been changed.", "Not allowed!"));
                return;
              }
            }
          }
          else
          {
            shouldApply = styleCode == bas.CurrentRoll.StyleCode;
            rollNo = string.Empty;
          }

          if (shouldApply)
          {
            await this.BasApplyRecipeAsync(recipeName, isManualMode);
          }

          // Save settings to the database
          await this.dbMfg.BasUpdateDefaultRecipeAsync(styleCode, rollNo, recipeName);
          await sewinQueue.RefreshAsync();
          tcs.SetResult((string.Empty, string.Empty));
        }
        catch (Exception ex)
        {
          tcs.SetException(ex);
        }
      }).NoWait();

      return tcs.Task;
    }

    private Task BasApplyRecipeAsync(string recipeName, bool isManualMode)
    {
      TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
      TaskUtilities.RunOnMainThreadAsync(async () =>
      {
        try
        {
          var bas = Program.Container.GetInstance<IBowAndSkewLogic>();
          await bas.ApplyRecipeAsync(recipeName, isManualMode);
          tcs.SetResult(null);
        }
        catch (Exception ex)
        {
          tcs.SetException(ex);
        }
      }).NoWait();

      return tcs.Task;
    }

    [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Not compatible with old clients")]
    public Task<IEnumerable<CoaterScheduleRoll>> GetCoaterSchedule(int minSequence, int maxSequence)
    {
      var tcs = new TaskCompletionSource<IEnumerable<CoaterScheduleRoll>>();
      TaskUtilities.RunOnMainThreadAsync(async () =>
      {
        try
        {
          var result = await this.dbMfg.GetCoaterScheduleAsync(minSequence, maxSequence);
          tcs.SetResult(result);
        }
        catch (Exception ex)
        {
          tcs.SetException(ex);
        }
      }).NoWait();

      return tcs.Task;
    }

    public void MoveQueueRoll(int rollIndex, int direction)
    {
      ISewinQueue sewinQueue = Program.Container.GetInstance<ISewinQueue>();
      sewinQueue.MoveRoll(rollIndex, direction);
    }

    private static IMeterLogic GetMeterLogicInstance(string name)
    {
      switch (name)
      {
        case nameof(IMahloLogic):
          return Program.Container.GetInstance<IMahloLogic>();

        case nameof(IBowAndSkewLogic):
          return Program.Container.GetInstance<IBowAndSkewLogic>();

        case nameof(IPatternRepeatLogic):
          return Program.Container.GetInstance<IPatternRepeatLogic>();

        default:
          throw new InvalidOperationException($"MahloHub.GetMeterLogicInstance(\"{name}\")");
      }
    }

    public IServiceSettings GetServiceSettings()
    {
      return Program.Container.GetInstance<IServiceSettings>();
    }
  }
}
