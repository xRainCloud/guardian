﻿using Guardian.Utilities;
using System.Text.RegularExpressions;

namespace Guardian.Features.Commands.Impl
{
    class CommandTranslate : Command
    {
        // TODO: Maybe I'll use this again, not sure.
        private static readonly Regex SpecialChars = new Regex("[~!@#$%^&*()_+`\\-=\\[\\]{}\\|;:'\",<.>\\/?]", RegexOptions.IgnoreCase);

        private Translator translator = new Translator();

        public CommandTranslate() : base("translate", new string[0], "<langfrom> <langto> <message>", false) { }

        public override void Execute(InRoomChat irc, string[] args)
        {
            if (args.Length > 2)
            {
                Mod.Instance.StartCoroutine(Translator.Translate(string.Join(" ", args.CopyOfRange(2, args.Length)), args[0], args[1], result =>
                {
                    if (result.Length > 1)
                    {
                        irc.AddMessage("Translation ".WithColor("FFCC00") + $"({result[0].ToUpper()} -> {args[1].ToUpper()})", result[1]);
                    }
                    else
                    {
                        irc.AddLine("An error occured while trying to retrieve the translation!".WithColor("FF0000"));
                    }
                }));
            }
        }
    }
}