﻿namespace UAlbion.Formats.MapEvents
{
    public abstract class AsyncMapEvent : AsyncEvent, IMapEvent
    {
        static readonly EventContext Implicit = new EventContext(new EventSource.None());
        EventContext _context;

        public abstract MapEventType EventType { get; }
        public EventContext Context { get => _context ?? Implicit; set => _context = value; }
    }
}