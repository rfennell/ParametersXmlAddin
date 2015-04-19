// Guids.cs
// MUST match guids.h
using System;

namespace BlackMarble.ParametersXmlAddin
{
    static class GuidList
    {
        public const string guidParametersXmlAddinPkgString = "633e9230-4287-47ed-8a7a-bbbdc80569f4";
        public const string guidParametersXmlAddinCmdSetString = "4db35f1c-7d70-4d3b-9098-c4d65acc177e";

        public static readonly Guid guidParametersXmlAddinCmdSet = new Guid(guidParametersXmlAddinCmdSetString);
    };
}