namespace OCR_BusinessLayer.Classes
{
    public class Column
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
        public int Top { get; set; }
        public int FirstLineInColumn { get; set; } = 1;
        public bool Completed { get; set; } = false;
        public bool Blocked { get; set; } = false; // true if column is above another column

        public Column(){}

        public Column(string t,int l,int r,int bot,int top)
        {
            Text = t;
            Left = l;
            Right = r;
            Bottom = bot;
            Top = top;
        }
        
    }
}
