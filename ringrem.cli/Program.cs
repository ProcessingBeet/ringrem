﻿using ringrem.core;
using ringrem.models;
using ringrem.interfaces;
class Program
{    static void Main(string[] args)
    {
        ICommand? command = null;
        Core.Run(command);
    }

}