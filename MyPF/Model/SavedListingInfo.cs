using System;

namespace MyPF.Model
{
    internal class SavedListingInfo
    {
        public SavedListingInfo(uint listingId, string hostName, string worldName,
            bool isCrossWorld, DateTime savedAt, DateTime lastRefreshed)
        {
            ListingId = listingId;
            HostName = hostName;
            WorldName = worldName;
            IsCrossWorld = isCrossWorld;
            SavedAt = savedAt;
            LastRefreshedAt = lastRefreshed;
        }

        public uint ListingId { get; set; }
        public string HostName { get; set; }

        public string WorldName { get; set; }
        public bool IsCrossWorld { get; set; }

        public DateTime SavedAt {  get; set; }

        public DateTime LastRefreshedAt { get; set; }
    }
}
