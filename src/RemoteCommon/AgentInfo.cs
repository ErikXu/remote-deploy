﻿using System;

namespace RemoteCommon
{
    public class AgentInfo
    {
        public string SessionId { get; set; }

        public string Ip { get; set; }

        public int Port { get; set; }

        public DateTime ConnectTime { get; set; }
    }
}