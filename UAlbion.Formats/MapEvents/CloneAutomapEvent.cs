﻿using System;
using System.IO;

namespace UAlbion.Formats.MapEvents
{
    public class CloneAutomapEvent : MapEvent
    {
        public CloneAutomapEvent(BinaryReader br, int id)
        {
            throw new NotImplementedException();
        }

        public override EventType Type => EventType.CloneAutomap;
    }
}