using System;
using System.Collections.Generic;

namespace Aljurythm
{
    internal class Menu
    {
        public List<MenuItem> MainItems { get; set; }
        public List<MenuItem> ExtraItems { get; set; }
        public string Title { get; set; }

        public void Show()
        {
            if (!string.IsNullOrEmpty(Title))
            {
                Logger.WriteLine($"{Title}:", ConsoleColor.Magenta);
                Logger.WriteLine(new string('─', Title.Length + 1), ConsoleColor.Magenta);
            }

            foreach (var item in MainItems) Logger.WriteLine(item.ToString());
            Logger.LineBreak();

            if (ExtraItems.Count == 0) return;

            ExtraItems.RemoveAll(i => i.Condition == false);
            foreach (var item in ExtraItems) Logger.WriteLine(item.ToString(), ConsoleColor.Blue);
            Logger.LineBreak();
        }

        public void Prompt()
        {
            while (true)
            {
                Logger.Write("> ");
                var choice = Console.ReadLine()?.ToLower();
                Logger.ClearLast();

                var chosenItem = MainItems.Find(i => i.HotKey == choice) ?? ExtraItems.Find(i => i.HotKey == choice);
                if (chosenItem == null) continue;
                chosenItem.Action();

                if (chosenItem.PostAction != MenuItem.PostActionMode.START) continue;
                Logger.Clear();
                break;
            }
        }
    }
}