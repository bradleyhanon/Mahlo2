namespace MahloService.Logic
{
  internal class Averager
  {
    private double? priorCounter;

    public double Count { get; private set; }
    public double Sum { get; private set; }
    public double Average => this.Count == 0.0 ? 0.0 : this.Sum / this.Count;

    public void Add(double value)
    {
      this.Sum += value;
      this.Count++;
    }

    public void Add(double value, double counter)
    {
      if (this.priorCounter.HasValue)
      {
        var weight = counter - this.priorCounter.Value;
        this.Count += weight;
        this.Sum += weight * value;
      }

      this.priorCounter = counter;
    }

    public void Clear()
    {
      this.Sum = 0;
      this.Count = 0;
    }
  }
}
