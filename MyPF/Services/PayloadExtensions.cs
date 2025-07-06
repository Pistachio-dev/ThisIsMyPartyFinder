using Dalamud.Game.Text.SeStringHandling;

namespace MyPF.Services
{
    public static class PayloadExtensions
    {
        public static string GetText(this Payload payload)
        {
            if (payload == null || !(payload is ITextProvider))
            {
                return string.Empty;
            }

            return (payload as ITextProvider)!.Text;
        }

        public static Payload? GetPayload(this SeString seString, int payloadIndex)
        {
            if (seString.Payloads.Count <= payloadIndex)
            {
                return null;
            }

            return seString.Payloads[payloadIndex];
        }
    }
}
