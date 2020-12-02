namespace StackItUp
{
    public struct Score
    {
        public int Points;
        public int BlocksStacked;
        public int ComboCount;
        public int MinComboPoints;
        public int LastPoints;
        public bool IsCombo => this.ComboCount > 1;

        public void AddPoints(int points)
        {
            if (points >= this.MinComboPoints)
            {
                this.ComboCount++;
            }
            else
            {
                this.ComboCount = 0;
            }

            if (this.ComboCount >= 2)
            {
                points *= this.ComboCount;
            }

            this.Points += points;
            this.LastPoints = points;
        }

        public void Reset()
        {
            this.Points = 0;
            this.BlocksStacked = 0;
        }
    }
}
