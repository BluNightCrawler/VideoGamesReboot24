namespace VideoGamesReboot24.Models
{
    public class Cart
    {
        public List<CartLine> Lines { get; set; } = new List<CartLine>();
        public void AddItem(VideoGameFull game, int quantity)
        {
            CartLine? line = Lines.Where(p => p.VideoGame.Id == game.Id).FirstOrDefault();
            if (line == null)
            {
                Lines.Add(new CartLine
                {
                    VideoGame = game,
                    Quantity = quantity
                });
            }
            else
            {
                line.Quantity += quantity;
            }
        }

        public void RemoveLine(VideoGameFull game)
        {
            Lines.RemoveAll(l => l.VideoGame.Id == game.Id);
        }

        public double ComputeTotalValue()
        {
            return Lines.Sum(e => e.VideoGame.Price * e.Quantity);
        }

        public void Clear() 
        { 
            Lines.Clear(); 
        }
    }
    public class CartLine
    {
        public int CartLineID { get; set; }
        public VideoGameFull VideoGame { get; set; } = new();
        public int Quantity { get; set; }
    }
}

