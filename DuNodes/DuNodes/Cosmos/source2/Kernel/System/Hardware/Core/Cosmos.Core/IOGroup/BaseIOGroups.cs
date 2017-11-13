namespace Cosmos.Core.IOGroup {
    public class BaseIOGroups {
        // These are common/fixed pieces of hardware. PCI, USB etc should be self discovering
        // and not hardcoded like this.
        // Further more some kind of security needs to be applied to these, but even now
        // at least we have isolation between the consumers that use these.
        public readonly Core.IOGroup.Keyboard Keyboard = new Core.IOGroup.Keyboard();
        public static readonly Core.IOGroup.Mouse Mouse = new Core.IOGroup.Mouse();
        public static readonly Core.IOGroup.PCSpeaker PCSpeaker = new Core.IOGroup.PCSpeaker();
        public readonly Core.IOGroup.PIT PIT = new Core.IOGroup.PIT();
        public readonly Core.IOGroup.TextScreen TextScreen = new Core.IOGroup.TextScreen();
        public readonly Core.IOGroup.ATA ATA1 = new Core.IOGroup.ATA(false);
        public readonly Core.IOGroup.ATA ATA2 = new Core.IOGroup.ATA(true);
        public readonly Core.IOGroup.RTC RTC = new Core.IOGroup.RTC();
    }
}
