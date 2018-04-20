namespace Mahlo.Models
{
  interface IMahloRoll
  {
    int RollId { get; }
    string RollNo { get; }
    string StyleCode { get; }
    string StyleName { get; }
    string ColorCode { get; }
    string ColorName { get; }
    string BackingCode { get; }
    int RollLength { get; }
    double RollWidth { get; }
    string DefaultRecipe { get; }
    double PatternRepeatLength { get; }
    string ProductImageURL { get; }

    int Feet { get; set; }
    int Speed { get; set; }

    bool IsCheckRoll { get; }
  }
}