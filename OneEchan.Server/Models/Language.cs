using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Models
{
    public class Language
    {
        public Language(int id, string name)
        {
            Name = name;
        }
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public static class Languages
    {
        public static List<Language> AllLanguage { get; } = new List<Language>
        {
            new Language(0, "English"),
            new Language(1, "日本語"),
            new Language(2, "繁體中文"),
            new Language(3, "简体中文"),
        };
    }
}
