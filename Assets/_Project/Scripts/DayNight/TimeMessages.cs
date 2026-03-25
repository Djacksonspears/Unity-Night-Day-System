
// Basic triggers
public struct SunriseMsg : IMessage { }
public struct SunsetMsg : IMessage { }

// Data-carrying message
public struct HourChangedMsg : IMessage { 
    public int NewHour; 
}