﻿using UAlbion.Api;

namespace UAlbion.Core.Events
{
    [Event("set_clear_colour", "Sets the background colour to be drawn when no geometry is visible")]
    public class SetClearColourEvent : EngineEvent
    {
        [EventPart("red", "The red component of the background colour")]
        public float Red { get; }
        [EventPart("green", "The green component of the background colour")]
        public float Green { get; }
        [EventPart("blue", "The blue component of the background colour")]
        public float Blue { get; }

        public SetClearColourEvent(float red, float green, float blue) { Red = red; Green = green; Blue = blue; }
    }
}