namespace MyPF.Model
{
    internal class SavedListingInfo
    {
        public SavedListingInfo(uint listingId, string hostName, string worldName, bool isCrossWorld)
        {
            ListingId = listingId;
            HostName = hostName;
            WorldName = worldName;
            IsCrossWorld = isCrossWorld;
        }

        public uint ListingId { get; set; }
        public string HostName { get; set; }

        public string WorldName { get; set; }
        public bool IsCrossWorld { get; set; }
    }
}
