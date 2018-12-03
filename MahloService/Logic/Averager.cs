namespace MahloService.Logic
{
  internal class Averager
  {
    public double Count { get; private set; }
    public double Sum { get; private set; }
    public double Average => this.Sum / this.Count;

    public void Add(double value)
    {
      this.Sum += value;
      this.Count++;
    }

    public void Clear()
    {
      this.Sum = 0;
      this.Count = 0;
    }
  }
}
