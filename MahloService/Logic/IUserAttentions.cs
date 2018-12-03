namespace MahloService.Logic
{
  internal interface IUserAttentions
  {
    bool Any { get; }
    bool IsRollTooLong { get; set; }
    bool IsRollTooShort { get; set; }
    bool IsSystemDisabled { get; set; }
    bool VerifyRollSequence { get; set; }
    void ClearAll();
  }

  internal interface IUserAttentions<Model> : IUserAttentions
  {
  }
}