public class CurrencyPair
{
   public string FromTo {get; set;}
   public string From {get; set;}
   public string To {get; set;}

   public CurrencyPair(string fromTo)
   {
      FromTo = fromTo;
      var currencies = FromTo.Split('/');
      From = currencies[0];
      To = currencies[1];
   }
   public CurrencyPair(string from, string to)
   {
      From = from;
      To = to;
      FromTo = From + "/" + To;
   }
   public CurrencyPair(Currency from, Currency to)
   {
      From = from.ToString();
      To = to.ToString();
      FromTo = From + "/" + To;
   }

   public override bool Equals(object? obj)
    {
        if (obj is CurrencyPair other)
        {
            return this.From == other.From && this.To == other.To;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(From, To); 
    }
}