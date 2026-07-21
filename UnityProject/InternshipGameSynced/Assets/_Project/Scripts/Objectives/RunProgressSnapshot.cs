namespace InternshipGame.Prototype
{
    public readonly struct RunProgressSnapshot
    {
        public RunProgressSnapshot(
            int totalRegistered,
            int requiredDeposits,
            int identifiedLive,
            int identifiedInert,
            int carried,
            int deposited)
        {
            TotalRegistered = totalRegistered;
            RequiredDeposits = requiredDeposits;
            IdentifiedLive = identifiedLive;
            IdentifiedInert = identifiedInert;
            Carried = carried;
            Deposited = deposited;
        }

        public int TotalRegistered { get; }
        public int RequiredDeposits { get; }
        public int IdentifiedLive { get; }
        public int IdentifiedInert { get; }
        public int Carried { get; }
        public int Deposited { get; }
        public int RemainingDeposits => System.Math.Max(0, RequiredDeposits - Deposited);
        public bool IsComplete => RequiredDeposits > 0 && Deposited >= RequiredDeposits;

        public static RunProgressSnapshot Empty(int requiredDeposits = 3)
        {
            return new RunProgressSnapshot(0, requiredDeposits, 0, 0, 0, 0);
        }
    }

    public enum DepositResult
    {
        Accepted,
        Unassayed,
        Inert,
        Duplicate,
        Unregistered,
        ObjectiveComplete,
        RunInactive,
        RackFull,
    }
}
