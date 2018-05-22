using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MahloService.Logic;
using MahloService.Models;
using MahloService.Opc;
using MahloService.Repository;
using Microsoft.AspNet.SignalR;

namespace MahloService.Ipc
{
  public class MahloHub : Hub
  {
    IMahloServer mahloServer;
    IDbMfg dbMfg;
    SynchronizationContext syncContext;

    public MahloHub()
    {
      this.mahloServer = Program.Container.GetInstance<MahloServer>();
      this.dbMfg = Program.Container.GetInstance<DbMfg>();
      this.syncContext = Program.Container.GetInstance<SynchronizationContext>();
    }

    public void RefreshAll()
    {
      this.mahloServer.RefreshAll(this.Context.ConnectionId);
    }

    public void MoveToPriorRoll(string name)
    {
      this.syncContext.Post(_ => this.GetMeterLogicInstance(name).MoveToPriorRoll(), null);
    }

    public void MoveToNextRoll(string name)
    {
      this.syncContext.Post(_ => this.GetMeterLogicInstance(name).MoveToNextRoll(), null);
    }

    public void WaitForSeam(string name)
    {
      this.syncContext.Post(_ => this.GetMeterLogicInstance(name).WaitForSeam(), null);
    }
    public Task<(string message, string caption)> BasSetRecipe(string rollNo, string styleCode, string recipeName, bool isManualMode, RecipeApplyToEnum applyTo)
    {
      var tcs = new TaskCompletionSource<(string message, string caption)>();

      this.syncContext.Post(async _ =>
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
            await this.BasApplyRecipe(recipeName, isManualMode);
          }

          // Save settings to the database
          await this.dbMfg.BasUpdateDefaultRecipe(styleCode, rollNo, recipeName);
          await sewinQueue.Refresh();
          tcs.SetResult((string.Empty, string.Empty));
        }
        catch (Exception ex)
        {
          tcs.SetException(ex);
        }
      }, null);

      return tcs.Task;
    }

    public Task BasApplyRecipe(string recipeName, bool isManualMode)
    {
      TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
      this.syncContext.Post(_ =>
      {
        try
        {
          var bas = Program.Container.GetInstance<IBowAndSkewLogic>();
          var meterSrc = Program.Container.GetInstance<IMeterSrc<BowAndSkewRoll>>();
          if (isManualMode)
          {
            if (bas.IsManualMode)
            {
              meterSrc.SetAutoMode(false);
            }
            else
            {
              meterSrc.SetRecipe(recipeName);
              meterSrc.SetAutoMode(true);
            }
          }

          tcs.SetResult(null);
        }
        catch (Exception ex)
        {
          tcs.SetException(ex);
        }
      }, null);

      return tcs.Task;
    }

    public Task<IEnumerable<CoaterScheduleRoll>> GetCoaterSchedule(int minSequence, int maxSequence)
    {
      var tcs = new TaskCompletionSource<IEnumerable<CoaterScheduleRoll>>();
      this.syncContext.Post(async _ =>
      {
        try
        {
          var result = await this.dbMfg.GetCoaterSchedule(minSequence, maxSequence);
          tcs.SetResult(result);
        }
        catch (Exception ex)
        {
          tcs.SetException(ex);
        }
      }, null);

      return tcs.Task;
    }

    private IMeterLogic GetMeterLogicInstance(string name)
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
  }
}
