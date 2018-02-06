﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZloGUILauncher
{
    [Serializable]
    public class Params
    {
        public string AccentColor { get; set; }
        public string Theme { get; set; }
        public bool isDebug { get; set; }
        public bool MaximizeGameWindow { get; set; }
        public bool UseExternalImage { get; set; }
        public string ImagePath { get; set; }
        public bool autostartZclient { get; set; }
        public bool isMusicEnabled { get; set; }
        public Params() { }
    }

    [Serializable]
    public class Config
    {
        public Params config { get; set; }
        //public Theme theme { get; set; }
        //  internal string config = "Config.json";
        public Config()
        {
            config  = new Params();
            config.AccentColor = "Cobalt";
            config.Theme = "BaseDark";
            config.isDebug = false;
            config.MaximizeGameWindow = false;
            config.UseExternalImage = false;
            config.ImagePath = "";
            config.autostartZclient = false;
            config.isMusicEnabled = true;
        }
    }
}